using bank.Core;
using bank.Utils;
using System;
using System.Linq;

namespace bank.Services
{
    public class AccountService
    {
        private readonly Bank bank;
        private readonly ExchangerateService exchangerateService;

        public AccountService(Bank bank)
        {
            this.bank = bank;
            this.exchangerateService = new ExchangerateService();
        }

        // Shows all accounts for a user
        public void ShowAccounts(User user)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("MY ACCOUNTS");

            if (!user.Accounts.Any())
            {
                ConsoleHelper.WriteWarning("You have no accounts.");
                var answer = ConsoleHelper.PromptWithEscape("Open a new account? (yes/no)");

                if (answer == "<ESC>" || string.IsNullOrWhiteSpace(answer))
                    return;

                answer = answer.ToLower();

                if (answer == "yes" || answer == "y")
                {
                    CreateAccount(user);
                    return;
                }

                return;
            }

            foreach (var acc in user.Accounts)
            {
                var type = acc is CheckingAccount ? "Checking" :
                           acc is SavingsAccount ? "Savings" : "Account";

                Console.WriteLine($"{acc.AccountNumber} - {type}");
                Console.WriteLine($"Balance: {acc.Balance} {acc.Currency}");
                Console.WriteLine();
            }

            decimal totalSek = 0;
            foreach (var acc in user.Accounts)
                totalSek += exchangerateService.ConvertToSek(acc.Currency, acc.Balance);

            Console.WriteLine($"Total balance (SEK): {totalSek:N2}");

            var back = ConsoleHelper.PromptWithEscape("Press ENTER or ESC to go back");
            if (back == "<ESC>" || string.IsNullOrWhiteSpace(back))
                return;
        }

        // Creates a new account
        public void CreateAccount(User user)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("CREATE ACCOUNT");

            Console.WriteLine("1. Savings");
            Console.WriteLine("2. Checking");

            var choice = ConsoleHelper.PromptWithEscape("Choose option");
            if (choice == "<ESC>" || string.IsNullOrWhiteSpace(choice))
                return;

            if (choice != "1" && choice != "2")
            {
                ConsoleHelper.WriteError("Invalid choice.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            var type = choice == "1" ? "savings" : "checking";

            bank.OpenAccount(user, type);

            ConsoleHelper.WriteSuccess("Account created.");
            ConsoleHelper.PauseWithMessage();
        }

        // Deposit money
        public void Deposit(User currentUser)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("DEPOSIT");

            var account = SelectAccount(currentUser);
            if (account == null)
                return;

            Console.Write($"Amount ({account.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                ConsoleHelper.WriteError("Invalid amount.");
                Console.ReadKey();
                return;
            }

            decimal before = account.Balance;

            account.Deposit(amount);

            if (account.Balance == before)
            {
                ConsoleHelper.WriteError("Deposit failed.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            ConsoleHelper.WriteSuccess("Deposit completed.");
            Console.WriteLine($"New balance: {account.Balance:N2} {account.Currency}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // Withdraw money
        public void Withdraw(User currentUser)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("WITHDRAW");

            var account = SelectAccount(currentUser);
            if (account == null)
                return;

            Console.Write($"Amount ({account.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                ConsoleHelper.WriteError("Invalid amount.");
                Console.ReadKey();
                return;
            }

            // Store before balance to compare result
            decimal before = account.Balance;

            // Perform withdraw
            account.Withdraw(amount);

            // If no change → fail
            if (account.Balance == before)
            {
                ConsoleHelper.WriteError("Withdraw failed. Check balance or limits.");
                Console.ReadKey();
                return;
            }

            // Success message (unified style)
            Console.WriteLine();
            ConsoleHelper.WriteSuccess($"Withdraw completed.");
            Console.WriteLine($"New balance: {account.Balance:N2} {account.Currency}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        // Lets user pick which account to use
        private Account? SelectAccount(User user)
        {
            if (!user.Accounts.Any())
            {
                ConsoleHelper.WriteWarning("You have no accounts.");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            if (user.Accounts.Count == 1)
            {
                var acc = user.Accounts[0];
                ConsoleHelper.WriteInfo($"Using: {acc.AccountNumber} ({acc.Currency})");
                return acc;
            }

            Console.WriteLine("Select account:");

            for (int i = 0; i < user.Accounts.Count; i++)
            {
                var acc = user.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} - {acc.Balance} {acc.Currency}");
            }

            var choice = ConsoleHelper.PromptWithEscape("Choose");
            if (choice == "<ESC>" || string.IsNullOrWhiteSpace(choice))
                return null;

            if (int.TryParse(choice, out var index) &&
                index >= 1 &&
                index <= user.Accounts.Count)
            {
                return user.Accounts[index - 1];
            }

            ConsoleHelper.WriteError("Invalid selection.");
            ConsoleHelper.PauseWithMessage();
            return null;
        }
    }
}
