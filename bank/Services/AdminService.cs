using bank.Core;
using bank.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // Checks if ESC or empty input
        private bool IsBack(string input)
        {
            return input == "<ESC>" || string.IsNullOrWhiteSpace(input);
        }

        // Reads input using ConsoleHelper
        private string ReadInput(string message)
        {
            return ConsoleHelper.PromptWithEscape(message);
        }

        // Admin dashboard menu
        public void ShowAdminDashboard(User admin)
        {
            if (!admin.IsAdmin())
            {
                ConsoleHelper.WriteError("Access denied.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.WriteHeader("ADMIN DASHBOARD");
                ConsoleHelper.WriteInfo($"Logged in as: {admin.Name} (Admin)");
                Console.WriteLine();

                ConsoleHelper.WriteHighlight("USER MANAGEMENT", ConsoleColor.Cyan);
                ConsoleHelper.WriteMenuOption("1", "Search Account");
                ConsoleHelper.WriteMenuOption("2", "View All Users");
                ConsoleHelper.WriteMenuOption("3", "Create New User");
                ConsoleHelper.WriteMenuOption("4", "Unblock Users");
                Console.WriteLine();

                ConsoleHelper.WriteHighlight("ACCOUNTS AND RATES", ConsoleColor.Cyan);
                ConsoleHelper.WriteMenuOption("5", "View All Accounts");
                ConsoleHelper.WriteMenuOption("6", "View All Exchange Rates");
                ConsoleHelper.WriteMenuOption("7", "Add or Update Exchange Rates");
                ConsoleHelper.WriteMenuOption("8", "Update Savings Rate");
                ConsoleHelper.WriteMenuOption("9", "Update Loan Rate");
                Console.WriteLine();

                ConsoleHelper.WriteMenuOption("10", "Back");
                Console.WriteLine();

                var choice = ReadInput("Choose option");
                if (IsBack(choice))
                    return;

                switch (choice)
                {
                    case "1": SearchAccountMenu(); break;
                    case "2": ViewAllUsers(); break;
                    case "3": CreateNewUserUI(); break;
                    case "4": UnblockUsers(); break;
                    case "5": ViewAllAccounts(); break;
                    case "6": ViewAllRates(); break;
                    case "7": EditExchangeRate(); break;
                    case "8": UpdateSavingsInterestRate(); break;
                    case "9": UpdateLoanInterestRate(); break;
                    case "10": return;

                    default:
                        ConsoleHelper.WriteWarning("Invalid choice.");
                        ConsoleHelper.PauseWithMessage();
                        break;
                }
            }
        }

        // Creates a new user
        private void CreateNewUserUI()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("CREATE NEW USER");

            ConsoleHelper.WriteInfo("Enter the user details below:");
            Console.WriteLine();

            var id = ReadInput("Enter SSN (YYYYMMDD-XXXX)");
            if (IsBack(id)) return;

            var name = ReadInput("Enter name");
            if (IsBack(name)) return;

            var pin = ConsoleHelper.PromptWithEscapeMasked("Enter PIN (4 digits)");
            if (IsBack(pin)) return;

            var user = new User(id, name, pin, UserRole.Customer);
            bank.Users.Add(user);

            ConsoleHelper.WriteSuccess($"User '{name}' created successfully.");
            ConsoleHelper.WriteInfo($"User ID: {id}");
            ConsoleHelper.PauseWithMessage();
        }

        private List<User>? _blockedUsers;

        // Gets blocked users
        private List<User> GetBlockedUsers()
        {
            return _blockedUsers ??= searchService.GetAllUsers().Where(u => u.isBlocked).ToList();
        }

        // Unblocks users
        public void UnblockUsers()
        {
            while (true)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.WriteHeader("UNBLOCK USERS");

                var blockedUsers = GetBlockedUsers();

                if (!ValidationHelper.IsValid(blockedUsers))
                {
                    ConsoleHelper.WriteInfo("No blocked users.");
                    ConsoleHelper.PauseWithMessage();
                    return;
                }

                TableFormatter.DisplayUsersTable(blockedUsers, "BLOCKED USERS");

                Console.WriteLine();
                ConsoleHelper.WriteInfo("These users are currently blocked due to too many failed login attempts.");
                Console.WriteLine();

                var input = ReadInput("Enter user ID to unblock");
                if (IsBack(input)) return;

                var userToUnlock = blockedUsers.FirstOrDefault(u => u.Id == input);

                if (userToUnlock == null)
                {
                    ConsoleHelper.WriteWarning("Invalid ID.");
                    ConsoleHelper.PauseWithMessage();
                    continue;
                }

                userToUnlock.FailedLoginAttempts = 0;
                _blockedUsers = null;

                ConsoleHelper.WriteSuccess($"User '{userToUnlock.Name}' has been successfully unblocked.");
                ConsoleHelper.PauseWithMessage();
            }
        }

        // Lists all users
        private void ViewAllUsers()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ALL USERS");

            var users = searchService.GetAllUsers();

            if (!ValidationHelper.IsValid(users))
            {
                ConsoleHelper.WriteWarning("No users found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            TableFormatter.DisplayUsersTable(users, "ALL REGISTERED USERS");

            ConsoleHelper.PauseWithMessage();
        }

        // Lists all accounts
        private void ViewAllAccounts()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ALL ACCOUNTS");

            var accounts = searchService.GetAllAccounts();

            if (!accounts.Any())
            {
                ConsoleHelper.WriteWarning("No accounts found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            TableFormatter.DisplayAccountsTable(accounts, "ALL BANK ACCOUNTS");

            ConsoleHelper.PauseWithMessage();
        }

        // Search menu
        private void SearchAccountMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("SEARCH ACCOUNT");

            ConsoleHelper.WriteMenuOption("1", "Search by Account Number");
            ConsoleHelper.WriteMenuOption("2", "Search by Username");
            Console.WriteLine();

            var choice = ReadInput("Choose");
            if (IsBack(choice)) return;

            if (choice == "1") SearchByAccountNumber();
            else if (choice == "2") SearchByUsername();
            else
            {
                ConsoleHelper.WriteWarning("Invalid choice.");
                ConsoleHelper.PauseWithMessage();
            }
        }

        // Search by account number
        private void SearchByAccountNumber()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("SEARCH BY ACCOUNT NUMBER");

            var accountNumber = ReadInput("Enter account number");
            if (IsBack(accountNumber)) return;

            var account = searchService.FindAccountByNumber(accountNumber);

            if (account == null)
            {
                ConsoleHelper.WriteWarning("Account not found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            TableFormatter.DisplayAccountsTable(new List<Account> { account }, "SEARCH RESULT");

            ConsoleHelper.PauseWithMessage();
        }

        // Search by username
        private void SearchByUsername()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("SEARCH BY USERNAME");

            var username = ReadInput("Enter username");
            if (IsBack(username)) return;

            var accounts = searchService.FindAccountsByUsername(username);

            if (!accounts.Any())
            {
                ConsoleHelper.WriteWarning("No accounts found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            TableFormatter.DisplayAccountsTable(accounts, $"ACCOUNTS FOR '{username}'");

            ConsoleHelper.PauseWithMessage();
        }

        // Shows all exchange rates
        private void ViewAllRates()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("EXCHANGE RATES");

            var rates = searchService.GetAllExchangeRates();

            if (!rates.Any())
            {
                ConsoleHelper.WriteWarning("No rates found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            TableFormatter.DisplayRatesTable(rates, "CURRENT EXCHANGE RATES");

            ConsoleHelper.PauseWithMessage();
        }

        // Adds or updates a rate




        private void EditExchangeRate()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("UPDATE/DELETE/CREATE EXCHANGE RATES");

            var rates = searchService.GetAllExchangeRates();

            TableFormatter.DisplayRatesTable(rates, "CURRENT EXCHANGE RATES");

            Console.WriteLine();
            var selected = ReadInput("Enter currency code or type (N) to add a new one").ToUpper();
            if (IsBack(selected)) return;

            if (selected.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                AddNewCurrency();
                return;
            }
            if(selected.Equals("SEK") || selected.Equals("0"))
            {
                ConsoleHelper.WriteWarning("Cannot edit base currency (SEK).");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            if (!Enum.TryParse(selected, true, out CurrencyCode code))
            {
                ConsoleHelper.WriteWarning("Invalid currency code.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            ConsoleHelper.WriteInfo($"Selected currency: {selected}");
            Console.WriteLine();
            var CrudChoice = ReadInput($"Choose action - (U) Update | (D) Delete");
            if (IsBack(CrudChoice)) return;

            if (CrudChoice.Equals("U", StringComparison.OrdinalIgnoreCase))
            {
                var rateText = ReadInput($"Enter new rate for {selected} to SEK");
                if (IsBack(rateText)) return;

                if (!decimal.TryParse(rateText, out var newRate))
                {
                    ConsoleHelper.WriteWarning("Invalid rate. Please enter a valid number.");
                    ConsoleHelper.PauseWithMessage();
                    return;
                }

                exchangerateService.AddRates(new ExchangeRate(code, newRate));
                ConsoleHelper.WriteSuccess($"Exchange rate for {selected} updated to {newRate}.");
                ConsoleHelper.PauseWithMessage();
            }
            else if (CrudChoice.Equals("D", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleHelper.WriteWarning($"Are you sure you want to delete {selected}?");
                var confirmation = ReadInput("Type (Y) to confirm or (N) to cancel");
                if (IsBack(confirmation)) return;

                if(confirmation.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    exchangerateService.DeleteField(code);
                    ConsoleHelper.WriteSuccess($"Currency {selected} deleted successfully.");
                    ConsoleHelper.PauseWithMessage();
                }
                else
                {
                    ConsoleHelper.WriteInfo("Deletion cancelled.");
                    ConsoleHelper.PauseWithMessage();
                }
            }
            else
            {
                ConsoleHelper.WriteWarning("Invalid choice.");
                ConsoleHelper.PauseWithMessage();
            }



        }

        // Adds a completely new currency
        private void AddNewCurrency()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ADD NEW CURRENCY");

            ConsoleHelper.WriteInfo("Add a new currency to the exchange rate system:");
            Console.WriteLine();

            var code = ReadInput("Enter currency code (e.g. EUR, INR)");
            if (IsBack(code)) return;

            var rateText = ReadInput("Enter exchange rate to SEK");
            if (IsBack(rateText)) return;

            if (!decimal.TryParse(rateText, out var rate))
            {
                ConsoleHelper.WriteWarning("Invalid rate. Please enter a valid number.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            var newRate = new ExchangeRate
            {
                Code = 0,
                CustomCode = code.ToUpper(),
                Rate = rate,
                LastUpdated = DateTime.Now
            };

            exchangerateService.AddRates(newRate);

            ConsoleHelper.WriteSuccess($"Currency '{code.ToUpper()}' added successfully with rate {rate}.");
            ConsoleHelper.PauseWithMessage();
        }

        // Updates savings interest rate
        private void UpdateSavingsInterestRate()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("UPDATE SAVINGS RATE");

            ConsoleHelper.WriteInfo($"Current savings interest rate: {bank.DefaultSavingsInterestRate}%");
            Console.WriteLine();

            var input = ReadInput("Enter new rate (%)");
            if (IsBack(input)) return;

            if (!decimal.TryParse(input, out var newRate))
            {
                ConsoleHelper.WriteWarning("Invalid rate.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            bank.UpdateDefaultSavingsRate(newRate);
            ConsoleHelper.WriteSuccess($"Savings rate updated from {bank.DefaultSavingsInterestRate}% to {newRate}%.");
            ConsoleHelper.PauseWithMessage();
        }

        // Updates loan interest rate
        private void UpdateLoanInterestRate()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("UPDATE LOAN RATE");

            ConsoleHelper.WriteInfo($"Current loan interest rate: {LoanService.CurrentLoanInterestRate}%");
            Console.WriteLine();

            var input = ReadInput("Enter new rate (%)");
            if (IsBack(input)) return;

            if (!decimal.TryParse(input, out var newRate))
            {
                ConsoleHelper.WriteWarning("Invalid rate.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            var oldRate = LoanService.CurrentLoanInterestRate;
            LoanService.SetLoanInterestRate(newRate);
            ConsoleHelper.WriteSuccess($"Loan rate updated from {oldRate}% to {newRate}%.");
            ConsoleHelper.PauseWithMessage();
        }
    }
}
