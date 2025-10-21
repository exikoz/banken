using bank.Core;
using System;

namespace bank.Services
{
    /// <summary>
    /// Admin service - handles admin-specific operations
    /// TODO: should implement the actual admin features here
    /// </summary>
    public class AdminService
    {
        private readonly Bank bank;

        public AdminService(Bank bank)
        {
            this.bank = bank;
        }

        /// <summary>
        /// Main admin dashboard
        /// TODO:  add admin features here
        /// </summary>
        public void ShowAdminDashboard(User admin)
        {
            if (!admin.IsAdmin())
            {
                Console.WriteLine("Access Denied: Admin privileges required.");
                Console.ReadKey();
                return;
            }

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN DASHBOARD ===");
                Console.WriteLine($"Logged in as: {admin.Name} (Admin)\n");

                // TODO: Teammates implement these features
                Console.WriteLine("Available admin features:");
                Console.WriteLine("- View all users");
                Console.WriteLine("- View all accounts");
                Console.WriteLine("- Create account for users");
                Console.WriteLine("- View largest deposit/withdrawal");
                Console.WriteLine("- Find user with most transactions");
                Console.WriteLine("- Add timestamp and username to search outputs");
                Console.WriteLine();
                Console.WriteLine("[Features not yet implemented - to be built by teammates]");
                Console.WriteLine("\nPress any key to return to main menu...");
                Console.ReadKey();
                exit = true;
            }
        }

        // TODO:
        // public void ViewAllUsers() { }
        // public void ViewAllAccounts() { }
        // public void CreateAccountForUser() { }
    }
}