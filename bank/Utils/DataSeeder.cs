using bank.Core;
using bank.Utils;
using System;

namespace bank.Utils
{
    public static class DataSeeder
    {
        // Seed basic test data
        public static void SeedTestData(Bank bank)
        {
            var user1 = new User("19910101-0101", "Alexander", "1234", UserRole.Customer);
            var user2 = new User("19920202-0202", "Maria", "5678", UserRole.Customer);
            var user3 = new User("19930303-0303", "Erik", "9999", UserRole.Customer);
            var user4 = new User("19940404-0404", "Lisa", "1111", UserRole.Customer);
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);

            bank.RegisterUser(user1);
            bank.RegisterUser(user2);
            bank.RegisterUser(user3);
            bank.RegisterUser(user4);
            bank.RegisterUser(admin);

            // Create only Alexander's two accounts
            var acc1 = bank.OpenAccount(user1, "01-01", "checking");
            var acc2 = bank.OpenAccount(user1, "01-02", "savings");

            SeedTransactions(acc1, acc2);

            ConsoleHelper.WriteSuccess("Test data seeded");
        }

        // Add transactions to seeded accounts
        private static void SeedTransactions(params Account[] accounts)
        {
            accounts[0].Deposit(5000);
            accounts[0].Withdraw(1500);
            accounts[0].Deposit(2000);
            accounts[0].Withdraw(500);

            accounts[1].Deposit(10000);
            accounts[1].Withdraw(2000);
        }

        // Minimal dataset (only admin)
        public static void SeedMinimalData(Bank bank)
        {
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);
            bank.RegisterUser(admin);

            ConsoleHelper.WriteSuccess("Minimal data seeded");
        }

        // Large dataset for stress testing
        public static void SeedLargeDataset(Bank bank)
        {
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);
            bank.RegisterUser(admin);

            for (int i = 1; i <= 50; i++)
            {
                var user = new User($"U{i:D3}", $"Test User {i}", "1234", UserRole.Customer);
                bank.RegisterUser(user);

                int accountCount = (i % 3) + 1;

                for (int j = 1; j <= accountCount; j++)
                {
                    var type = (j % 2 == 0) ? "savings" : "checking";
                    var account = bank.OpenAccount(user, $"ACC{i:D3}-{j}", type);

                    if (account != null)
                    {
                        account.Deposit(1000 * i);
                        if (i % 2 == 0)
                            account.Withdraw(500);
                    }
                }
            }

            ConsoleHelper.WriteSuccess("Large dataset seeded");
        }
    }
}
