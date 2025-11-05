using bank.Core;
using bank.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Principal;

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


            // Hämta alla transaktioner från användarens konton
            var allTransactions = currentUser.Accounts
                .SelectMany(a => a.Transactions
                    .Select(t => new { Account = a, Transaction = t }))
                .OrderByDescending(x => x.Transaction.TimeStamp)
                .ToList();

            var processingTransactions = allTransactions.Where(x => !x.Transaction.IsProcessed).ToList();
            var completedTransactions = allTransactions.Where(x => x.Transaction.IsProcessed).ToList();
            var deposits = allTransactions.Where(x => x.Transaction.Type == "Deposit");
            var withdrawals = allTransactions.Where(x => x.Transaction.Type == "Withdraw");



            IEnumerable<dynamic> TransactionToShow = allTransactions;

            var filters = new List<(string Name, IEnumerable<dynamic> Data)>
            {
                ("All", allTransactions),
                ("Processing", processingTransactions),
                ("Completed", completedTransactions),
                ("Withdrawals", withdrawals),
                ("Deposits", deposits),

            };

            if (!ValidationHelper.IsValid(allTransactions))
            {
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                return;
            }






            int x = 1;

            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("=== TRANSACTION LOG ===\n");
                Console.WriteLine("(<-) Press left/right arrows to sort by columns (->)");
                Console.WriteLine(new string('-', Console.WindowWidth -1));
                Console.WriteLine($"{"Date sent",-22} | {"Type",-10} | {"Amount",-12} | {"Currency",-8} | {"From",-25} | {"To",-15} | {"Status",-15}");
                Console.WriteLine(new string('-', Console.WindowWidth -1));

                Console.WriteLine($"Currently viewing: {filters[x].Name}");


                foreach (var item in filters[x].Data)
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

                    if (!t.IsProcessed)
                    {
                        ConsoleHelper.WriteHighlight(
                            $"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | {t.Type,-10} | {t.Amount,-12:F2} | {t.Currency,-8} | " +
                            $"{fromDisplay,-25} | {toDisplay,-25} | Processing: {t.ProcessDuration}"
                        );
                        Console.WriteLine();
                    }
                    else
                    {
                        ConsoleHelper.WriteSuccess($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} \n | {t.Type,-10} | {t.Amount,-12:F2} | {t.Currency,-8} | {fromDisplay,-25} | {toDisplay,-25} | Completed: {t.ProcessDuration}");
                    }

                }
                Console.WriteLine(new string('-', 120));



                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    x = (x - 1 + filters.Count) % filters.Count;
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    x = (x + 1) % filters.Count;
                }
            }
            while (key.Key != ConsoleKey.Escape);

        }

    }
}
