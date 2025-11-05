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
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSACTION LOG");

            // Run pending processing before showing
            bank.ProcessPendingTransfers();

            // Fetches all user transactions 
            var allTransactions = currentUser.Accounts
                .SelectMany(a => a.Transactions
                    .Select(t => new { Account = a, Transaction = t }))
                .Where(x =>
                    // Normal transactions (Deposit, Withdraw, External Transfer)
                    !x.Transaction.IsInternal ||

                    // For internal transfers, show ONLY the "withdraw" side
                    (x.Transaction.IsInternal &&
                     x.Transaction.FromAccount == x.Transaction.AccountNumber)
                )
                .OrderByDescending(x => x.Transaction.TimeStamp)
                .ToList();


            /* Afterwards I divide the list into smaller lists holding the filtered content. 
             */
            var processingTransactions = allTransactions.Where(x => !x.Transaction.IsProcessed).ToList();
            var completedTransactions = allTransactions.Where(x => x.Transaction.IsProcessed).ToList();
            var deposits = allTransactions.Where(x => x.Transaction.Type == "Deposit");
            var withdrawals = allTransactions.Where(x => x.Transaction.Type == "Withdraw");


            //Then I use this dynamic array to hold the lists. It holds all transactions by default
            IEnumerable<dynamic> TransactionToShow = allTransactions;


            /* I place all the transactions with their Title in one list, 
               This represents the possible navigations available. 
               We also need this for our "arrow" logic. 
            */
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
                ConsoleHelper.WriteWarning("No transactions found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Header
            Console.WriteLine(
                $"{"Date",-22} | {"Type",-10} | {"Amount",-12} | {"Currency",-8} | {"From",-25} | {"To",-25} | {"Status",-15} | {"ID",-10}"
            );
            Console.WriteLine(new string('-', 150));




            // represents the position in console, default first object in filters when init
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


                // The actual iteration and printing of the table, this is reprinted when list view is changed

                foreach (var item in filters[x].Data)
                {
                    var t = item.Transaction;

                    // If FromUser isn't defiened , show "Internal"
                    string fromDisplay = !string.IsNullOrWhiteSpace(t.FromUser)
                        ? $"{t.FromUser} ({t.FromAccount})"
                        : "Internal";

                    // If FromUser isn't defiened , show "Internal"
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


                /* here we read key input, and either go left(-) or right(+) 
                // I use modolus to stay within bounds. Our list is at this moment
                // at 5 indices (0,1,2,3,4), so if we try and head over to 5 then: 
                // (4+1) % 5 = 0
                */


                // Status formatting
                string status;
                if (t.IsPending && t.ReleaseAt.HasValue)
                {
                    int minutesLeft = (int)(t.ReleaseAt.Value - DateTime.UtcNow).TotalMinutes;
                    if (minutesLeft < 0) minutesLeft = 0;
                    status = $"Pending ({minutesLeft} min)";
                }
                else
                {
                    status = "Completed";
                }

                Console.WriteLine(
                    $"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | " +
                    $"{t.Type,-10} | " +
                    $"{t.Amount,-12:N2} | " +
                    $"{t.Currency,-8} | " +
                    $"{fromDisplay,-25} | " +
                    $"{toDisplay,-25} | " +
                    $"{status,-15} | " +
                    $"{t.Id,-10}"
                );
            }
            while (key.Key != ConsoleKey.Escape);

            Console.WriteLine(new string('-', 150));
            ConsoleHelper.PauseWithMessage();
        }

    }
}
