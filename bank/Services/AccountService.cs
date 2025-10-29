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
                    // Visa kontonummer och typ (Savings / Checking)
                    string accountType = account is CheckingAccount ? "Checking Account" :
                                         account is SavingsAccount ? "Savings Account" :
                                         "Unknown Type";

                    //Show Available incl. overdraft up to 1,000 for both types
                    decimal overdraft = 1000m;
                    decimal available = account.Balance + overdraft; 

                    Console.WriteLine($"Account: {account.AccountNumber} ({accountType})");
                    Console.WriteLine($"Balance: {account.Balance:C}  |  Available: {available:C} (includes overdraft up to 1,000)"); // CHANGE
                    Console.WriteLine("─────────────────────");
                }

            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        // Skapar nytt konto och låter användaren välja kontotyp
        public void CreateAccount(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CREATE NEW ACCOUNT ===\n");

            // Kontonummer skapas automatiskt i Bank.OpenAccount()
            // Ingen manuell inmatning krävs längre.


            // Let the user choose account type
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

            // Determine account type based on input
            string accountType = choice == "1" ? "savings" : "checking";

            // Create the account using the selected type
            var newAccount = bank.OpenAccount(currentUser, accountType);
            Console.WriteLine($"\n✓ {accountType.ToUpper()} account {newAccount.AccountNumber} created successfully!");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

        }

        // Handles deposit operation
        public void Deposit(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== DEPOSIT MONEY ===\n");

            var account = SelectAccount(currentUser);
            if (account == null) return;

            Console.Write("Amount to deposit: ");
            if (decimal.TryParse(Console.ReadLine(), out var amount))
            {
                if (amount <= 0)
                {
                    Console.WriteLine("\n✗ Deposit amount must be greater than zero!");
                }
                else
                {
                    account.Deposit(amount);
                    Console.WriteLine($"\n✓ Successfully deposited {amount:C} to account {account.AccountNumber}.");
                }
            }
            else
            {
                Console.WriteLine("\n✗ Invalid input — please enter a number.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }



        /// Handles withdrawal operation
        public void Withdraw(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== WITHDRAW MONEY ===\n");

            var account = SelectAccount(currentUser);
            if (account == null) return;

      
            Console.WriteLine("(Info) You can withdraw up to your balance + 1,000.00 (overdraft)."); 

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
                Console.WriteLine("You do not have any accounts yet.");
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
                //For all accounts show Available (balance + 1,000 overdraft)
                var acc = currentUser.Accounts[i]; 
                decimal available = acc.Balance + 1000m;
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} - Balance: {acc.Balance:C} | Available: {available:C}"); // CHANGE
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
