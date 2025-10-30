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

            Console.WriteLine($"{"Date and Time",-22} | {"Type",-10} | {"Amount",-15} | {"Currency",-8} | {"Account",-10} | {"Account Type",-12}");
            Console.WriteLine(new string('-', 90));

            var allTransactions = currentUser.Accounts
                .SelectMany(a => a.Transactions.Select(t => new { Account = a, Transaction = t }))
                .OrderByDescending(x => x.Transaction.TimeStamp)
                .ToList();

            if (!allTransactions.Any())
            {
                Console.WriteLine("No transactions found.");
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return;
            }

            foreach (var x in allTransactions)
            {
                var a = x.Account;
                var t = x.Transaction;

                string accountType = a is SavingsAccount ? "Savings" :
                                     a is CheckingAccount ? "Checking" : "Other";

                Console.WriteLine($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | {t.Type,-10} | {t.Amount,-15} | {a.Currency,-8} | {a.AccountNumber,-10} | {accountType,-12}");
            }

            Console.WriteLine(new string('-', 90));
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }
    }
}
