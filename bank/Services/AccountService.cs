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
                Console.WriteLine($"Balance: {acc.Balance:N2} {acc.Currency}");
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

            Console.Write($"Amount ({account.Currency}): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                ConsoleHelper.WriteError("Invalid amount.");
                Console.ReadKey();
                return;
            }

            // Deposit happens instantly
            account.Balance += amount;

            // Create completed transaction
            var tx = new Transaction(
                id: Bank.GenerateTransactionId(),
                accountNumber: account.AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Deposit",
                amount: amount
            )
            {
                Currency = account.Currency,
                FromAccount = "0000",
                ToAccount = account.AccountNumber,
                FromUser = "ATM",
                ToUser = currentUser.Name,
                IsPending = false,
                Status = "Completed"
            };

            account.Transactions.Add(tx);

            ConsoleHelper.WriteSuccess($"Deposit completed: {amount} {account.Currency}");
            ConsoleHelper.WriteSuccess($"New balance: {account.Balance:N2} {account.Currency}");
            ConsoleHelper.PauseWithMessage();
        }

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

            if (!CanWithdraw(account, amount))
            {
                Console.ReadKey();
                return;
            }

            // Withdraw happens instantly
            account.Balance -= amount;

            var tx = new Transaction(
                id: Bank.GenerateTransactionId(),
                accountNumber: account.AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Withdraw",
                amount: amount
            )
            {
                Currency = account.Currency,
                FromAccount = account.AccountNumber,
                ToAccount = "0000",
                FromUser = currentUser.Name,
                ToUser = "ATM",
                IsPending = false,
                Status = "Completed"
            };

            account.Transactions.Add(tx);

            ConsoleHelper.WriteSuccess($"Withdrawal completed: {amount} {account.Currency}");
            ConsoleHelper.WriteSuccess($"New balance: {account.Balance:N2} {account.Currency}");
            ConsoleHelper.PauseWithMessage();
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