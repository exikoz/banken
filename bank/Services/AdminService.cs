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
        private readonly ExchangerateService exchangerateService = new ExchangerateService();
        private readonly SearchService searchService;

        public AdminService(Bank bank)
        {
            this.bank = bank;
            this.searchService = new SearchService(bank, exchangerateService);
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
                Console.WriteLine("4. View All Exchange rates");
                Console.WriteLine("5. Add/Update Exchange rates");
                Console.WriteLine("6. Unblock users");
                Console.WriteLine("7. Back to Main Menu");
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
                        ViewAllRates();
                        break;
                    case "5":
                        AddExchangeRate();
                        break;
                    case "6":
                        UnlockUserBlocks();
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("\n Invalid choice.");
                        TableFormatter.PauseForUser();
                        break;
                }
            }
        }



        private List<User>? _blockedUsers;


        // if blockedUsers have not been initialized, then we default to fetching them 
        private List<User> GetBlockedUsers()
        {
           return _blockedUsers ??= searchService.GetAllUsers().Where(u => u.isBlocked).ToList();
        }




        public void UnlockUserBlocks()
        {



            // Here we init  blocked users list
            var blockedUsers = GetBlockedUsers();

            if (blockedUsers.Count == 0) 
            {
                Console.WriteLine("No blocked users found.");
                TableFormatter.PauseForUser();
                return;
            }



            while(true)
            {



                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var user in blockedUsers)
                {
                    Console.WriteLine($"User: {user.Name} (ID: {user.Id}) is blocked");
                }


                Console.ResetColor();

                Console.WriteLine("Type in user ID to unblock or type 'B' to go back:");
                var input = Console.ReadLine();

                if (string.Equals(input, "B", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }


                var userToUnlock = blockedUsers.FirstOrDefault(u => u.Id == input);

                if (userToUnlock == null)
                {
                    Console.WriteLine("Invalid user ID. Please try again.");
                    TableFormatter.PauseForUser();
                    continue;
                }

                userToUnlock.FailedLoginAttempts = 0;

                Console.ForegroundColor = ConsoleColor.Green;
                if(!userToUnlock.isBlocked)
                {
                    Console.WriteLine($"User: {userToUnlock.Name} (ID: {userToUnlock.Id}) has been unblocked successfully.");
                    Console.ResetColor();


                    // Refresh the blocked users list
                    _blockedUsers = null;
                    blockedUsers = GetBlockedUsers();
                }

            }
            TableFormatter.PauseForUser();
        }


        // TODO I ÖVRIGT: När vi kallar listor, borde vi ha en tom först som fylls första gången den kallas
        // och sedan istället för att direkt alltid häma, borde vi först kolla om listan är tom , så sparar man pp onödiga hämntingar




        // TODO - fixa så att man bara kan skriva in språk och valutakod på olika sätt (sek, sve, etc), just nu funkar denna halvt pga en miss
        private void AddExchangeRate()
        {
            Console.Clear();
            Console.WriteLine("=== UPDATE EXCHANGE RATES ===\n");

            var rates = searchService.GetAllExchangeRates();


            Console.WriteLine("Available currencies:");

            TableFormatter.DisplayRatesTable(rates, "");

            if (!rates.Any())
            {
                exchangerateService.initJson();
                var codes = Enum.GetValues<CurrencyCode>();
                foreach (var code in codes)
                {
                    Console.WriteLine($"{(int)code}. {code}");
                }

            }

            Console.WriteLine("Pick a rate to add or update by typing in their code:\n");


            var selectedRate = Console.ReadLine();

            if (selectedRate == null)
            {
                Console.WriteLine("\n Invalid choice. Needs to be a number");
                TableFormatter.PauseForUser();
                return;
            }

            if (selectedRate.ToLower() == "sek")
            {
                Console.WriteLine("Cannot change the base currency");
                        TableFormatter.PauseForUser();
                return;
            }

            if(!Enum.TryParse(selectedRate, true, out CurrencyCode choice))
            {
                Console.WriteLine("Invalid currency code. Choose one on the list.");
                TableFormatter.PauseForUser();
                return;
            }



            if (!rates.Any())
            {
                Console.WriteLine("No exchangerates added yet. Time to add one!");
                var codes = Enum.GetValues<CurrencyCode>();
                foreach (var code in codes)
                {
                    Console.WriteLine($"{(int)code}. {code}");
                }
            }
            else if(choice < 0)
            {
                Console.WriteLine("\n Invalid choice. Needs to be one of the listed rates");
                TableFormatter.PauseForUser();
                return;
            }

            Console.WriteLine($"\n Enter the new exchange rate value for {selectedRate}: ");
            var rateInput = Console.ReadLine();
            if(!decimal.TryParse(rateInput, out var newRate))
            {
                Console.WriteLine("\n Invalid rate value. Needs to be a decimal number");
                TableFormatter.PauseForUser();
                return;
            }

            if (newRate <= 0 || newRate >= 999)
            {
                Console.WriteLine("\n Exchange rate cannot be too low or high!");
                TableFormatter.PauseForUser();
                return;
            }


            exchangerateService.AddRates(new ExchangeRate((CurrencyCode)choice, newRate));
            TableFormatter.PauseForUser();
        }



        private void ViewAllRates()
        {
            Console.Clear();
            Console.WriteLine("=== ALL RATES ===\n");

            var rates = searchService.GetAllExchangeRates();

            if (!rates.Any())
            {
                Console.WriteLine("No exchangerates added yet.");
                TableFormatter.PauseForUser();
                return;
            }

            TableFormatter.DisplayRatesTable(rates, "ALL EXCHANGE RATES");
            TableFormatter.PauseForUser();
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