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

        private void ShowLoginMenu()
        {
            ConsoleHelper.WriteHeader("WELCOME");

            ConsoleHelper.WriteMenuOption("1", "Log In");
            ConsoleHelper.WriteMenuOption("2", "Register New User");
            ConsoleHelper.WriteMenuOption("0", "Exit");

            var choice = ConsoleHelper.PromptWithEscape("Choose option");

            if (choice == "<ESC>")
                return;

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
                    ConsoleHelper.PauseWithMessage();
                    break;
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("MALMÖ ROYAL BANK");
            Console.WriteLine($"Logged in as: {currentUser!.Name} ({currentUser.Id}) [{currentUser.Role}]\n");

            if (currentUser.IsAdmin())
                ShowAdminMenu();
            else
                ShowCustomerMenu();
        }

        private void ShowCustomerMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("MAIN MENU");

            ConsoleHelper.WriteMenuOption("1", "View My Accounts");
            ConsoleHelper.WriteMenuOption("2", "Deposit Money");
            ConsoleHelper.WriteMenuOption("3", "Withdraw Money");
            ConsoleHelper.WriteMenuOption("4", "Open New Account");
            ConsoleHelper.WriteMenuOption("5", "Transfer Money");
            ConsoleHelper.WriteMenuOption("6", "View Transaction Log");
            ConsoleHelper.WriteMenuOption("7", "Apply for Loan");
            ConsoleHelper.WriteMenuOption("8", "Interest Calculation");
            ConsoleHelper.WriteMenuOption("9", "Log Out");
            ConsoleHelper.WriteMenuOption("0", "Exit");
            Console.WriteLine();

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

        private void ShowAdminMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ADMIN MENU");

            ConsoleHelper.WriteMenuOption("1", "Admin Dashboard");
            ConsoleHelper.WriteMenuOption("2", "View My Accounts");
            ConsoleHelper.WriteMenuOption("3", "Log Out");
            ConsoleHelper.WriteMenuOption("0", "Exit");
            Console.WriteLine();

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

        private void ShowTransferMenu()
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("TRANSFER MENU");

            ConsoleHelper.WriteMenuOption("1", "Transfer between own accounts");
            ConsoleHelper.WriteMenuOption("2", "Transfer to another customer");
            ConsoleHelper.WriteMenuOption("0", "Back");
            Console.WriteLine();

            var choice = ConsoleHelper.PromptWithEscape("Choose option");

            if (choice == "<ESC>" || choice == "0")
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
                    ConsoleHelper.PauseWithMessage();
                    break;
            }
        }

        private void Logout()
        {
            currentUser = null;
            if (!hasShownSplash)
            {
                ConsoleHelper.WriteInfo("See you next time!");
                ConsoleHelper.PauseWithMessage();
                hasShownSplash = true;
            }
        }
    }
}
