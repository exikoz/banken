using System;
using System.Linq;
using bank.Core;

namespace bank.Services
{
    /// <summary>
    /// Handles all types of account transfers: between own accounts and to other users.
    /// </summary>
    public class TransferService
    {
        private readonly Bank bank;
        private readonly AccountService accountService;
        private readonly ExchangerateService exchangeRateService = new ExchangerateService();

        public TransferService(Bank bank, AccountService accountService)
        {
            this.bank = bank ?? throw new ArgumentNullException(nameof(bank));
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        /// <summary>
        /// Transfer money between the user's own accounts.
        /// </summary>
        public void DoTransferOwn(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSFER BETWEEN YOUR OWN ACCOUNTS ===\n");

            if (currentUser.Accounts.Count < 2)
            {
                Console.WriteLine("You need at least two accounts to perform an internal transfer.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                var sourceAccounts = currentUser.Accounts.Where(a => a.Balance > 0).ToList();
                if (!sourceAccounts.Any())
                {
                    Console.WriteLine("No accounts with positive balance available for transfer.");
                    Console.ReadKey();
                    return;
                }

                Console.Clear();
                Console.WriteLine("=== SELECT SOURCE ACCOUNT ===\n");
                for (int i = 0; i < sourceAccounts.Count; i++)
                {
                    var acc = sourceAccounts[i];
                    Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
                }

                Console.Write("\nChoose source (number): ");
                if (!int.TryParse(Console.ReadLine(), out var srcIdx) || srcIdx < 1 || srcIdx > sourceAccounts.Count)
                {
                    Console.WriteLine("\nInvalid selection. Please try again.");
                    Console.ReadKey();
                    continue;
                }

                var fromAcc = sourceAccounts[srcIdx - 1];

                var destinationAccounts = currentUser.Accounts.Where(a => a.AccountNumber != fromAcc.AccountNumber).ToList();
                Console.Clear();
                Console.WriteLine("=== SELECT DESTINATION ACCOUNT ===\n");
                for (int i = 0; i < destinationAccounts.Count; i++)
                {
                    var acc = destinationAccounts[i];
                    Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
                }

                Console.Write("\nChoose destination (number): ");
                if (!int.TryParse(Console.ReadLine(), out var dstIdx) || dstIdx < 1 || dstIdx > destinationAccounts.Count)
                {
                    Console.WriteLine("\nInvalid selection. Please try again.");
                    Console.ReadKey();
                    continue;
                }

                var toAcc = destinationAccounts[dstIdx - 1];

                Console.Write($"\nAmount ({fromAcc.Currency}): ");
                if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
                {
                    Console.WriteLine("\nInvalid amount. Please enter a positive number.");
                    Console.ReadKey();
                    continue;
                }

                if (amount > fromAcc.Balance)
                {
                    Console.WriteLine("\nInsufficient funds in selected account.");
                    Console.ReadKey();
                    continue;
                }

                decimal convertedAmount = amount;
                if (fromAcc.Currency != toAcc.Currency)
                {
                    convertedAmount = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAcc.Currency);
                    Console.WriteLine($"\nCurrency converted: {amount:N2} {fromAcc.Currency} = {convertedAmount:N2} {toAcc.Currency}");
                }

                // Create transactions for both accounts
                var withdrawTx = new Transaction(Guid.NewGuid().ToString("N"), fromAcc.AccountNumber, DateTime.UtcNow, "Withdraw", amount)
                {
                    Currency = fromAcc.Currency,
                    FromUser = currentUser.Name,
                    ToUser = currentUser.Name,
                    FromAccount = fromAcc.AccountNumber,
                    ToAccount = toAcc.AccountNumber
                };

                var depositTx = new Transaction(Guid.NewGuid().ToString("N"), toAcc.AccountNumber, DateTime.UtcNow, "Deposit", convertedAmount)
                {
                    Currency = toAcc.Currency,
                    FromUser = currentUser.Name,
                    ToUser = currentUser.Name,
                    FromAccount = fromAcc.AccountNumber,
                    ToAccount = toAcc.AccountNumber
                };

                fromAcc.Balance -= amount;
                toAcc.Balance += convertedAmount;

                fromAcc.Transactions.Add(withdrawTx);
                toAcc.Transactions.Add(depositTx);

                Console.WriteLine($"\nTransfer completed: {amount:N2} {fromAcc.Currency} from {fromAcc.AccountNumber} to {toAcc.AccountNumber}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                break;
            }
        }

        /// <summary>
        /// Interactive transfer to another user's account.
        /// Only shows sender's own accounts and hides all others.
        /// </summary>
        public void TransferToOther(User sender)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSFER TO ANOTHER CUSTOMER ===\n");

            // Show only sender's own accounts with available balance
            var availableAccounts = sender.Accounts.Where(a => a.Balance > 0).ToList();
            if (!availableAccounts.Any())
            {
                Console.WriteLine("You don't have any accounts with available balance.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Select one of your accounts to send money from:\n");
            for (int i = 0; i < availableAccounts.Count; i++)
            {
                var acc = availableAccounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
            }

            Console.Write("\nChoose account (number): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > availableAccounts.Count)
            {
                Console.WriteLine("\nInvalid selection. Press any key to return.");
                Console.ReadKey();
                return;
            }

            var fromAccount = availableAccounts[selectedIndex - 1];

            // Ask for recipient account number (no list of other users shown)
            Console.Write("\nEnter recipient account number: ");
            var toAccountNumber = Console.ReadLine()?.Trim();

            var recipientAccount = bank.FindAccount(toAccountNumber!);
            if (recipientAccount == null)
            {
                Console.WriteLine("\nNo account found with that number.");
                Console.ReadKey();
                return;
            }

            // Display recipient name only
            Console.WriteLine($"\nRecipient: {recipientAccount.Owner.Name}");

            // Ask for transfer amount
            Console.Write($"Amount ({fromAccount.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("\nInvalid amount entered.");
                Console.ReadKey();
                return;
            }

            // Perform the transfer
            bool result = TransferToOther(sender, fromAccount.AccountNumber, toAccountNumber!, amount);

            if (result)
                Console.WriteLine("\nTransfer completed successfully.");
            else
                Console.WriteLine("\nTransfer failed.");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Executes a transfer between different users' accounts.
        /// </summary>
        public bool TransferToOther(User sender, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (sender == null)
                return false;

            var from = bank.FindAccount(fromAccountNumber);
            var to = bank.FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                Console.WriteLine("Transfer: One or both accounts not found.");
                return false;
            }

            if (from.Owner != sender)
            {
                Console.WriteLine("Transfer: You can only send money from your own account.");
                return false;
            }

            if (to.Owner == sender)
            {
                Console.WriteLine("Transfer: Use the 'own account' option instead.");
                return false;
            }

            if (!HasCoverage(from, amount))
            {
                Console.WriteLine("Transfer: Insufficient funds or overdraft limit reached.");
                return false;
            }

            decimal convertedAmount = amount;
            if (from.Currency != to.Currency)
            {
                convertedAmount = exchangeRateService.ConvertCurrency(amount, from.Currency, to.Currency);
                Console.WriteLine($"Currency converted: {amount:N2} {from.Currency} = {convertedAmount:N2} {to.Currency}");
            }

            // Create transactions
            var withdrawTx = new Transaction(Guid.NewGuid().ToString("N"), from.AccountNumber, DateTime.UtcNow, "Withdraw", amount)
            {
                Currency = from.Currency,
                FromUser = sender.Name,
                ToUser = to.Owner.Name,
                FromAccount = from.AccountNumber,
                ToAccount = to.AccountNumber
            };

            var depositTx = new Transaction(Guid.NewGuid().ToString("N"), to.AccountNumber, DateTime.UtcNow, "Deposit", convertedAmount)
            {
                Currency = to.Currency,
                FromUser = sender.Name,
                ToUser = to.Owner.Name,
                FromAccount = from.AccountNumber,
                ToAccount = to.AccountNumber
            };

            from.Balance -= amount;
            to.Balance += convertedAmount;

            from.Transactions.Add(withdrawTx);
            to.Transactions.Add(depositTx);

            Console.WriteLine($"\nSuccessfully transferred {amount:N2} {from.Currency} from {from.AccountNumber} to {to.Owner.Name} ({to.AccountNumber}).");
            Console.WriteLine($"Your new balance: {from.Balance:N2} {from.Currency}.");
            return true;
        }

        /// <summary>
        /// Checks if an account has enough balance or overdraft coverage.
        /// </summary>
        private bool HasCoverage(Account from, decimal amount)
        {
            if (from is CheckingAccount ca)
                return (from.Balance - amount) >= -ca.OverdraftLimit;
            else
                return (from.Balance - amount) >= 0;
        }
    }
}
