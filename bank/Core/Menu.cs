using bank.Services;
using bank.Utils;

namespace bank.Core
{
    public class Menu
    {
        private readonly Bank bank;
        private readonly AuthenticationService authService;
        private readonly AccountService accountService;
        private readonly AdminService adminService;
        private User? currentUser;
        private readonly TransactionService transactionService;
        private readonly InterestService interestService;
        private bool hasShownSplash = false;

        public Menu()
        {
            bank = new Bank();
            authService = new AuthenticationService(bank);
            accountService = new AccountService(bank);
            adminService = new AdminService(bank);
            transactionService = new TransactionService(bank);
            interestService = new InterestService(bank);

            DataSeeder.SeedTestData(bank);
        }

        public void DrawUI()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Malmö Royal Bank";

            // Show splash screen only once
            //if (!hasShownSplash)
            //{
            //    AsciiArt.DisplaySplashScreen();
            //    hasShownSplash = true;
            //}

            while (true)
            {
                if (currentUser == null)
                {
                    ConsoleHelper.ClearScreen();
                    AsciiArt.DisplayBankLogo();
                    ShowLoginMenu();
                }
                else
                {
                    Console.Clear();
                    ShowMainMenu();
                }
            }
        }

        private void ShowLoginMenu()
        {
            ConsoleHelper.WriteHeader("WELCOME");

            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Register New User");
            Console.WriteLine("3. Exit");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    currentUser = authService.ShowLoginUI();
                    break;
                case "2":
                    authService.ShowRegistrationUI();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.WriteError("Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine($"=== MALMÖ ROYAL BANK ===");
            Console.WriteLine($"Logged in as: {currentUser!.Name} ({currentUser.Id}) [{currentUser.Role}]\n");

            if (currentUser.IsAdmin())
            {
                ShowAdminMenu();
            }
            else
            {
                ShowCustomerMenu();
            }
        }

        private void ShowCustomerMenu()
        {
            Console.WriteLine("1. View My Accounts (Balance)");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Open New Account");
            Console.WriteLine("5. Calculate Interest");
            Console.WriteLine("6. View Transaction Log");
            Console.WriteLine("7. Log Out");
            Console.WriteLine("8. Exit");
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
                    interestService.CalculateInterest(currentUser!);
                    break;
                case "6":
                    transactionService.ShowTransactionLog(currentUser!);
                    break;
                case "7":
                    currentUser = null;
                    ConsoleHelper.WriteSuccess("Successfully logged out.");
                    Console.WriteLine("\nPress any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "8":
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.WriteError("Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }

        private void ShowAdminMenu()
        {
            Console.WriteLine("1. Admin Dashboard");
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
                    ConsoleHelper.WriteSuccess("Successfully logged out.");
                    Console.WriteLine("\nPress any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.WriteError("Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}