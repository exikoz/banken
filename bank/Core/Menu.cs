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
        private readonly LoanService loanService;
        private readonly TransferService transferService; 

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
            Console.WriteLine("0. Exit");
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
                case "0":
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
            Console.WriteLine("\n1. View My Accounts (Balance)");
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
                    Console.WriteLine("\nSuccessfully logged out. Press any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }

        }

        private void ShowAdminMenu()
        {
            Console.WriteLine("\n1. Admin Dashboard");
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
                    Console.WriteLine("\nSuccessfully logged out. Press any key to return to the welcome screen.");
                    Console.ReadKey();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
        private void ShowTransferMenu()
            { 
            Console.WriteLine("=== TRANSFER MONEY ===\n");
            Console.WriteLine("1. Transfer between my own accounts");
            Console.WriteLine("2. Transfer to another customer");
            Console.WriteLine("3. Back to main menu");
            Console.Write("\nChoose option: ");

            var choice = Console.ReadLine();
            switch (choice) {
                case "1":
                    transferService.DoTransferOwn(currentUser!);
                    break;
                
                case "2":
                    transferService.DoTransferToOther(currentUser!);
                    break;

                case "3":
                    return;

                default:
                    Console.WriteLine("\n✗ Invalid choice. Press any key to try again.");
                    Console.ReadKey();
                    break;

            }

        }
    }
}
