using System;
using System.Linq;
using System.Threading;
using bank.Core;
using bank.Utils;

namespace bank.Services
{
    public class TransferService
    {
        private readonly Bank bank;
        private readonly AccountService accountService;
        private readonly ExchangerateService exchangeRateService = new ExchangerateService();

        public TransferService(Bank bank, AccountService accountService)
        {
            this.bank = bank;
            this.accountService = accountService;
        }

        private bool ValidatePin(User user)
        {
            for (int i = 0; i < 3; i++)
            {
                var pin = ConsoleHelper.PromptWithEscapeMasked("Enter PIN");
                if (pin == "<ESC>") return false;
                if (pin == user.PIN) return true;
                ConsoleHelper.WriteWarning("Incorrect PIN.");
            }

            ConsoleHelper.WriteWarning("Too many attempts. Returning...");
            return false;
        }

        private bool ApplyWithdrawal(Account from, decimal amount, out decimal fee, out string type)
        {
            fee = 0;
            type = "Standard";
            return from.Balance >= amount;
        }

        // INTERNAL TRANSFER
        public void DoTransferOwn(User currentUser)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER BETWEEN OWN ACCOUNTS");

            // Must have at least two accounts total
            if (currentUser.Accounts.Count < 2)
            {
                ConsoleHelper.WriteWarning("You need at least two accounts to transfer between them.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Get accounts with balance > 0
            var sourceAccounts = currentUser.Accounts
                .Where(a => a.Balance > 0)
                .ToList();

            // Must have at least ONE account with balance
            if (sourceAccounts.Count == 0)
            {
                ConsoleHelper.WriteWarning("No accounts have available balance.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // If only 1 account has balance, ensure destination options exist
            if (sourceAccounts.Count == 1)
            {
                var onlySource = sourceAccounts[0];

                // Find all possible destination accounts
                var possibleDestinations = currentUser.Accounts
                    .Where(a => a != onlySource)
                    .ToList();

                if (!possibleDestinations.Any())
                {
                    ConsoleHelper.WriteWarning("You only have one usable account. No valid destination.");
                    ConsoleHelper.PauseWithMessage();
                    return;
                }
            }

            // ----- SELECT SOURCE ACCOUNT -----
            Console.WriteLine("Select source account:\n");
            for (int i = 0; i < sourceAccounts.Count; i++)
                Console.WriteLine($"{i + 1}. {sourceAccounts[i].AccountNumber} | {sourceAccounts[i].Balance:N2} {sourceAccounts[i].Currency}");

            Console.Write("\nChoose: ");
            if (!int.TryParse(Console.ReadLine(), out int srcChoice) ||
                srcChoice < 1 || srcChoice > sourceAccounts.Count)
                return;

            var fromAcc = sourceAccounts[srcChoice - 1];

            // Destination accounts must be ALL OTHER accounts
            var destList = currentUser.Accounts
                .Where(a => a != fromAcc)
                .ToList();

            if (!destList.Any())
            {
                ConsoleHelper.WriteWarning("No valid destination account.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // ----- SELECT DESTINATION ACCOUNT -----
            Console.WriteLine("\nSelect destination account:\n");
            for (int i = 0; i < destList.Count; i++)
                Console.WriteLine($"{i + 1}. {destList[i].AccountNumber} | {destList[i].Balance:N2} {destList[i].Currency}");

            Console.Write("\nChoose: ");
            if (!int.TryParse(Console.ReadLine(), out int dstChoice) ||
                dstChoice < 1 || dstChoice > destList.Count)
                return;

            var toAcc = destList[dstChoice - 1];

            // ----- AMOUNT -----
            Console.Clear();
            ConsoleHelper.WriteHeader("ENTER AMOUNT");

            decimal amount = 0;
            int amountAttempts = 0;

            while (amountAttempts < 2)
            {
                Console.Write($"Enter Amount ({fromAcc.Currency}): ");
                if (decimal.TryParse(Console.ReadLine(), out amount) && amount > 0)
                {
                    if (amount <= fromAcc.Balance)
                        break;

                    ConsoleHelper.WriteError("Amount exceeds available balance.");
                }
                else
                {
                    ConsoleHelper.WriteError("Invalid amount.");
                }

                amountAttempts++;
            }

            if (amountAttempts == 2)
            {
                ConsoleHelper.WriteError("Too many failed attempts.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // PIN CHECK
            Console.Clear();
            ConsoleHelper.WriteHeader("ENTER PIN TO CONFIRM TRANSFER");
            if (!ValidatePin(currentUser)) return;

            if (!ApplyWithdrawal(fromAcc, amount, out decimal fee, out string _))
            {
                ConsoleHelper.WriteError("Insufficient coverage.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Create pending transfer
            string txId = Bank.GenerateTransactionId();
            DateTime releaseTime = DateTime.UtcNow.AddMinutes(3);

            decimal converted = amount;
            bool sameCurrency = fromAcc.Currency == toAcc.Currency;

            if (!sameCurrency)
                converted = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAcc.Currency);

            var pendingTx = new Transaction(
                id: txId,
                accountNumber: fromAcc.AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Transfer",
                amount: amount,
                currency: fromAcc.Currency,
                accountType: fromAcc.AccountType,
                fromAccount: fromAcc.AccountNumber,
                toAccount: toAcc.AccountNumber,
                fromUser: currentUser.Name,
                toUser: currentUser.Name
            )
            {
                IsPending = true,
                Status = "Pending",
                IsInternal = true,
                ReleaseAt = releaseTime,
                ConvertedAmount = converted,
                TargetCurrency = toAcc.Currency
            };

            fromAcc.Transactions.Add(pendingTx);
            toAcc.Transactions.Add(pendingTx);

            bank.PendingTransfers.Add(pendingTx);

            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER INITIATED");
            ConsoleHelper.WriteSuccess($"Transfer is now pending (3 minutes).");
            ConsoleHelper.WriteSuccess($"Transaction ID: {txId}");
            ConsoleHelper.WriteInfo($"From: {fromAcc.AccountNumber}");
            ConsoleHelper.WriteInfo($"To: {toAcc.AccountNumber}");
            ConsoleHelper.WriteInfo($"Amount: {amount:N2} {fromAcc.Currency}");

            ConsoleHelper.PauseWithMessage();
        }


        // EXTERNAL TRANSFER
        public void TransferToOther(User sender)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER TO ANOTHER CUSTOMER");

            var accounts = sender.Accounts
                .Where(a => a.Balance > 0)
                .ToList();

            if (!accounts.Any())
            {
                ConsoleHelper.WriteWarning("No available accounts.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            for (int i = 0; i < accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {accounts[i].AccountNumber} | {accounts[i].Balance:N2} {accounts[i].Currency}");

            Console.Write("\nChoose account: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > accounts.Count)
                return;

            var fromAcc = accounts[choice - 1];

            Console.Write("Enter recipient account number: ");
            var toNumber = Console.ReadLine()?.Trim();

            var toAcc = bank.FindAccount(toNumber);

            if (toAcc == null || toAcc.Owner == sender)
            {
                ConsoleHelper.WriteError("Invalid recipient.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            ConsoleHelper.WriteInfo($"Recipient: {toAcc.Owner.Name}");

            decimal amount = 0;
            int amountAttempts = 0;

            while (amountAttempts < 2)
            {
                Console.Write($"Enter Amount ({fromAcc.Currency}): ");
                if (decimal.TryParse(Console.ReadLine(), out amount) && amount > 0)
                {
                    if (amount <= fromAcc.Balance)
                        break;

                    ConsoleHelper.WriteError("Amount exceeds available balance.");
                }
                else
                {
                    ConsoleHelper.WriteError("Invalid amount.");
                }

                amountAttempts++;
            }

            if (amountAttempts == 2)
            {
                ConsoleHelper.WriteError("Too many failed attempts.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            Console.Clear();
            ConsoleHelper.WriteHeader("ENTER PIN TO CONFIRM TRANSFER");
            if (!ValidatePin(sender)) return;

            if (!accountService.CanWithdraw(fromAcc, amount))
            {
                ConsoleHelper.WriteError("Insufficient funds.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            decimal converted = amount;
            bool sameCurrency = fromAcc.Currency == toAcc.Currency;

            if (!sameCurrency)
                converted = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAcc.Currency);

            fromAcc.Balance -= amount;

            string transactionId = Bank.GenerateTransactionId();

            var tx = new Transaction(
                id: transactionId,
                accountNumber: fromAcc.AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Transfer",
                amount: amount,
                currency: fromAcc.Currency,
                accountType: fromAcc.AccountType,
                fromAccount: fromAcc.AccountNumber,
                toAccount: toAcc.AccountNumber,
                fromUser: sender.Name,
                toUser: toAcc.Owner.Name
            )
            {
                IsPending = true,
                Status = "Pending",
                ReleaseAt = DateTime.UtcNow.AddMinutes(3),
                IsInternal = false
            };

            fromAcc.Transactions.Add(tx);

            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER CREATED");

            ConsoleHelper.WriteSuccess($"Transfer created: {amount:N2} {fromAcc.Currency}");
            ConsoleHelper.WriteSuccess("Status: Pending (3 minutes)");
            ConsoleHelper.WriteSuccess($"Transaction ID: {tx.Id}");
            ConsoleHelper.WriteSuccess($"Your new balance: {fromAcc.Balance:N2} {fromAcc.Currency}");

            ConsoleHelper.PauseWithMessage();
        }
    }
}
