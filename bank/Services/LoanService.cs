using bank.Core;
using bank.Utils;
using System;
using System.Linq;

namespace bank.Services
{
    public class LoanService
    {
        private readonly Bank bank;
        private readonly ExchangerateService exchangerateService;

        // Statisk egenskap för aktuell låneränta (kan ändras från AdminService)
        public static decimal CurrentLoanInterestRate { get; private set; } = 5.5m;
        public static decimal LoanBaseInterestRate { get; private set; } = 5.5m;

        public static void UpdateLoanBaseRate(decimal newRate)
        {
            if (newRate > 0 && newRate < 20)
                LoanBaseInterestRate = newRate;
            else
                Console.WriteLine("Interest rate must be between 0 and 20%");
        }

        public LoanService(Bank bank)
        {
            this.bank = bank;
            this.exchangerateService = new ExchangerateService();
        }

        // Anropas från AdminService för att uppdatera låneräntan
        public static void SetLoanInterestRate(decimal newRate)
        {
            if (newRate > 0 && newRate < 20)
            {
                CurrentLoanInterestRate = newRate;
            }
            else
            {
                Console.WriteLine("Interest rate must be between 0 and 20%.");
            }
        }

        public void ApplyForLoan(User currentUser)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("APPLY FOR LOAN");

            if (currentUser.Accounts == null || !currentUser.Accounts.Any())
            {
                ConsoleHelper.WriteWarning("You have no accounts. Cannot apply for a loan.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // Summera alla konton i SEK
            decimal totalBalanceSek = 0;
            foreach (var acc in currentUser.Accounts)
                totalBalanceSek += exchangerateService.ConvertToSek(acc.Currency, acc.Balance);

            decimal maxLoan = totalBalanceSek * 5;

            ConsoleHelper.WriteInfo($"Your total balance: {totalBalanceSek:N2} SEK");
            ConsoleHelper.WriteInfo($"Maximum allowed loan (x5): {maxLoan:N2} SEK");
            Console.WriteLine();

            var amountInput = ConsoleHelper.PromptWithEscape("How much would you like to borrow (SEK)");
            if (amountInput == "<ESC>")
                return;

            if (!decimal.TryParse(amountInput, out var amount) || amount <= 0)
            {
                ConsoleHelper.WriteError("Invalid amount.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // 2. Kontrollera att det inte överskrider maxgränsen
            if (amount > maxLoan)
            {
                ConsoleHelper.WriteError("Loan denied. Requested amount exceeds your limit.");
                ConsoleHelper.WriteWarning("You can only borrow up to 5x your total account balance.");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // 3. Fråga sedan om antal månader
            var monthsInput = ConsoleHelper.PromptWithEscape("Over how many months would you like to repay the loan");
            if (monthsInput == "<ESC>")
                return;

            if (!int.TryParse(monthsInput, out var months) || months <= 0 || months > 300)
            {
                ConsoleHelper.WriteError("Invalid number of months (1-300).");
                ConsoleHelper.PauseWithMessage();
                return;
            }

            // 4. När allt är ifyllt, räkna ut räntan i bakgrunden (döljs från användaren)
            decimal baseRate = LoanBaseInterestRate;
            decimal monthlyRateIncrease = 0.1m;

            decimal effectiveRate = baseRate;
            if (months > 12)
                effectiveRate += (months - 12) * monthlyRateIncrease;

            // 5. Gör själva beräkningen och visa resultatet
            decimal interestAmount = Math.Round(amount * (effectiveRate / 100m) * (months / 12m), 2);
            decimal totalRepayment = amount + interestAmount;

            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("LOAN SUMMARY");

            ConsoleHelper.WriteInfo($"Loan amount: {amount:N2} SEK");
            ConsoleHelper.WriteInfo($"Repayment period: {months} months");
            ConsoleHelper.WriteInfo($"Interest rate: {effectiveRate:N2}%");
            ConsoleHelper.WriteWarning($"Interest to pay: {interestAmount:N2} SEK");
            ConsoleHelper.WriteSuccess($"Total repayment: {totalRepayment:N2} SEK");

            ConsoleHelper.PauseWithMessage();
        }
    }
}
