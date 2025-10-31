using bank.Core;
using System;
using System.Linq;

namespace bank.Services
{
    /// <summary>
    /// Handles all account-related operations (create, view, deposit, withdraw)
    /// </summary>
    public class AccountService
    {
        private readonly Bank bank;

        public AccountService(Bank bank)
        {
            this.bank = bank;
        }

        /// <summary>
        /// Displays all accounts for the current user
        /// </summary>
        public void ShowAccounts(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== MY ACCOUNTS ===\n");

            if (!currentUser.Accounts.Any())
            {
                Console.WriteLine("You do not have any accounts yet.");
                Console.Write("\nWould you like to open one? (Yes/No): ");

                string? answer = Console.ReadLine()?.Trim().ToLower();

                if (answer == "y" || answer == "yes")
                {
                    CreateAccount(currentUser);
                    return;
                }
                else if (answer == "n" || answer == "no")
                {
                    Console.WriteLine("\nReturning to main menu...");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    Console.WriteLine("\nInvalid choice. Please type Yes or No.");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                foreach (var account in currentUser.Accounts)
                {
                    string accountType = account is CheckingAccount ? "Checking Account" :
                                         account is SavingsAccount ? "Savings Account" :
                                         "Unknown Type";

                    //Show Available incl. overdraft up to 1,000 for both types
                    decimal overdraft = 1000m;
                    decimal available = account.Balance + overdraft; 

                    Console.WriteLine($"Account: {account.AccountNumber} ({accountType})");
                    Console.WriteLine($"Balance: {account.Balance} {account.Currency}");
                    Console.WriteLine("─────────────────────");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // Create new account
        public void CreateAccount(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CREATE NEW ACCOUNT ===\n");

            Console.WriteLine("\nSelect account type:");
            Console.WriteLine("1. Savings Account");
            Console.WriteLine("2. Checking Account");
            Console.Write("\nChoose option: ");
            var choice = Console.ReadLine();

            if (choice != "1" && choice != "2")
            {
                Console.WriteLine("\n✗ Invalid choice. Account not created.");
                Console.ReadKey();
                return;
            }

            string accountType = choice == "1" ? "savings" : "checking";

            var newAccount = bank.OpenAccount(currentUser, accountType);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // Deposit money
        public void Deposit(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== DEPOSIT MONEY ===\n");

            var account = SelectAccount(currentUser);
            if (account == null) return;

            Console.Write($"Amount to deposit ({account.Currency}): ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                if (amount <= 0)
                {
                    Console.WriteLine("\n✗ Deposit amount must be greater than zero!");
                }
                else
                {
                    account.Deposit(amount);
                }
            }
            else
            {
                Console.WriteLine("\n✗ Invalid input — please enter a number.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // Withdraw money
        public void Withdraw(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== WITHDRAW MONEY ===\n");

            var account = SelectAccount(currentUser);
            if (account == null) return;

            Console.Write($"Amount to withdraw ({account.Currency}): ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                
                account.Withdraw(amount); 
            }
            else
            {
                Console.WriteLine("\n✗ Invalid amount!");
            }

            Console.ReadKey();
        }

        // Account selector
        private Account? SelectAccount(User currentUser)
        {
            if (!currentUser.Accounts.Any())
            {
                Console.WriteLine("You do not have any accounts yet.");
                Console.ReadKey();
                return null;
            }

            if (currentUser.Accounts.Count == 1)
            {
                var acc = currentUser.Accounts[0];
                Console.WriteLine($"Using account: {acc.AccountNumber} ({acc.Currency})\n");
                return acc;
            }

            Console.WriteLine("Select account:");
            for (int i = 0; i < currentUser.Accounts.Count; i++)
            {
                var acc = currentUser.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} - Balance: {acc.Balance} {acc.Currency}");
            }

            Console.Write("\nSelect: ");
            if (int.TryParse(Console.ReadLine(), out var choice) &&
                choice > 0 && choice <= currentUser.Accounts.Count)
            {
                return currentUser.Accounts[choice - 1];
            }

            Console.WriteLine("\n✗ Invalid selection!");
            Console.ReadKey();
            return null;
        }
    }
}
