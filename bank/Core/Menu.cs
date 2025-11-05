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
        private readonly TransactionService transactionService;
        private readonly InterestService interestService;
        private readonly LoanService loanService;
        private readonly TransferService transferService;

        private User? currentUser;
        private bool hasShownSplash = false;

        public Menu()
        {
            bank = new Bank();
            authService = new AuthenticationService(bank);
            accountService = new AccountService(bank);
            adminService = new AdminService(bank);
            transactionService = new TransactionService(bank);
            interestService = new InterestService(bank);
            loanService = new LoanService(bank);
            transferService = new TransferService(bank, accountService);

            DataSeeder.SeedTestData(bank);
        }

        // Handles top-level UI loop
        public void DrawUI()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Malmö Royal Bank";

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
                    ConsoleHelper.ClearScreen();
                    ShowMainMenu();
                }
            }
        }

        // Login options
        private void ShowLoginMenu()
        {
            ConsoleHelper.WriteHeader("WELCOME");

            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Register New User");
            Console.WriteLine("0. Exit");
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
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.WriteError("Invalid choice.");
                    Console.ReadKey();
                    break;
            }
        }

        // Routing user to correct menu
        private void ShowMainMenu()
        {
            Console.WriteLine("MALMÖ ROYAL BANK");
            Console.WriteLine($"Logged in as: {currentUser!.Name} ({currentUser.Id}) [{currentUser.Role}]\n");

            if (currentUser.IsAdmin())
                ShowAdminMenu();
            else
                ShowCustomerMenu();
        }

        // Menu for regular customers
        private void ShowCustomerMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("MAIN MENU");

            Console.WriteLine("1. View My Accounts");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Open New Account");
            Console.WriteLine("5. Transfer Money");
            Console.WriteLine("6. View Transaction Log");
            Console.WriteLine("7. Apply for Loan");
            Console.WriteLine("8. Interest Calculation");
            Console.WriteLine("9. Log Out");
            Console.WriteLine("0. Exit");

            var choice = ConsoleHelper.PromptWithEscape("Choose option");

            if (choice == "<ESC>")
                return;

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
                    ShowTransferMenu();
                    break;

                case "6":
                    transactionService.ShowTransactionLog(currentUser!);
                    break;

                case "7":
                    loanService.ApplyForLoan(currentUser!);
                    break;

                case "8":
                    interestService.CalculateInterest(currentUser!);
                    break;

                case "9":
                    Logout();
                    break;

                case "0":
                    Environment.Exit(0);
                    break;

                default:
                    ConsoleHelper.WriteError("Invalid choice.");
                    ConsoleHelper.PauseWithMessage();
                    break;
            }
        }

        // Menu for admin users
        private void ShowAdminMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ADMIN MENU");

            Console.WriteLine("1. Admin Dashboard");
            Console.WriteLine("2. View My Accounts");
            Console.WriteLine("3. Log Out");
            Console.WriteLine("0. Exit");

            var choice = ConsoleHelper.PromptWithEscape("Choose option");

            if (choice == "<ESC>" || string.IsNullOrWhiteSpace(choice))
                return;

            switch (choice)
            {
                case "1":
                    adminService.ShowAdminDashboard(currentUser!);
                    break;

                case "2":
                    accountService.ShowAccounts(currentUser!);
                    break;

                case "3":
                    Logout();
                    break;

                case "0":
                    Environment.Exit(0);
                    break;

                default:
                    ConsoleHelper.WriteError("Invalid choice.");
                    ConsoleHelper.PauseWithMessage();
                    break;
            }
        }

        // Handles transfers
        private void ShowTransferMenu()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER MONEY");

            Console.WriteLine("1. Transfer between my own accounts");
            Console.WriteLine("2. Transfer to another customer");
            Console.WriteLine("ESC. Back to main menu");
            Console.Write("\nChoose option: ");

            var choice = ConsoleHelper.PromptWithEscape("Choose option");

            if (choice == "<ESC>")
                return;

            switch (choice)
            {
                case "1":
                    transferService.DoTransferOwn(currentUser!);
                    break;

                case "2":
                    transferService.TransferToOther(currentUser!);
                    break;

                default:
                    ConsoleHelper.WriteError("Invalid choice.");
                    Console.ReadKey();
                    break;
            }
        }


        // Logout helper
        private void Logout()
        {
            currentUser = null;
            ConsoleHelper.WriteSuccess("Successfully logged out.");
            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}
