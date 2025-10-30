using bank.Core;
using System;

namespace bank.Utils
{
    /// <summary>
    /// Seeds the bank with test data for development/testing
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds the bank with test users and accounts
        /// </summary>
        public static void SeedTestData(Bank bank)
        {
            // Create test users
            var user1 = new User("U001", "Alexander", "1234", UserRole.Customer);
            var user2 = new User("U002", "Maria", "5678", UserRole.Customer);
            var user3 = new User("U003", "Erik", "9999", UserRole.Customer);
            var user4 = new User("U004", "Lisa", "1111", UserRole.Customer);
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);

            // Register users
            bank.RegisterUser(user1);
            bank.RegisterUser(user2);
            bank.RegisterUser(user3);
            bank.RegisterUser(user4);
            bank.RegisterUser(admin);

            // Create accounts for Alexander (U001)
            var acc1 = bank.OpenAccount(user1, "ACC001", "checking");
            var acc2 = bank.OpenAccount(user1, "ACC002", "savings"); // Alexander has 2 accounts

            var acc3 = bank.OpenAccount(user2, "ACC003", "checking");

            var acc4 = bank.OpenAccount(user3, "ACC004", "checking");
            var acc5 = bank.OpenAccount(user3, "ACC005", "savings");

            var acc6 = bank.OpenAccount(user4, "ACC006", "checking");


            // Seed some initial balances and transactions
            SeedTransactions(acc1, acc2, acc3, acc4, acc5, acc6);

            //Console.WriteLine("✓ Test data seeded successfully!");
        }

        /// <summary>
        /// Seeds accounts with initial transactions to create test data
        /// </summary>
        private static void SeedTransactions(params Account[] accounts)
        {
            // Alexander's first account - some activity
            accounts[0].Deposit(5000);
            accounts[0].Withdraw(1500);
            accounts[0].Deposit(2000);
            accounts[0].Withdraw(500);

            // Alexander's second account - high balance
            accounts[1].Deposit(10000);
            accounts[1].Withdraw(2000);

            // Maria's account - moderate activity
            accounts[2].Deposit(3000);
            accounts[2].Withdraw(500);
            accounts[2].Deposit(1000);

            // Erik's first account - low balance
            accounts[3].Deposit(500);
            accounts[3].Withdraw(100);

            // Erik's second account - many transactions
            accounts[4].Deposit(1000);
            accounts[4].Withdraw(200);
            accounts[4].Deposit(500);
            accounts[4].Withdraw(300);
            accounts[4].Deposit(800);
            accounts[4].Withdraw(150);

            // Lisa's account - new user, just deposited
            accounts[5].Deposit(2500);
        }

        /// <summary>
        /// Seeds minimal data - useful for production-like testing
        /// </summary>
        public static void SeedMinimalData(Bank bank)
        {
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);
            bank.RegisterUser(admin);

            Console.WriteLine("✓ Minimal data seeded (admin only).");
        }

        /// <summary>
        /// Seeds a large dataset - useful for performance testing
        /// </summary>
        public static void SeedLargeDataset(Bank bank)
        {
            // Create admin
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);
            bank.RegisterUser(admin);

            // Create 50 test users with accounts
            for (int i = 1; i <= 50; i++)
            {
                var user = new User($"U{i:D3}", $"Test User {i}", "1234", UserRole.Customer);
                bank.RegisterUser(user);

                // Each user gets 1-3 accounts
                int accountCount = (i % 3) + 1;
                for (int j = 1; j <= accountCount; j++)
                {
                    var accountType = (j % 2 == 0) ? "savings" : "checking";
                    var account = bank.OpenAccount(user, $"ACC{i:D3}-{j}", accountType);


                    // Add some random transactions
                    if (account != null)
                    {
                        account.Deposit(1000 * i);
                        if (i % 2 == 0) account.Withdraw(500);
                    }
                }
            }

            Console.WriteLine($"✓ Large dataset seeded (50 users, ~100 accounts).");
        }
    }
}