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
                Console.WriteLine("4. Create New User");
                Console.WriteLine("5. Back to Main Menu");
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
                        CreateNewUser();
                        break;
                    case "5":
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
        /// Create a new user in the system (Admin only)
        /// </summary>
        private void CreateNewUser()
        {
            Console.Clear();
            Console.WriteLine("=== CREATE NEW USER ===\n");

            // Get User ID
            Console.Write("Enter User ID (e.g., U005): ");
            var userId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.WriteLine("\n✗ User ID cannot be empty!");
                TableFormatter.PauseForUser();
                return;
            }

            // Check if user ID already exists
            if (bank.FindUser(userId) != null)
            {
                Console.WriteLine($"\n✗ User ID '{userId}' already exists!");
                TableFormatter.PauseForUser();
                return;
            }

            // Get Name
            Console.Write("Enter user name: ");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("\n✗ Name cannot be empty!");
                TableFormatter.PauseForUser();
                return;
            }

            // Get PIN
            Console.Write("Create a PIN (4 digits): ");
            var pin = ReadPassword();

            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                Console.WriteLine("\n✗ PIN must be exactly 4 digits!");
                TableFormatter.PauseForUser();
                return;
            }

            // Get User Role
            Console.WriteLine("\nSelect user role:");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Admin");
            Console.Write("\nChoose (1-2): ");
            var roleChoice = Console.ReadLine();

            UserRole role = UserRole.Customer;
            switch (roleChoice)
            {
                case "1":
                    role = UserRole.Customer;
                    break;
                case "2":
                    role = UserRole.Admin;
                    Console.WriteLine("\n⚠ Warning: Creating admin user with elevated privileges.");
                    break;
                default:
                    Console.WriteLine("\n✗ Invalid choice. Defaulting to Customer role.");
                    break;
            }

            // Create the user
            var newUser = new User(userId, name, pin, role);
            bank.RegisterUser(newUser);

            Console.WriteLine($"\n✓ User '{name}' (ID: {userId}) created successfully as {role}!");

            // Optionally create an account for the new user
            Console.Write("\nWould you like to create an account for this user? (y/n): ");
            var createAccount = Console.ReadLine()?.ToLower();

            if (createAccount == "y" || createAccount == "yes")
            {
                CreateAccountForNewUser(newUser);
            }

            TableFormatter.PauseForUser();
        }

        /// <summary>
        /// Create an account for a newly created user
        /// </summary>
        private void CreateAccountForNewUser(User user)
        {
            Console.Write("\nEnter account number (e.g., ACC007): ");
            var accountNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                Console.WriteLine("\n✗ Account number cannot be empty!");
                return;
            }

            if (bank.FindAccount(accountNumber) != null)
            {
                Console.WriteLine($"\n✗ Account number '{accountNumber}' already exists!");
                return;
            }

            // Create standard account
            bank.OpenAccount(user, accountNumber);
            Console.WriteLine($"\n✓ Account '{accountNumber}' created successfully!");
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

        /// <summary>
        /// Reads password input with masked characters (asterisks)
        /// </summary>
        private string ReadPassword()
        {
            var password = "";
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.KeyChar >= '0' && key.KeyChar <= '9')
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return password;
        }
    }
}