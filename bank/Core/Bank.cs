using System;
using System.Collections.Generic;
using System.Linq;
using bank.Utils;

namespace bank.Core
{
    public class Bank
    {
        public List<User> Users { get; } = new();
        public List<Account> Accounts { get; } = new();
        public decimal DefaultSavingsInterestRate { get; private set; } = 3.0m;

        public Bank() { }

        // Find an account by number
        public Account? FindAccount(string accountNumber)
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        // Create account with auto-generated number: 01-01, 01-02, 02-01...
        public Account OpenAccount(User user, string accountType)
        {
            // Ensure user exists
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            // Build account number
            int userIndex = Users.FindIndex(u => u.Id == user.Id) + 1;
            int perUserIndex = user.Accounts.Count + 1;
            string accountNumber = $"{userIndex:D2}-{perUserIndex:D2}";

            // Load currencies
            var exchangeService = new bank.Services.ExchangerateService();
            var currencies = exchangeService.getAllRates()
                .Select(r => string.IsNullOrWhiteSpace(r.CustomCode) ? r.Code.ToString() : r.CustomCode)
                .Distinct()
                .ToList();

            if (!currencies.Any())
            {
                ConsoleHelper.WriteError("No currencies available");
                return null!;
            }

            ConsoleHelper.WriteInfo("Select currency:");
            for (int i = 0; i < currencies.Count; i++)
                ConsoleHelper.WriteInfo($"{i + 1}. {currencies[i]}");

            var input = ConsoleHelper.Prompt("Choice");
            var choice = int.TryParse(input, out int c) && c >= 1 && c <= currencies.Count
                ? currencies[c - 1]
                : "SEK";

            // Create account
            Account account = accountType.ToLower() switch
            {
                "savings" => new SavingsAccount(accountNumber, user, accountType, choice),
                "checking" => new CheckingAccount(accountNumber, user, accountType, choice),
                _ => new Account(accountNumber, user, accountType, choice)
            };

            Accounts.Add(account);
            user.Accounts.Add(account);

            ConsoleHelper.WriteSuccess($"Created {accountType} account {accountNumber} ({choice})");
            return account;
        }

        // Manual creation (used by seeder)
        public Account OpenAccount(User user, string accountNumber, string accountType)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            Account account = accountType.ToLower() switch
            {
                "savings" => new SavingsAccount(accountNumber, user, accountType, "SEK"),
                "checking" => new CheckingAccount(accountNumber, user, accountType, "SEK"),
                _ => new Account(accountNumber, user, accountType, "SEK")
            };

            Accounts.Add(account);
            user.Accounts.Add(account);

            ConsoleHelper.WriteInfo($"Seed: Created {accountType} account {accountNumber} for user {user.Id}");
            return account;
        }

        // Transfer between own accounts
        public bool Transfer(User user, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (user == null)
            {
                ConsoleHelper.WriteError("Invalid user");
                return false;
            }
            if (string.IsNullOrWhiteSpace(fromAccountNumber) || string.IsNullOrWhiteSpace(toAccountNumber))
            {
                ConsoleHelper.WriteError("Missing account numbers");
                return false;
            }
            if (fromAccountNumber == toAccountNumber)
            {
                ConsoleHelper.WriteError("Same account");
                return false;
            }
            if (amount <= 0)
            {
                ConsoleHelper.WriteError("Invalid amount");
                return false;
            }

            var from = FindAccount(fromAccountNumber);
            var to = FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                ConsoleHelper.WriteError("Account not found");
                return false;
            }

            if (from.Owner != user || to.Owner != user)
            {
                ConsoleHelper.WriteError("Transfers only allowed between your own accounts");
                return false;
            }

            bool ok = from is CheckingAccount ca
                ? (from.Balance - amount) >= -ca.OverdraftLimit
                : amount <= from.Balance;

            if (!ok)
            {
                ConsoleHelper.WriteError("Insufficient funds");
                return false;
            }

            decimal before = from.Balance;
            from.Withdraw(amount);

            if (from.Balance != before - amount)
            {
                ConsoleHelper.WriteError("Withdraw failed");
                return false;
            }

            to.Deposit(amount);
            ConsoleHelper.WriteSuccess($"Transferred {amount} from {fromAccountNumber} to {toAccountNumber}");
            return true;
        }

        // Register user
        public void RegisterUser(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);
        }

        // Find user by ID
        public User? FindUser(string userId)
        {
            return Users.FirstOrDefault(u => u.Id == userId);
        }

        // Update savings interest
        public void UpdateDefaultSavingsRate(decimal newRate)
        {
            if (newRate > 0 && newRate < 10)
                DefaultSavingsInterestRate = newRate;
            else
                ConsoleHelper.WriteError("Rate must be 0–10%");
        }
    }
}
