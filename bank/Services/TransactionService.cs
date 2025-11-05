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

            // Get all transactions for all accounts owned by the user
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
                ConsoleHelper.WriteWarning("No transactions found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Header
            Console.WriteLine(
                $"{"Date",-22} | {"Type",-10} | {"Amount",-12} | {"Currency",-8} | {"From",-25} | {"To",-25} | {"Status",-15} | {"ID",-10}"
            );
            Console.WriteLine(new string('-', 150));





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

                // From formatting
                string fromDisplay =
                    t.FromAccount == "0000" ? "ATM" :
                    $"{t.FromUser} ({t.FromAccount})";

                // To formatting
                string toDisplay =
                    t.ToAccount == "0000" ? "ATM" :
                    $"{t.ToUser} ({t.ToAccount})";

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
