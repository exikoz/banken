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

            // Lista alla sparkkonton för användaren som har saldo >0
            var savingsAccounts = bank.Accounts
                .OfType<SavingsAccount>()
                .Where(a => a.Owner == currentUser && a.Balance > 0)
                .ToList();


            if (!savingsAccounts.Any())
            {
                Console.WriteLine("No savings accounts with positive balance found.\n");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Select a savings account:");
            for (int i = 0; i < savingsAccounts.Count; i++)
            {
                var acc = savingsAccounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} - {acc.Balance:N2} {acc.Currency}");
            }

            Console.Write("\nChoose account (1–{0}): ", savingsAccounts.Count);
            if (!int.TryParse(Console.ReadLine(), out int accountChoice) ||
                accountChoice < 1 || accountChoice > savingsAccounts.Count)
            {
                Console.WriteLine("Invalid selection.\n");
                Console.ReadKey();
                return;
            }

            var selectedAccount = savingsAccounts[accountChoice - 1];

            // Använd bankens standardränta
            decimal bankRate = bank.DefaultSavingsInterestRate;
            Console.WriteLine("\nEnter period details below:");

            // Fråga efter antal månader
            Console.Write("Enter number of months: ");
            if (!int.TryParse(Console.ReadLine(), out int months) || months <= 0)
            {
                Console.WriteLine("\nInvalid input.\n");
                Console.ReadKey();
                return;
            }

            // Dynamisk ränta: basränta gäller för 1–12 månader, sedan +0.1 % per extra månad
            decimal effectiveRate = bankRate;
            decimal monthlyRateIncrease = 0.1m;

            if (months > 12)
                effectiveRate += (months - 12) * monthlyRateIncrease;

            // Kör beräkningen med den dynamiska räntan
            selectedAccount.CalculateFutureBalance(effectiveRate, months);


            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

    }
}
