using bank.Core;
using bank.Utils;
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

            List<Transaction> transactions = new();


            if (!ValidationHelper.IsValid(allTransactions))
            {
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return;
            }


            ConsoleHelper.WriteMenuOption("1","Show processed transactions");
            ConsoleHelper.WriteMenuOption("2","Show completed transactions");
            ConsoleHelper.WriteMenuOption("3","Show all transactions");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    currentUser = authService.ShowLoginUI();
                    break;
                case "2":
                    authService.ShowRegistrationUI();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.WriteError("Invalid choice.");
                    Console.ReadKey();
                    break;
            }


            Console.WriteLine($"{"Date sent",-22} | {"Type",-10} | {"Amount",-12} | {"Currency",-8} | {"From",-25} | {"To",-25} | {"Processed",-25}");
            Console.WriteLine(new string('-', 120));

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

                if(!t.IsProcessed)
                {
                    ConsoleHelper.WriteHighlight($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | {t.Type,-10} | {t.Amount,-12:F2} | {t.Currency,-8} | {fromDisplay,-25} | {toDisplay,-25} | Status: Processing: {t.ProcessDuration}");
                }
                else
                {
                    ConsoleHelper.WriteSuccess($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | {t.Type,-10} | {t.Amount,-12:F2} | {t.Currency,-8} | {fromDisplay,-25} | {toDisplay,-25} | Status: Complete: {t.ProcessDuration}");
                }

            }

            Console.WriteLine(new string('-', 120));
            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey();
        }


        public List<Transaction> GetAllTransactionsOrdered(User currentUser)
        {
            // Hämta alla transaktioner från användarens konton
                List<Transaction> t = currentUser.Accounts
                .SelectMany(a => a.Transactions
                    .Select(t => new { Account = a, Transaction = t }))
                .OrderByDescending(x => x.Transaction.TimeStamp)
                .ToList();

            return t;
        }


    }
}
