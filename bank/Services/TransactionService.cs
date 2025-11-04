using bank.Core;
using System;
using System.Linq;

namespace bank.Services
{
    public class TransactionService
    {
        private readonly Bank bank;

        public TransactionService(Bank bank)
        {
            this.bank = bank;
        }

        public void ShowTransactionLog(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSACTION LOG ===\n");

            // Hämta alla transaktioner från användarens konton
            var allTransactions = currentUser.Accounts
                .SelectMany(a => a.Transactions
                    .Select(t => new { Account = a, Transaction = t }))
                .OrderByDescending(x => x.Transaction.TimeStamp)
                .ToList();

            if (!allTransactions.Any())
            {
                Console.WriteLine("No transactions found.");
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"Date",-22} | {"Type",-10} | {"Amount",-12} | {"Currency",-8} | {"From",-25} | {"To",-25}");
            Console.WriteLine(new string('-', 110));

            foreach (var item in allTransactions)
            {
                var t = item.Transaction;

                // Om FromUser inte är satt, visa "Internal"
                string fromDisplay = !string.IsNullOrWhiteSpace(t.FromUser)
                    ? $"{t.FromUser} ({t.FromAccount})"
                    : "Internal";

                // Om ToUser inte är satt, visa "Internal"
                string toDisplay = !string.IsNullOrWhiteSpace(t.ToUser)
                    ? $"{t.ToUser} ({t.ToAccount})"
                    : "Internal";

                Console.WriteLine($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | {t.Type,-10} | {t.Amount,-12:F2} | {t.Currency,-8} | {fromDisplay,-25} | {toDisplay,-25}");
            }

            Console.WriteLine(new string('-', 110));
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }
}
