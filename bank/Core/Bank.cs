using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    public class Bank
    {
        public List<User> Users { get; } = new();
        public List<Account> Accounts { get; } = new();
        public decimal DefaultSavingsInterestRate { get; private set; } = 3.0m;


        public Bank() { }

        // Find account by number
        public Account? FindAccount(string accountNumber)
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        // Create account with currency selection
        public Account OpenAccount(User user, string accountType)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            // build account number as <userIndex>-<perUserIndex> -> 01-01, 01-02, 02-01
            int userIndex = Users.FindIndex(u => u.Id == user.Id) + 1;          // 01, 02, 03...
            int perUserIndex = user.Accounts.Count + 1;                          // 01, 02, ...
            string accountNumber = $"{userIndex:D2}-{perUserIndex:D2}";

            // Hämta valutorna dynamiskt från exchangeRates.json
            var exchangerateService = new bank.Services.ExchangerateService();
            var allRates = exchangerateService.getAllRates().ToList();

            // Bygg en lista av alla valutor (både enum och custom)
            var currencies = allRates.Select(r =>
                string.IsNullOrWhiteSpace(r.CustomCode)
                    ? r.Code.ToString()
                    : r.CustomCode
            ).Distinct().ToList();

            if (!currencies.Any())
            {
                Console.WriteLine("No available currencies found. Please add exchange rates first.");
                return null!;
            }

            Console.WriteLine("\nSelect currency:");
            for (int i = 0; i < currencies.Count; i++)
                Console.WriteLine($"{i + 1}. {currencies[i]}");

            Console.Write("Choice: ");
            var choice = int.TryParse(Console.ReadLine(), out int c) && c >= 1 && c <= currencies.Count
                ? currencies[c - 1]
                : "SEK";


            Account account;

            switch (accountType.ToLower().Trim())
            {
                case "savings":
                    account = new SavingsAccount(accountNumber, user, accountType, choice);
                    break;
                case "checking":
                    account = new CheckingAccount(accountNumber, user, accountType, choice);
                    break;
                default:
                    account = new Account(accountNumber, user, accountType, choice);
                    break;
            }

            Accounts.Add(account);
            user.Accounts.Add(account);

            Console.WriteLine($"\nNew {accountType} account created: {accountNumber} ({choice})");
            return account;
        }

        // Create account with default SEK
        public Account OpenAccount(User user, string accountNumber, string accountType)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            Account account;

            switch (accountType.ToLower().Trim())
            {
                case "savings":
                    account = new SavingsAccount(accountNumber.Trim(), user, accountType, "SEK");
                    break;
                case "checking":
                    account = new CheckingAccount(accountNumber.Trim(), user, accountType, "SEK");
                    break;
                default:
                    account = new Account(accountNumber.Trim(), user, accountType, "SEK");
                    break;
            }

            Accounts.Add(account);
            user.Accounts.Add(account);

            Console.WriteLine($"Bank: {accountType}-account {account.AccountNumber} created for user {user.Id}");
            return account;
        }

        // Transfer between accounts
        public bool Transfer(User user, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (user == null)
            {
                Console.WriteLine("Transfer failed: Invalid user.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(fromAccountNumber) || string.IsNullOrWhiteSpace(toAccountNumber))
            {
                Console.WriteLine("Transfer failed: Missing account numbers.");
                return false;
            }
            if (fromAccountNumber == toAccountNumber)
            {
                Console.WriteLine("Transfer failed: Cannot transfer to the same account.");
                return false;
            }
            if (amount <= 0)
            {
                Console.WriteLine("Transfer failed: Amount must be greater than zero.");
                return false;
            }

            var from = FindAccount(fromAccountNumber);
            var to = FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                Console.WriteLine("Transfer failed: One or both accounts not found.");
                return false;
            }

            if (from.Owner != user || to.Owner != user)
            {
                Console.WriteLine("Transfer failed: You can only transfer between your own accounts.");
                return false;
            }

            bool hasCoverage;
            if (from is CheckingAccount ca)
                hasCoverage = (from.Balance - amount) >= -ca.OverdraftLimit;
            else
                hasCoverage = amount <= from.Balance;

            if (!hasCoverage)
            {
                Console.WriteLine("Transfer failed: Insufficient funds including overdraft limit.");
                return false;
            }

            var before = from.Balance;
            from.Withdraw(amount);
            var withdrew = (from.Balance == before - amount);

            if (!withdrew)
            {
                Console.WriteLine("Transfer failed: Withdrawal failed.");
                return false;
            }

            to.Deposit(amount);
            Console.WriteLine($"Transfer succeeded: {amount} transferred from {fromAccountNumber} to {toAccountNumber}.");
            return true;
        }

        // Register new user
        public void RegisterUser(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);
        }

        // Find user by ID
        public User? FindUser(string userId) => Users.FirstOrDefault(u => u.Id == userId);


        public void UpdateDefaultSavingsRate(decimal newRate)
        {
            if (newRate > 0 && newRate < 10)
                DefaultSavingsInterestRate = newRate;
            else
                Console.WriteLine("Interest rate must be between 0 and 10%");
        }
    }
}
