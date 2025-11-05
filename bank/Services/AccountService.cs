using bank.Core;
using bank.Utils;
using System;
using System.Linq;
using System.Threading;

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

        public void Deposit(User currentUser)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("DEPOSIT");


            var account = SelectAccount(currentUser);
            if (account == null)
                return;

            if (!account.canTransfer)
            {
                ConsoleHelper.WriteError("There are pending transactions on your account. Please wait until they have finished processing.");
                Console.ReadKey();
                return;
            }


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
                return;
            }

            ConsoleHelper.WriteSuccess($"Deposit processed at {DateTime.UtcNow}." +
                $" \n Reserved amount: {amount} will be processed in 15 minutes. \n ");
            ConsoleHelper.WriteInfo("Further transactions will be locked until the transaction is processed");
            Console.WriteLine($"New reserved balance: {account.Balance:N2} {account.Currency}");




            account.canTransfer = false;

            Thread backgroundThread = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));

                }
                finally
                {
                    account.canTransfer = true;
                }
            });
            backgroundThread.IsBackground = true;
            backgroundThread.Start();

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public void Withdraw(User currentUser)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("WITHDRAW");

            var account = SelectAccount(currentUser);
            if (account == null)
                return;


            if (!account.canTransfer)
            {
                ConsoleHelper.WriteError("There are pending transactions on your account. Please wait until they have finished processing.");
                Console.ReadKey();
                return;
            }

            Console.Write($"Amount ({account.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                ConsoleHelper.WriteError("Invalid amount.");
                Console.ReadKey();
                return;
            }

            if (!CanWithdraw(account, amount))
            {
                Console.ReadKey();
                return;
            }




            decimal before = account.Balance;
            account.Withdraw(amount);
            if (account.Balance == before)
            {
                ConsoleHelper.WriteError("Withdrawal failed.");
            }


            ConsoleHelper.WriteSuccess($"Withdrawal processed at {DateTime.UtcNow}." +
                $" \n Reserved amount: {amount} will be processed in 15 minutes. \n ");
            ConsoleHelper.WriteInfo("Further transactions will be locked until the transaction is processed");
            Console.WriteLine($"New reserved balance: {account.Balance:N2} {account.Currency}");

            account.canTransfer = false;
            Thread backgroundThread = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
                finally
                {
                    account.canTransfer = true;
                }
            });
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public bool CanWithdraw(Account account, decimal amount)
        {
            if (amount <= 0)
            {
                ConsoleHelper.WriteError("Insufficient funds for this withdrawal.");
                return false;

            }


            if (account is SavingsAccount savings)
            {
                decimal total = amount;
                if (savings.FreeWithdrawalsLeft <= 0)
                    total += savings.WithdrawFee;

                return account.Balance >= total;
            }

            return account.Balance >= amount;
        }

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
        public bool isLocked(Account acc)
        {
            return acc.canTransfer;
        }
    }
}