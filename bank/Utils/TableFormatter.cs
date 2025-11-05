using bank.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bank.Utils
{
    /// <summary>
    /// Handles all table formatting and display logic
    /// Makes it easy to display data in a clean table format
    /// </summary>
    public static class TableFormatter
    {
        /// <summary>
        /// Display users in a simple table
        /// </summary>
        public static void DisplayUsersTable(List<User> users, string title = "USERS")
        {
            if (!users.Any())
            {
                Console.WriteLine("No users to display.");
                return;
            }

            // Print title
            Console.WriteLine();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();

            // Print header
            Console.WriteLine($"{"User ID",-15} {"Name",-25} {"Role",-15} {"# Accounts",-15} {"Status",-15}");
            Console.WriteLine(new string('-', 85));

            // Print each user
            foreach (var user in users)
            {
                string status = user.isBlocked ? "BLOCKED" : "Active";

                // Change color for blocked users (red) or admins (cyan)
                if (user.isBlocked)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (user.IsAdmin())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.Write($"{user.Id,-15} ");
                Console.Write($"{user.Name,-25} ");
                Console.Write($"{user.Role,-15} ");
                Console.Write($"{user.Accounts.Count,-15} ");

                // Color the status column
                if (user.isBlocked)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write($"{status,-15}");
                Console.ResetColor();
                Console.WriteLine();
            }

            // Print footer
            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"Total users: {users.Count}");

            // Show count of blocked users if any
            int blockedCount = users.Count(u => u.isBlocked);
            if (blockedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Blocked users: {blockedCount}");
                Console.ResetColor();
            }
        }



        public static void DisplayRatesTable(IEnumerable<ExchangeRate> rates, string title = "")
        {
            if (!rates.Any())
            {
                Console.WriteLine("No rates to display.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();

            Console.WriteLine(new string('-', 80));
            Console.ForegroundColor = ConsoleColor.Green;

            // Visa rätt valutakod: antingen CustomCode (om finns) eller Enum Code
            foreach (var rate in rates)
            {
                string displayCode = string.IsNullOrWhiteSpace(rate.CustomCode)
                    ? rate.Code.ToString()
                    : rate.CustomCode;

                Console.WriteLine($"Code: {displayCode,-5}  Rate: {rate.Rate,-10}  Last Updated: {rate.LastUpdated}");
            }

            Console.ResetColor();

        }





        /// <summary>
        /// Display accounts in a simple table
        /// </summary>
        public static void DisplayAccountsTable(List<Account> accounts, string title = "SEARCH RESULTS")
        {
            if (!accounts.Any())
            {
                Console.WriteLine("No accounts to display.");
                return;
            }

            // Print title
            Console.WriteLine();
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();

            // Print header
            Console.WriteLine($"{"Account #",-15} {"Owner ID",-12} {"Owner Name",-20} {"Balance",-15} {"Currency",-10} {"Type",-18}");
            Console.WriteLine(new string('-', 90));

            var exchangerateService = new bank.Services.ExchangerateService();

            // Print each account
            foreach (var account in accounts)
            {
                string accountType = account.GetType().Name;

                if (account.Balance > 5000)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (account.Balance > 1000)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(
                    $"{account.AccountNumber,-15} " +
                    $"{account.Owner.Id,-12} " +
                    $"{account.Owner.Name,-20} " +
                    $"{account.Balance,10:N2} " +
                    $"{account.Currency,-10} " +
                    $"{accountType,-18}"
                );

                Console.ResetColor();
            }

            // Calculate total balance in SEK using conversion
            decimal totalBalanceSek = 0;
            foreach (var acc in accounts)
            {
                totalBalanceSek += exchangerateService.ConvertToSek(acc.Currency, acc.Balance);
            }

            // Footer
            Console.WriteLine(new string('-', 90));
            Console.WriteLine($"Total accounts: {accounts.Count}");
            Console.WriteLine($"Total balance (converted to SEK): {totalBalanceSek:N2} kr");
        }


        /// <summary>
        /// Pause and wait for user to press a key
        /// </summary>
        public static void PauseForUser()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}