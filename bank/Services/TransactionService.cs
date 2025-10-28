using bank.Core;
using System;
using System.Collections.Generic;
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

            if (currentUser == null || !currentUser.Accounts.Any())
            {
                Console.WriteLine("No accounts found for this user.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            var allTransactions = currentUser.Accounts
                .SelectMany(a => a.Transactions)
                .OrderByDescending(t => t.TimeStamp)
                .ToList();

            if (!allTransactions.Any())
            {
                Console.WriteLine("No transactions found for this user.");
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Date and Time           | Type        | Amount     | Account");
            Console.WriteLine("--------------------------------------------------------------");

            foreach (var t in allTransactions)
            {
                Console.WriteLine($"{t.TimeStamp,-23:G} | {t.Type,-10} | {t.Amount,10:C} | {t.AccountNumber}");
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }
}
