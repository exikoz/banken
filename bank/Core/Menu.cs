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
        private User? currentUser;

        public Menu()
        {
            bank = new Bank();
            authService = new AuthenticationService(bank);
            accountService = new AccountService(bank);
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Create test users
            var user1 = new User("U001", "Alexander", "1234");
            var user2 = new User("U002", "Maria", "5678");

            bank.RegisterUser(user1);
            bank.RegisterUser(user2);

            // Create test accounts
            // Alexander (U001) has ACC001 with 0 balance
            bank.OpenAccount(user1, "ACC001");
            // Maria (U002) has ACC002 with 0 balance
            bank.OpenAccount(user2, "ACC002");
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
            // Display User info and main menu options
            Console.WriteLine($"--- Welcome, {currentUser!.Name} ({currentUser.Id}) ---");
            Console.WriteLine("\n1. View My Accounts (Balance)");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Open New Account"); // New option added
            Console.WriteLine("5. Log Out");
            Console.WriteLine("6. Exit");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Calls the ShowAccounts method in AccountService
                    accountService.ShowAccounts(currentUser!);
                    break;
                case "2":
                    // Calls the Deposit method in AccountService
                    accountService.Deposit(currentUser!);
                    break;
                case "3":
                    // Calls the Withdraw method in AccountService
                    accountService.Withdraw(currentUser!);
                    break;
                case "4":
                    // Calls the CreateAccount method in AccountService
                    accountService.CreateAccount(currentUser!);
                    break;
                case "5":
                    // Log out logic: simply setting currentUser to null
                    currentUser = null;
                    Console.WriteLine("\nSuccessfully logged out. Press any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "6":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}