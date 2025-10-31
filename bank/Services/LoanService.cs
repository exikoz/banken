using bank.Core;
using System;
using System.Linq;

namespace bank.Services
{
    public class LoanService
    {
        private readonly Bank bank;

        public LoanService(Bank bank)
        {
            this.bank = bank;
        }

        // Låter användaren ansöka om ett nytt lån
        public void ApplyForLoan(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== APPLY FOR LOAN ===\n");

            // Kontrollera att användaren har konton
            if (currentUser.Accounts == null || !currentUser.Accounts.Any())
            {
                Console.WriteLine("✗ You have no accounts. Cannot apply for a loan.");
                Console.ReadKey();
                return;
            }

            // Räkna ut användarens totala balans
            decimal totalBalance = currentUser.Accounts.Sum(a => a.Balance);
            decimal maxLoan = totalBalance * 5;

            var currency = currentUser.Accounts.First().Currency;
            Console.WriteLine($"Your total balance: {totalBalance} {currency}");
            Console.WriteLine($"Maximum allowed loan (x5): {maxLoan} {currency}\n");


            Console.Write("Enter desired amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("\n✗ Invalid amount.");
                Console.ReadKey();
                return;
            }

            // Kontrollera om användaren försöker låna för mycket
            if (amount > maxLoan)
            {
                Console.WriteLine("\n✗ Loan denied. Requested amount exceeds your limit.");
                Console.WriteLine("You can only borrow up to 5x your total account balance.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter number of months for repayment: ");
            if (!int.TryParse(Console.ReadLine(), out var months) || months <= 0)
            {
                Console.WriteLine("\n✗ Invalid number of months.");
                Console.ReadKey();
                return;
            }

            // Enkel fast ränta
            decimal interestRate = 5.5m;
            decimal totalRepayment = amount + (amount * (interestRate / 100) * (months / 12m));

            Console.WriteLine($"\n✓ Loan approved!");
            Console.WriteLine($"Amount: {amount} {currency}");
            Console.WriteLine($"Total loan + interest: {totalRepayment} {currency}");
            Console.WriteLine($"Months: {months}");
            Console.WriteLine($"Total repayment: {totalRepayment} {currency}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
