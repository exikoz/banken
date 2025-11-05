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
                Console.WriteLine($"Logged in as: {admin.Name} (Admin)\n");

                Console.WriteLine("User Management");
                Console.WriteLine("1. Search Account");
                Console.WriteLine("2. View All Users");
                Console.WriteLine("3. Create New User");
                Console.WriteLine("4. Unblock Users\n");

                Console.WriteLine("Account and Rates");
                Console.WriteLine("5. View All Accounts");
                Console.WriteLine("6. View All Exchange Rates");
                Console.WriteLine("7. Add or Update Exchange Rates");
                Console.WriteLine("8. Update Savings Rate");
                Console.WriteLine("9. Update Loan Rate\n");

                Console.WriteLine("10. Back");

                var choice = ReadInput("Choose option");
                if (IsBack(choice))
                    return;

                switch (choice)
                {
                    case "1": SearchAccountMenu(); break;
                    case "2": ViewAllUsers(); break;
                    case "3": CreateNewUserUI(); break;
                    case "4": UnlockUserBlocks(); break;
                    case "5": ViewAllAccounts(); break;
                    case "6": ViewAllRates(); break;
                    case "7": AddExchangeRate(); break;
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

            var id = ReadInput("Enter SSN");
            if (IsBack(id)) return;

            var name = ReadInput("Enter name");
            if (IsBack(name)) return;

            var pin = ReadInput("Enter PIN");
            if (IsBack(pin)) return;

            var user = new User(id, name, pin, UserRole.Customer);
            bank.Users.Add(user);

            ConsoleHelper.WriteSuccess("User created.");
            ConsoleHelper.PauseWithMessage();
        }

        private List<User>? _blockedUsers;

        // Gets blocked users
        private List<User> GetBlockedUsers()
        {
            return _blockedUsers ??= searchService.GetAllUsers().Where(u => u.isBlocked).ToList();
        }

        // Unblocks users
        public void UnlockUserBlocks()
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

                foreach (var user in blockedUsers)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"User: {user.Name} (ID: {user.Id}) is blocked");
                }
                Console.ResetColor();

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

                ConsoleHelper.WriteSuccess($"{userToUnlock.Name} unblocked.");
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

            foreach (var u in users)
                Console.WriteLine($"{u.Id} - {u.Name} ({u.Role})");

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

            foreach (var a in accounts)
                Console.WriteLine($"{a.AccountNumber} - {a.Owner.Name} - {a.Balance} {a.Currency}");

            ConsoleHelper.PauseWithMessage();
        }

        // Search menu
        private void SearchAccountMenu()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("SEARCH ACCOUNT");

            Console.WriteLine("1. Search by Account Number");
            Console.WriteLine("2. Search by Username");

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
            var accountNumber = ReadInput("Enter account number");
            if (IsBack(accountNumber)) return;

            var account = searchService.FindAccountByNumber(accountNumber);

            if (account == null)
            {
                ConsoleHelper.WriteWarning("Account not found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            Console.WriteLine($"{account.AccountNumber} - {account.Owner.Name} - {account.Balance} {account.Currency}");
            ConsoleHelper.PauseWithMessage();
        }

        // Search by username
        private void SearchByUsername()
        {
            var username = ReadInput("Enter username");
            if (IsBack(username)) return;

            var accounts = searchService.FindAccountsByUsername(username);

            if (!accounts.Any())
            {
                ConsoleHelper.WriteWarning("No accounts found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            foreach (var a in accounts)
                Console.WriteLine($"{a.AccountNumber} - {a.Owner.Name} - {a.Balance} {a.Currency}");

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

            foreach (var r in rates)
                Console.WriteLine($"{(r.CustomCode ?? r.Code.ToString())} -> {r.Rate}");

            ConsoleHelper.PauseWithMessage();
        }

        // Adds or updates a rate
        private void AddExchangeRate()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("UPDATE/DELETE/CREATE EXCHANGE RATES");

            var rates = searchService.GetAllExchangeRates();

            foreach (var r in rates)
                Console.WriteLine($"{(r.CustomCode ?? r.Code.ToString())} -> {r.Rate}");

            var selected = ReadInput("Enter currency code or type (N) to add a new one");
            if (IsBack(selected)) return;

            if (selected.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                AddNewCurrency();
                return;
            }


            if (!Enum.TryParse(selected, true, out CurrencyCode code))
            {
                ConsoleHelper.WriteWarning("Invalid currency.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            Console.WriteLine($"You chose: {selected}");
            var CrudChoice = ReadInput($"Choose: (D) for delete - (U) to update");
            if (CrudChoice.Equals("U", StringComparison.OrdinalIgnoreCase))
            {
                var rateText = ReadInput($"Enter new rate for {selected}");
                if (IsBack(rateText)) return;

                if (!decimal.TryParse(rateText, out var newRate))
                {
                    ConsoleHelper.WriteWarning("Invalid rate.");
                    ConsoleHelper.PauseWithMessage();
                    return;
                }

                exchangerateService.AddRates(new ExchangeRate(code, newRate));
                ConsoleHelper.WriteSuccess("Rate updated.");
                ConsoleHelper.PauseWithMessage();
            }
            else if (CrudChoice.Equals("D", StringComparison.OrdinalIgnoreCase))
            {
                var rateText = ReadInput($"Are you sure? (Y) / (N) ");
                if (IsBack(rateText)) return;
                if(rateText.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    exchangerateService.DeleteField(code);
                }

            }



        }

        // Adds a completely new currency
        private void AddNewCurrency()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ADD NEW CURRENCY");

            var code = ReadInput("Enter currency code");
            if (IsBack(code)) return;

            var rateText = ReadInput("Enter rate");
            if (IsBack(rateText)) return;

            if (!decimal.TryParse(rateText, out var rate))
            {
                ConsoleHelper.WriteWarning("Invalid rate.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            var newRate = new ExchangeRate
            {
                Code = 0,
                CustomCode = code,
                Rate = rate,
                LastUpdated = DateTime.Now
            };

            exchangerateService.AddRates(newRate);

            ConsoleHelper.WriteSuccess("Currency added.");
            ConsoleHelper.PauseWithMessage();
        }

        // Updates savings interest rate
        private void UpdateSavingsInterestRate()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("UPDATE SAVINGS RATE");

            Console.WriteLine($"Current: {bank.DefaultSavingsInterestRate}%");

            var input = ReadInput("Enter new rate");
            if (IsBack(input)) return;

            if (!decimal.TryParse(input, out var newRate))
            {
                ConsoleHelper.WriteWarning("Invalid rate.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            bank.UpdateDefaultSavingsRate(newRate);
            ConsoleHelper.WriteSuccess("Savings rate updated.");
            ConsoleHelper.PauseWithMessage();
        }

        // Updates loan interest rate
        private void UpdateLoanInterestRate()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("UPDATE LOAN RATE");

            Console.WriteLine($"Current: {LoanService.CurrentLoanInterestRate}%");

            var input = ReadInput("Enter new rate");
            if (IsBack(input)) return;

            if (!decimal.TryParse(input, out var newRate))
            {
                ConsoleHelper.WriteWarning("Invalid rate.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            LoanService.SetLoanInterestRate(newRate);
            ConsoleHelper.WriteSuccess("Loan rate updated.");
            ConsoleHelper.PauseWithMessage();
        }
    }
}
