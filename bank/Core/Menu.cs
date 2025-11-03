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
                ShowAdminMenu();
            else
                ShowCustomerMenu();
        }

        private void ShowCustomerMenu()
        {
            Console.WriteLine("1. View My Accounts (Balance)");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Open New Account");
            Console.WriteLine("5. Calculate Interest");
            Console.WriteLine("6. Transfer Money");
            Console.WriteLine("7. View Transaction Log");
            Console.WriteLine("8. Apply for Loan");
            Console.WriteLine("9. Log Out");
            Console.WriteLine("0. Exit");
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
                    ShowTransferMenu();
                    break;
                case "7":
                    transactionService.ShowTransactionLog(currentUser!);
                    break;
                case "8":
                    loanService.ApplyForLoan(currentUser!);
                    break;
                case "9":
                    currentUser = null;
                    ConsoleHelper.WriteSuccess("Successfully logged out.");
                    Console.WriteLine("\nPress any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "0":
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
            Console.WriteLine("0. Exit");
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
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    ConsoleHelper.WriteError("Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }

        private void ShowTransferMenu()
        {
            ConsoleHelper.WriteHeader("TRANSFER MONEY");

            Console.WriteLine("1. Transfer between my own accounts");
            Console.WriteLine("2. Transfer to another customer");
            Console.WriteLine("3. Back to main menu");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    transferService.DoTransferOwn(currentUser!);
                    break;
                case "2":
                    transferService.TransferToOther(currentUser!); // works once overload added
                    break;
                case "3":
                    return;
                default:
                    ConsoleHelper.WriteError("Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
