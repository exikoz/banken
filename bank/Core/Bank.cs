using System;
using System.Collections.Generic;
using System.Linq;
using bank.Utils;

namespace bank.Core
{
    // 8-char A–Z0–9 transaction IDs
    public static class TransactionIdGenerator
    {
        private static readonly Random _rng = new Random();
        private static readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        public static string Generate()
        {
            var buff = new char[8];
            for (int i = 0; i < 8; i++)
                buff[i] = _alphabet[_rng.Next(_alphabet.Length)];
            return new string(buff);
        }
    }

    public class Bank
    {
        public List<User> Users { get; } = new();
        public List<Account> Accounts { get; } = new();
        public decimal DefaultSavingsInterestRate { get; private set; } = 3.0m;

        public Bank() { }

        public Account? FindAccount(string accountNumber)
            => Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

        public Account OpenAccount(User user, string accountType)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            int userIndex = Users.FindIndex(u => u.Id == user.Id) + 1;
            int perUserIndex = user.Accounts.Count + 1;
            string accountNumber = $"{userIndex:D2}-{perUserIndex:D2}";

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
                Console.WriteLine($"{i + 1}. {currencies[i]}");

            var input = ConsoleHelper.Prompt("Choice");
            var choice = int.TryParse(input, out int c) && c >= 1 && c <= currencies.Count
                ? currencies[c - 1]
                : "SEK";

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

        public bool Transfer(User user, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (user == null) { ConsoleHelper.WriteError("Invalid user"); return false; }
            if (string.IsNullOrWhiteSpace(fromAccountNumber) || string.IsNullOrWhiteSpace(toAccountNumber)) { ConsoleHelper.WriteError("Missing account numbers"); return false; }
            if (fromAccountNumber == toAccountNumber) { ConsoleHelper.WriteError("Same account"); return false; }
            if (amount <= 0) { ConsoleHelper.WriteError("Invalid amount"); return false; }

            var from = FindAccount(fromAccountNumber);
            var to = FindAccount(toAccountNumber);
            if (from == null || to == null) { ConsoleHelper.WriteError("Account not found"); return false; }
            if (from.Owner != user || to.Owner != user) { ConsoleHelper.WriteError("Transfers only allowed between your own accounts"); return false; }

            bool ok = amount <= from.Balance;
            if (!ok) { ConsoleHelper.WriteError("Insufficient funds"); return false; }

            decimal before = from.Balance;
            from.Withdraw(amount);
            if (from.Balance != before - amount) { ConsoleHelper.WriteError("Withdraw failed"); return false; }

            to.Deposit(amount);
            ConsoleHelper.WriteSuccess($"Transferred {amount} from {fromAccountNumber} to {toAccountNumber}");
            return true;
        }

        public void RegisterUser(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);
        }

        public User? FindUser(string userId)
            => Users.FirstOrDefault(u => u.Id == userId);

        public void UpdateDefaultSavingsRate(decimal newRate)
        {
            if (newRate > 0 && newRate < 10) DefaultSavingsInterestRate = newRate;
            else ConsoleHelper.WriteError("Rate must be 0–10%");
        }

        public void ProcessPendingTransfers()
        {
            foreach (var account in Accounts)
            {
                var pending = account.Transactions
                    .Where(t => t.IsPending && t.ReleaseAt <= DateTime.UtcNow)
                    .ToList();

                foreach (var tx in pending)
                {
                    var receiver = FindAccount(tx.ToAccount);
                    if (receiver == null)
                        continue;

                    // 1. Add money to receiver balance
                    receiver.Balance += tx.Amount;

                    // 2. Mark original TX (sender’s pending) as completed
                    tx.IsPending = false;
                    tx.Status = "Completed";
                    tx.ReleaseAt = null;

                    // 3. Create matching deposit TX for receiver
                    var receiveTx = new Transaction(
                        id: tx.Id,                         // SAME ID
                        accountNumber: receiver.AccountNumber,
                        timeStamp: DateTime.UtcNow,
                        type: "Transfer",
                        amount: tx.Amount,
                        currency: receiver.Currency,
                        accountType: receiver.AccountType,
                        fromAccount: tx.FromAccount,
                        toAccount: tx.ToAccount,
                        fromUser: tx.FromUser,
                        toUser: tx.ToUser
                    )
                    {
                        IsPending = false,
                        Status = "Completed",
                        IsInternal = false                // External transfer
                    };

                    // 4. Add to receiver’s transaction list
                    receiver.Transactions.Add(receiveTx);
                }
            }
        }

        // Wrapper for generator
        public static string GenerateTransactionId() => TransactionIdGenerator.Generate();
    }
}
