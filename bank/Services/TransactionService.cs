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
            ConsoleHelper.ClearScreen();
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

            if (!allTransactions.Any())
            {
                ConsoleHelper.WriteWarning("No transactions found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            ConsoleHelper.WriteInfo($"Showing {allTransactions.Count} transaction(s)");
            Console.WriteLine();

            // Header with color
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                $"{"Date",-22} | {"Type",-10} | {"Amount",-12} | {"Curr",-5} | {"From",-25} | {"To",-25} | {"Status",-20} | {"ID",-10}"
            );
            Console.ResetColor();
            ConsoleHelper.WriteSeparator('=', 150);

            foreach (var item in allTransactions)
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

                // Status formatting and color
                string statusText = t.Status;
                ConsoleColor statusColor = ConsoleColor.White;

                if (t.Status == "Pending" && t.ReleaseAt.HasValue)
                {
                    int minutesLeft = (int)(t.ReleaseAt.Value - DateTime.UtcNow).TotalMinutes;
                    if (minutesLeft < 0) minutesLeft = 0;

                    statusText = $"Pending ({minutesLeft} min)";
                    statusColor = ConsoleColor.Yellow;
                }
                else if (t.Status == "Completed")
                {
                    statusText = "Completed";
                    statusColor = ConsoleColor.Green;
                }
                else if (t.Status == "Failed")
                {
                    statusText = "Failed";
                    statusColor = ConsoleColor.Red;
                }

                // Date
                Console.Write($"{t.TimeStamp:yyyy-MM-dd HH:mm:ss} | ");

                // Type
                Console.Write($"{t.Type,-10} | ");

                // Amount
                Console.Write($"{t.Amount,-12:N2} | ");

                // Currency
                Console.Write($"{t.Currency,-5} | ");

                // From
                Console.Write($"{fromDisplay,-25} | ");

                // To
                Console.Write($"{toDisplay,-25} | ");

                // Status with color for entire column
                Console.ForegroundColor = statusColor;
                Console.Write($"{statusText,-20} | ");
                Console.ResetColor();

                // Transaction ID
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"{t.Id,-10}");
                Console.ResetColor();
                Console.WriteLine();
            }

            ConsoleHelper.WriteSeparator('=', 150);

            ConsoleHelper.PauseWithMessage();
        }
    }
}