using bank.Core;
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

        public void CalculateInterest(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== CALCULATE INTEREST ===\n");

            if (currentUser == null)
            {
                Console.WriteLine("You must be logged in to use this feature.\n");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter your savings account number: ");
            string accountNumber = Console.ReadLine();

            var savings = bank.Accounts
                .OfType<SavingsAccount>()
                .FirstOrDefault(a => a.AccountNumber.Equals(accountNumber, StringComparison.OrdinalIgnoreCase)
                                  && a.Owner == currentUser);

            if (savings == null)
            {
                Console.WriteLine("No savings account found for this user.\n");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter annual interest rate (%): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal rate))
            {
                Console.WriteLine("Invalid input.\n");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter number of months: ");
            if (!int.TryParse(Console.ReadLine(), out int months))
            {
                Console.WriteLine("Invalid input.\n");
                Console.ReadKey();
                return;
            }

            // Beräkna framtida saldo via SavingsAccount-logiken
            savings.CalculateFutureBalance(rate, months);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
