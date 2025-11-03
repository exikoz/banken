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

            decimal maxLoan = totalBalanceSek * 5;

            Console.WriteLine($"Your total balance: {totalBalanceSek:N2} SEK");
            Console.WriteLine($"Maximum allowed loan (x5): {maxLoan:N2} SEK\n");

            Console.Write("Enter desired amount (SEK): ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("\nInvalid amount.");
                Console.ReadKey();
                return;
            }

            if (amount > maxLoan)
            {
                Console.WriteLine("\nLoan denied. Requested amount exceeds your limit.");
                Console.WriteLine("You can only borrow up to 5x your total account balance.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter number of months for repayment: ");
            if (!int.TryParse(Console.ReadLine(), out var months) || months <= 0)
            {
                Console.WriteLine("\nInvalid number of months.");
                Console.ReadKey();
                return;
            }

            // Använd den aktuella räntan
            decimal interestRate = CurrentLoanInterestRate;
            decimal interestAmount = Math.Round(amount * (interestRate / 100m) * (months / 12m), 2);
            decimal totalRepayment = amount + interestAmount;

            Console.WriteLine("\nLoan summary:");
            Console.WriteLine($"Loan amount (principal): {amount:N2} SEK");
            Console.WriteLine($"Interest rate: {interestRate}% for {months} months");
            Console.WriteLine($"Interest to pay: {interestAmount:N2} SEK");
            Console.WriteLine($"Total repayment: {totalRepayment:N2} SEK");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
