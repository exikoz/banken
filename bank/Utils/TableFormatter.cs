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
            Console.WriteLine($"{"User ID",-15} {"Name",-25} {"Role",-15} {"# Accounts",-15}");
            Console.WriteLine(new string('-', 70)); // Simple line separator

            // Print each user
            foreach (var user in users)
            {
                // Change color for admins
                if (user.IsAdmin())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.WriteLine(
                    $"{user.Id,-15} " +
                    $"{user.Name,-25} " +
                    $"{user.Role,-15} " +
                    $"{user.Accounts.Count,-15}"
                );

                Console.ResetColor();
            }

            // Print footer
            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"Total users: {users.Count}");
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
            Console.WriteLine($"{"Account #",-15} {"Owner ID",-12} {"Owner Name",-20} {"Balance",-15} {"Type",-18}");
            Console.WriteLine(new string('-', 80)); // Simple line separator

            // Print each account
            foreach (var account in accounts)
            {
                string accountType = account.GetType().Name;

                // Change color based on balance
                if (account.Balance > 5000)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (account.Balance > 1000)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine(
                    $"{account.AccountNumber,-15} " +
                    $"{account.Owner.Id,-12} " +
                    $"{account.Owner.Name,-20} " +
                    $"{account.Balance,-15:C} " +
                    $"{accountType,-18}"
                );

                Console.ResetColor();
            }

            // Print footer
            Console.WriteLine(new string('-', 80));
            decimal totalBalance = accounts.Sum(a => a.Balance);
            Console.WriteLine($"Total accounts: {accounts.Count}");
            Console.WriteLine($"Total balance: {totalBalance:C}");
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