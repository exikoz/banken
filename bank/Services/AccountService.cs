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
                Console.WriteLine("\nWould you like to create one? (Press 'y' for yes)");
                if (Console.ReadKey(true).KeyChar == 'y')
                {
                    CreateAccount(currentUser);
                    return;
                }
            }
            else
            {
                foreach (var account in currentUser.Accounts)
                {
                    Console.WriteLine($"Account: {account.AccountNumber}");
                    Console.WriteLine($"Balance: {account.Balance:C}");
                    Console.WriteLine("─────────────────────");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Creates a new account for the user
        /// </summary>
        public void CreateAccount(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CREATE NEW ACCOUNT ===\n");

            Console.Write("Enter account number (e.g., ACC123): ");
            var accountNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                Console.WriteLine("\n✗ Account number cannot be empty!");
                Console.ReadKey();
                return;
            }

            if (bank.FindAccount(accountNumber) != null)
            {
                Console.WriteLine("\n✗ Account number already exists!");
                Console.ReadKey();
                return;
            }

            bank.OpenAccount(currentUser, accountNumber);
            Console.WriteLine($"\n✓ Account {accountNumber} created successfully!");
            Console.ReadKey();
        }

        /// <summary>
        /// Handles deposit operation
        /// </summary>
        public void Deposit(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== DEPOSIT MONEY ===\n");

            var account = SelectAccount(currentUser);
            if (account == null) return;

            Console.Write("Amount to deposit: ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                account.Deposit(amount);
            }
            else
            {
                Console.WriteLine("\n✗ Invalid amount!");
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Handles withdrawal operation
        /// </summary>
        public void Withdraw(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== WITHDRAW MONEY ===\n");

            var account = SelectAccount(currentUser);
            if (account == null) return;

            Console.Write("Amount to withdraw: ");
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

        /// <summary>
        /// Allows user to select an account from their list
        /// </summary>
        private Account? SelectAccount(User currentUser)
        {
            if (!currentUser.Accounts.Any())
            {
                Console.WriteLine("You do not have any accounts!");
                Console.ReadKey();
                return null;
            }

            if (currentUser.Accounts.Count == 1)
            {
                Console.WriteLine($"Using account: {currentUser.Accounts[0].AccountNumber}\n");
                return currentUser.Accounts[0];
            }

            Console.WriteLine("Select account:");
            for (int i = 0; i < currentUser.Accounts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {currentUser.Accounts[i].AccountNumber} - Balance: {currentUser.Accounts[i].Balance:C}");
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