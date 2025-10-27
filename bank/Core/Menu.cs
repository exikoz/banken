using bank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    /// <summary>
    /// Main menu orchestrator - handles overall application flow and delegates to specialized services
    /// </summary>
    public class Menu
    {
        private readonly Bank bank;
        private readonly AuthenticationService authService;
        private readonly AccountService accountService;
        private readonly AdminService adminService;
        private User? currentUser;

        public Menu()
        {
            bank = new Bank();
            authService = new AuthenticationService(bank);
            accountService = new AccountService(bank);
            adminService = new AdminService(bank);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // --- Create test users ---
            var user1 = new User("U001", "Alexander", "1234");
            var user2 = new User("U002", "Maria", "5678");
            var admin = new User("ADMIN", "Admin User", "0000", UserRole.Admin);

            // --- Register them in the bank system ---
            bank.RegisterUser(user1);
            bank.RegisterUser(user2);
            bank.RegisterUser(admin);

            // --- No accounts created automatically ---
            // Users can now create Savings or Checking accounts manually via menu
        }


        public void DrawUI()
        {
            Console.Title = "Malmo Royal Bank";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== WELCOME TO MALMO BANK SYSTEM ===\n");

                if (currentUser == null)
                {
                    ShowLoginMenu();
                }
                else
                {
                    ShowMainMenu();
                }
            }
        }

        /// <summary>
        /// Shows the menu for unauthenticated users (Login, Register, Exit)
        /// Delegates login and registration UI entirely to AuthenticationService.
        /// </summary>
        private void ShowLoginMenu()
        {
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Register New User");
            Console.WriteLine("3. Exit");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Calls the complete login flow from AuthenticationService
                    currentUser = authService.ShowLoginUI();
                    break;
                case "2":
                    // Calls the complete registration flow from AuthenticationService
                    authService.ShowRegistrationUI();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }

        /// <summary>
        /// Shows the main menu for authenticated users
        /// Delegates banking operations to AccountService.
        /// </summary>
        private void ShowMainMenu()
        {
            Console.WriteLine($"--- Welcome, {currentUser!.Name} ({currentUser.Id}) [{currentUser.Role}] ---");

            // NEW: Show different menu based on role
            if (currentUser.IsAdmin())
            {
                ShowAdminMenu();
            }
            else
            {
                ShowCustomerMenu();
            }
        }

        // NEW: Separate menu for customers
        private void ShowCustomerMenu()
        {
            Console.WriteLine("\n1. View My Accounts (Balance)");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Open New Account");
            Console.WriteLine("5. Calculate Interest");
            Console.WriteLine("6. Log Out");
            Console.WriteLine("7. Exit");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    accountService.ShowAccounts(currentUser!);
                    break;
                case "2":
                    accountService.Deposit(currentUser!);
                    break;
                case "3":
                    accountService.Withdraw(currentUser!);
                    break;
                case "4":
                    accountService.CreateAccount(currentUser!);
                    break;
                case "5":
                    CalculateInterestUI();
                    break;
                case "6":
                    currentUser = null;
                    Console.WriteLine("\nSuccessfully logged out. Press any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "7":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }

        // NEW: Separate menu for admins
        private void ShowAdminMenu()
        {
            Console.WriteLine("\n1. Admin Dashboard");
            Console.WriteLine("2. View My Accounts (Personal)");
            Console.WriteLine("3. Log Out");
            Console.WriteLine("4. Exit");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    adminService.ShowAdminDashboard(currentUser!);
                    break;
                case "2":
                    accountService.ShowAccounts(currentUser!);
                    break;
                case "3":
                    currentUser = null;
                    Console.WriteLine("\nSuccessfully logged out. Press any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }

        private void CalculateInterestUI()
        {
            if (currentUser == null)
            {
                Console.WriteLine("\nYou must be logged in to use this feature.\n");
                Console.ReadKey();
                return;
            }

            Console.Write("\nEnter your savings account number: ");
            string accountNumber = Console.ReadLine();

            var savings = bank.Accounts
                .OfType<SavingsAccount>()
                .FirstOrDefault(a => a.AccountNumber.Equals(accountNumber, StringComparison.OrdinalIgnoreCase)
                                  && a.Owner == currentUser);

            if (savings == null)
            {
                Console.WriteLine("\nNo savings account found for this user.\n");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter annual interest rate (%): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rate))
            {
                Console.WriteLine("\nInvalid input.\n");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter number of months: ");
            if (!int.TryParse(Console.ReadLine(), out int months))
            {
                Console.WriteLine("\nInvalid input.\n");
                Console.ReadKey();
                return;
            }

            savings.CalculateFutureBalance(rate, months);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}