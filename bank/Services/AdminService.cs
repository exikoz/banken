using bank.Core;
using bank.Utils;

namespace bank.Services
{
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

        private List<User> GetBlockedUsers()
        {
            return _blockedUsers ??= searchService.GetAllUsers().Where(u => u.isBlocked).ToList();
        }

        public void UnlockUserBlocks()
        {
            var blockedUsers = GetBlockedUsers();

            if (blockedUsers.Count == 0)
            {
                Console.WriteLine("No blocked users found.");
                TableFormatter.PauseForUser();
                return;
            }

            while (true)
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
                if (!userToUnlock.isBlocked)
                {
                    Console.WriteLine($"User: {userToUnlock.Name} (ID: {userToUnlock.Id}) has been unblocked successfully.");
                    Console.ResetColor();

                    _blockedUsers = null;
                    blockedUsers = GetBlockedUsers();
                }
            }
            TableFormatter.PauseForUser();
        }

        private void AddExchangeRate()
        {
            Console.Clear();
            Console.WriteLine("=== UPDATE EXCHANGE RATES ===\n");

            var rates = searchService.GetAllExchangeRates();
            Console.WriteLine("Available currencies:");
            TableFormatter.DisplayRatesTable(rates, "");

            Console.WriteLine("Pick a rate to add or update by typing in their code:");
            Console.WriteLine("Type 'NEW' if you want to add a new currency that isn’t listed.\n");

            var selectedRate = Console.ReadLine();

            if (string.Equals(selectedRate, "NEW", StringComparison.OrdinalIgnoreCase))
            {
                AddNewCurrency();
                return;
            }

            if (string.IsNullOrWhiteSpace(selectedRate))
            {
                Console.WriteLine("\n Invalid choice. Needs to be a number");
                TableFormatter.PauseForUser();
                return;
            }

            if (selectedRate.Equals("SEK", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Cannot change the base currency (SEK).");
                TableFormatter.PauseForUser();
                return;
            }

            if (!Enum.TryParse(selectedRate, true, out CurrencyCode choice))
            {
                Console.WriteLine("Invalid currency code. Choose one on the list.");
                TableFormatter.PauseForUser();
                return;
            }

            Console.WriteLine($"\n Enter the new exchange rate value for {selectedRate}: ");
            var rateInput = Console.ReadLine();
            if (!decimal.TryParse(rateInput, out var newRate))
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

        private void AddNewCurrency()
        {
            Console.Clear();
            Console.WriteLine("=== ADD NEW CURRENCY ===\n");

            Console.Write("Enter new currency code (e.g. EUR): ");
            var inputCode = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(inputCode))
            {
                Console.WriteLine("Invalid currency code.");
                TableFormatter.PauseForUser();
                return;
            }

            if (inputCode == "SEK")
            {
                Console.WriteLine("You cannot add or modify the base currency (SEK).");
                TableFormatter.PauseForUser();
                return;
            }

            var existingRates = searchService.GetAllExchangeRates();
            if (existingRates.Any(r =>
                (r.CustomCode != null && r.CustomCode.Equals(inputCode, StringComparison.OrdinalIgnoreCase)) ||
                r.Code.ToString().Equals(inputCode, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("That currency already exists.");
                TableFormatter.PauseForUser();
                return;
            }

            Console.Write($"Enter exchange rate for {inputCode} (vs SEK = 1.00): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rate) || rate <= 0)
            {
                Console.WriteLine("Invalid rate value.");
                TableFormatter.PauseForUser();
                return;
            }

            CurrencyCode parsedCode;
            bool isKnown = Enum.TryParse(inputCode, true, out parsedCode);

            if (!isKnown)
            {
                Console.WriteLine($"Note: '{inputCode}' is not a predefined currency, adding it as custom.");
                parsedCode = (CurrencyCode)(Enum.GetValues(typeof(CurrencyCode)).Cast<int>().Max() + 1);
            }

            var newRate = new ExchangeRate
            {
                Code = parsedCode,
                CustomCode = isKnown ? null : inputCode,
                Rate = rate,
                LastUpdated = DateTime.Now
            };

            exchangerateService.AddRates(newRate);

            Console.WriteLine($"\nAdded new currency {inputCode} with rate {rate}");
            TableFormatter.PauseForUser();
        }
    }
}
