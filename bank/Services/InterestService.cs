using bank.Core;
using bank.Utils;
using System;
using System.Linq;

namespace bank.Services
{
    public class InterestService
    {
        private readonly Bank bank;

        public InterestService(Bank bank)
        {
            this.bank = bank;
        }

        // Entry point for interest calculation
        public void CalculateInterest(User currentUser)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("CALCULATE INTEREST");

            if (currentUser == null)
            {
                ConsoleHelper.WriteError("You must be logged in.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // List savings accounts with balance > 0
            var savingsAccounts = bank.Accounts
                .OfType<SavingsAccount>()
                .Where(a => a.Owner == currentUser && a.Balance > 0)
                .ToList();

            if (!savingsAccounts.Any())
            {
                ConsoleHelper.WriteWarning("No savings accounts with positive balance found.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Display account list
            ConsoleHelper.WriteInfo("Select a savings account (ESC to go back):");
            Console.WriteLine();

            for (int i = 0; i < savingsAccounts.Count; i++)
            {
                var acc = savingsAccounts[i];
                ConsoleHelper.WriteMenuOption($"{i + 1}", $"{acc.AccountNumber} - {acc.Balance:N2} {acc.Currency}");
            }
            Console.WriteLine();

            // ESC support
            var input = ConsoleHelper.PromptWithEscape("Choose account");

            if (input == "<ESC>")
                return;

            if (!int.TryParse(input, out int accountChoice) ||
                accountChoice < 1 || accountChoice > savingsAccounts.Count)
            {
                ConsoleHelper.WriteError("Invalid selection.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            var selectedAccount = savingsAccounts[accountChoice - 1];

            // Ask for months
            var monthsInput = ConsoleHelper.PromptWithEscape("Enter number of months");

            if (monthsInput == "<ESC>")
                return;

            if (!int.TryParse(monthsInput, out int months) || months <= 0)
            {
                ConsoleHelper.WriteError("Invalid number.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Calculate effective rate
            decimal bankRate = bank.DefaultSavingsInterestRate;
            decimal effectiveRate = bankRate;

            if (months > 12)
                effectiveRate += (months - 12) * 0.1m;

            // Calculate future balance
            decimal futureBalance = CalculateFutureBalance(selectedAccount, effectiveRate, months);
            decimal interestEarned = futureBalance - selectedAccount.Balance;

            Console.WriteLine();
            ConsoleHelper.WriteInfo($"Interest rate: {effectiveRate:N2}%");
            ConsoleHelper.WriteSuccess($"Interest earned: {interestEarned:N2} {selectedAccount.Currency}");
            ConsoleHelper.WriteSuccess($"Future balance after {months} months: {futureBalance:N2} {selectedAccount.Currency}");



            ConsoleHelper.PauseWithMessage();
        }

        // Pure calculation method
        public decimal CalculateFutureBalance(Account account, decimal annualInterestRate, int months)
        {
            if (annualInterestRate < 0 || months < 0)
                return account.Balance;

            decimal effectiveRate = annualInterestRate;

            if (months > 12)
                effectiveRate += (months - 12) * 0.1m;

            decimal interest = Math.Round(account.Balance * (effectiveRate / 100m) * (months / 12m), 2);

            return account.Balance + interest;
        }
    }
}
