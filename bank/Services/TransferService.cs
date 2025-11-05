using System;
using System.Linq;
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

        // Pin validator with max 3 attempts
        private bool ValidatePin(User user)
        {
            for (int i = 0; i < 3; i++)
            {
                Console.Write("Enter PIN: ");
                var pin = Console.ReadLine();

                if (pin == user.PIN)
                    return true;

                ConsoleHelper.WriteWarning("Incorrect PIN.");
            }

            ConsoleHelper.WriteWarning("Too many attempts. Returning...");
            return false;
        }

        // Simple withdrawal rule (no overdraft)
        private bool ApplyWithdrawal(Account from, decimal amount, out decimal fee, out string type)
        {
            fee = 0;
            type = "Standard";
            return from.Balance >= amount;
        }

        // TRANSFER BETWEEN OWN ACC.
        public void DoTransferOwn(User currentUser)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER BETWEEN OWN ACCOUNTS");

            var accounts = currentUser.Accounts.ToList();
            if (!accounts.Any(a => a.Balance > 0))
            {
                ConsoleHelper.WriteWarning("No accounts with balance available.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Select source account
            Console.WriteLine("Select source account:\n");
            for (int i = 0; i < accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {accounts[i].AccountNumber} | {accounts[i].Balance:N2} {accounts[i].Currency}");

            Console.Write("\nChoose: ");
            if (!int.TryParse(Console.ReadLine(), out int srcChoice) || srcChoice < 1 || srcChoice > accounts.Count)
                return;

            var fromAcc = accounts[srcChoice - 1];

            // Select destination
            Console.WriteLine("\nSELECT DESTINATION ACCOUNT\n");

            var destList = accounts.Where(a => a != fromAcc).ToList();
            for (int i = 0; i < destList.Count; i++)
                Console.WriteLine($"{i + 1}. {destList[i].AccountNumber} | {destList[i].Balance:N2} {destList[i].Currency}");

            Console.Write("\nChoose: ");
            if (!int.TryParse(Console.ReadLine(), out int dstChoice) || dstChoice < 1 || dstChoice > destList.Count)
                return;

            var toAcc = destList[dstChoice - 1];

            // ENTER AMOUNT (separate clean screen)
            Console.Clear();
            ConsoleHelper.WriteHeader("ENTER AMOUNT");

            Console.Write($"Enter Amount ({fromAcc.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                return;

            // PIN screen
            Console.Clear();
            ConsoleHelper.WriteHeader("ENTER PIN TO CONFIRM TRANSFER");

            if (!ValidatePin(currentUser))
                return;

            // Check coverage
            if (!ApplyWithdrawal(fromAcc, amount, out decimal fee, out string tType))
            {
                ConsoleHelper.WriteError("Insufficient coverage.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Conversion
            decimal converted = amount;
            bool sameCurrency = fromAcc.Currency == toAcc.Currency;

            if (!sameCurrency)
                converted = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAcc.Currency);

            // Apply
            fromAcc.Balance -= amount + fee;
            toAcc.Balance += converted;

            // Result screen
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER COMPLETED");

            string display = sameCurrency
                ? $"{amount:N2} {fromAcc.Currency}"
                : $"{amount:N2} {fromAcc.Currency} → {converted:N2} {toAcc.Currency}";

            ConsoleHelper.WriteSuccess($"Transfer completed: {display}");
            ConsoleHelper.WriteSuccess($"Sender balance: {fromAcc.Balance:N2} {fromAcc.Currency}");
            ConsoleHelper.WriteSuccess($"Receiver balance: {toAcc.Balance:N2} {toAcc.Currency}");

            ConsoleHelper.PauseWithMessage();
        }

        // TRANSFER TO OTHER USER
        public void TransferToOther(User sender)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER TO ANOTHER CUSTOMER");

            var accounts = sender.Accounts.Where(a => a.Balance > 0).ToList();
            if (!accounts.Any())
            {
                ConsoleHelper.WriteWarning("No available accounts.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Select sender account
            for (int i = 0; i < accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {accounts[i].AccountNumber} | {accounts[i].Balance:N2} {accounts[i].Currency}");

            Console.Write("\nChoose account: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > accounts.Count)
                return;

            var fromAcc = accounts[choice - 1];

            // Recipient
            Console.WriteLine("\nRECIPIENT ACCOUNT\n");
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

            // Amount (no clear here)
            Console.WriteLine("\nENTER AMOUNT");
            Console.Write($"Enter Amount ({fromAcc.Currency}): ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                return;

            // PIN (clear here)
            Console.Clear();
            ConsoleHelper.WriteHeader("ENTER PIN TO CONFIRM TRANSFER");

            if (!ValidatePin(sender))
                return;

            if (fromAcc.Balance < amount)
            {
                ConsoleHelper.WriteError("Insufficient funds.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Conversion
            decimal converted = amount;
            bool sameCurrency = fromAcc.Currency == toAcc.Currency;

            if (!sameCurrency)
                converted = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAcc.Currency);

            // Apply
            fromAcc.Balance -= amount;
            toAcc.Balance += converted;

            // Result
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER COMPLETED");

            string display = sameCurrency
                ? $"{amount:N2} {fromAcc.Currency}"
                : $"{amount:N2} {fromAcc.Currency} → {converted:N2} {toAcc.Currency}";

            ConsoleHelper.WriteSuccess($"Transfer completed: {display}");
            ConsoleHelper.WriteSuccess($"Sent from: {fromAcc.AccountNumber}");
            ConsoleHelper.WriteSuccess($"Received by: {toAcc.Owner.Name} ({toAcc.AccountNumber})");
            ConsoleHelper.WriteSuccess($"Your new balance: {fromAcc.Balance:N2} {fromAcc.Currency}");

            ConsoleHelper.PauseWithMessage();
        }
    }
}
