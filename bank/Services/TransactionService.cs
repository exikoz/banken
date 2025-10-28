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

            Console.WriteLine($"{"Date and Time",-22} | {"Type",-10} | {"Amount",15} | {"Account",-10} | {"Account Type",-12}");
            Console.WriteLine(new string('-', 80)); // gör linjen lika lång som kolumnerna

            foreach (var t in allTransactions)
            {
                var account = currentUser.Accounts
                    .FirstOrDefault(a => a.AccountNumber.Trim().Equals(t.AccountNumber.Trim(), StringComparison.OrdinalIgnoreCase));

                string accountType = account is SavingsAccount ? "Savings" :
                                     account is CheckingAccount ? "Checking" : "Other";

                // Justerade kolumnbredder för perfekt alignment
                Console.WriteLine($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | {t.Type,-10} | {t.Amount,15:C} | {t.AccountNumber,-10} | {accountType,-12}");
            }

            Console.WriteLine(new string('-', 80)); // avslutande linje
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();


        }

    }
}
