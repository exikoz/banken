using bank.Core;
using bank.Utils;

namespace bank.Services
{
    /// <summary>
    /// Admin service - handles admin dashboard and delegates to specialized services
    /// </summary>
    public class AdminService
    {
        private readonly Bank bank;
        private readonly SearchService searchService;

        public AdminService(Bank bank)
        {
            this.bank = bank;
            this.searchService = new SearchService(bank);
        }

        /// <summary>
        /// Shows the admin dashboard
        /// </summary>
        public void ShowAdminDashboard(User admin)
        {
            if (!admin.IsAdmin())
            {
                Console.WriteLine("Access Denied: Admin privileges required.");
                TableFormatter.PauseForUser();
                return;
            }

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN DASHBOARD ===");
                Console.WriteLine($"Logged in as: {admin.Name} (Admin)\n");

                Console.WriteLine("1. Search Account");
                Console.WriteLine("2. View All Users");
                Console.WriteLine("3. View All Accounts");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("\nChoose option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchAccountMenu();
                        break;
                    case "2":
                        ViewAllUsers();
                        break;
                    case "3":
                        ViewAllAccounts();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\n✗ Invalid choice.");
                        TableFormatter.PauseForUser();
                        break;
                }
            }
        }

        /// <summary>
        /// Display all users in the system
        /// </summary>
        private void ViewAllUsers()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW ALL USERS ===\n");

            var allUsers = searchService.GetAllUsers();

            if (!allUsers.Any())
            {
                Console.WriteLine("No users in the system.");
                TableFormatter.PauseForUser();
                return;
            }

            TableFormatter.DisplayUsersTable(allUsers, "ALL USERS");
            TableFormatter.PauseForUser();
        }

        /// <summary>
        /// Displays all accounts in the system (admin only)
        /// </summary>
        /// <summary>
        /// Displays all accounts in the system (admin only)
        /// </summary>
        /// <summary>
        /// Display all accounts in the system
        /// </summary>
        private void ViewAllAccounts()
        {
            Console.Clear();
            Console.WriteLine("=== VIEW ALL ACCOUNTS ===\n");

            var allAccounts = searchService.GetAllAccounts();

            if (!allAccounts.Any())
            {
                Console.WriteLine("No accounts in the system.");
                TableFormatter.PauseForUser();
                return;
            }

            TableFormatter.DisplayAccountsTable(allAccounts, "ALL ACCOUNTS");
            TableFormatter.PauseForUser();
        }

        /// <summary>
        /// Search account menu
        /// </summary>
        private void SearchAccountMenu()
        {
            Console.Clear();
            Console.WriteLine("=== SEARCH ACCOUNT ===\n");

            Console.WriteLine("Search by:");
            Console.WriteLine("1. Account Number");
            Console.WriteLine("2. Username");
            Console.Write("\nChoose (1-2): ");

            var searchType = Console.ReadLine();

            switch (searchType)
            {
                case "1":
                    SearchByAccountNumber();
                    break;
                case "2":
                    SearchByUsername();
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice.");
                    TableFormatter.PauseForUser();
                    break;
            }
        }

        /// <summary>
        /// Search by account number
        /// </summary>
        private void SearchByAccountNumber()
        {
            Console.Write("\nEnter account number: ");
            var accountNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                Console.WriteLine("\n✗ Account number cannot be empty!");
                TableFormatter.PauseForUser();
                return;
            }

            var account = searchService.FindAccountByNumber(accountNumber);

            if (account == null)
            {
                Console.WriteLine($"\n✗ No account found with number: {accountNumber}");
                TableFormatter.PauseForUser();
                return;
            }

            TableFormatter.DisplayAccountsTable(new List<Account> { account });
            TableFormatter.PauseForUser();
        }

        /// <summary>
        /// Search by username
        /// </summary>
        private void SearchByUsername()
        {
            Console.Write("\nEnter username: ");
            var username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("\n✗ Username cannot be empty!");
                TableFormatter.PauseForUser();
                return;
            }

            var accounts = searchService.FindAccountsByUsername(username);

            if (!accounts.Any())
            {
                Console.WriteLine($"\n✗ No accounts found for user: {username}");
                TableFormatter.PauseForUser();
                return;
            }

            TableFormatter.DisplayAccountsTable(accounts);
            TableFormatter.PauseForUser();
        }
    }
}