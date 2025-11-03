using bank.Core;
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
            Console.Clear();
            Console.WriteLine("=== APPLY FOR LOAN ===\n");

            if (currentUser.Accounts == null || !currentUser.Accounts.Any())
            {
                Console.WriteLine("You have no accounts. Cannot apply for a loan.");
                Console.ReadKey();
                return;
            }

            // Summera alla konton i SEK
            decimal totalBalanceSek = 0;
            foreach (var acc in currentUser.Accounts)
                totalBalanceSek += exchangerateService.ConvertToSek(acc.Currency, acc.Balance);

            decimal maxLoan = totalBalanceSek * 5;;

            Console.WriteLine($"Your total balance: {totalBalanceSek:N2} SEK");
            Console.WriteLine($"Maximum allowed loan (x5): {maxLoan:N2} SEK\n");

            // 1. Fråga först hur mycket användaren vill låna
            Console.Write("How much would you like to borrow (SEK): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("\nInvalid amount.");
                Console.ReadKey();
                return;
            }

            // 2. Kontrollera att det inte överskrider maxgränsen
            if (amount > maxLoan)
            {
                Console.WriteLine("\nLoan denied. Requested amount exceeds your limit.");
                Console.WriteLine("You can only borrow up to 5x your total account balance.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            // 3. Fråga sedan om antal månader
            Console.Write("Over how many months would you like to repay the loan: ");
            if (!int.TryParse(Console.ReadLine(), out var months) || months <= 0)
            {
                Console.WriteLine("\nInvalid number of months.");
                Console.ReadKey();
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

            Console.WriteLine("\nLoan summary:\n");
            Console.WriteLine($"Loan amount: {amount:N2} SEK");
            Console.WriteLine($"Repayment period: {months} months");
            Console.WriteLine($"Interest rate: {effectiveRate:N2}%");
            Console.WriteLine($"Interest to pay: {interestAmount:N2} SEK");
            Console.WriteLine($"Total repayment: {totalRepayment:N2} SEK");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

        }
    }
}
