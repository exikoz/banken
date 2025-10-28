using bank.Core;
using System;

namespace bank.Services
{
    public class LoanService
    {
        private readonly Bank bank;

        public LoanService(Bank bank)
        {
            this.bank = bank;
        }

        public void OfferLoanUI(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== LOAN SIMULATION ===\n");

            if (currentUser == null)
            {
                Console.WriteLine("You must be logged in to use this feature.");
                Console.ReadKey();
                return;
            }

            Console.Write("How much do you want to borrow (kr): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal loanAmount) || loanAmount <= 0)
            {
                Console.WriteLine("\nInvalid loan amount.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter annual interest rate in % (example: 6): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal annualRate) || annualRate <= 0)
            {
                Console.WriteLine("\nInvalid interest rate.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter loan duration in years: ");
            if (!int.TryParse(Console.ReadLine(), out int years) || years <= 0)
            {
                Console.WriteLine("\nInvalid duration.");
                Console.ReadKey();
                return;
            }

            decimal totalToRepay = CalculateTotalLoanCost(loanAmount, annualRate, years);
            decimal totalInterest = totalToRepay - loanAmount;

            Console.WriteLine("\n-------------------------------------------");
            Console.WriteLine($"Requested loan:      {loanAmount:C}");
            Console.WriteLine($"Interest rate:       {annualRate}%");
            Console.WriteLine($"Duration:            {years} year(s)");
            Console.WriteLine($"Total interest cost: {totalInterest:C}");
            Console.WriteLine($"Total to repay:      {totalToRepay:C}");
            Console.WriteLine("-------------------------------------------");

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private decimal CalculateTotalLoanCost(decimal principal, decimal annualRatePercent, int years)
        {
            decimal rate = annualRatePercent / 100m;
            decimal total = principal * (decimal)Math.Pow((double)(1 + rate), years);
            return decimal.Round(total, 2);
        }
    }
}
