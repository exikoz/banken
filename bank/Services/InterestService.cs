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
            Console.WriteLine("=== CALCULATE SAVINGS INTEREST ===\n");

            if (currentUser == null)
            {
                Console.WriteLine("You must be logged in to use this feature.\n");
                Console.ReadKey();
                return;
            }

            // Filtrera användarens sparkonton
            var savingsAccounts = bank.Accounts
                .OfType<SavingsAccount>()
                .Where(a => a.Owner == currentUser)
                .ToList();

            if (!savingsAccounts.Any())
            {
                Console.WriteLine("You don’t have any savings accounts yet.\n");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Select a savings account:");
            for (int i = 0; i < savingsAccounts.Count; i++)
            {
                var acc = savingsAccounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
            }

            Console.Write($"\nEnter number (1–{savingsAccounts.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > savingsAccounts.Count)
            {
                Console.WriteLine("\nInvalid choice.");
                Console.ReadKey();
                return;
            }

            var selectedAccount = savingsAccounts[choice - 1];

            Console.WriteLine($"\nSelected account: {selectedAccount.AccountNumber}");
            Console.WriteLine($"Current balance: {selectedAccount.Balance:N2} {selectedAccount.Currency}");
            Console.WriteLine($"Current bank interest rate: {bank.DefaultSavingsInterestRate}%\n");

            Console.Write("Enter number of months: ");
            if (!int.TryParse(Console.ReadLine(), out int months) || months <= 0)
            {
                Console.WriteLine("\nInvalid input.\n");
                Console.ReadKey();
                return;
            }

            // Använd bankens fasta ränta
            decimal rate = bank.DefaultSavingsInterestRate / 100m;

            decimal balanceInSEK;
            decimal currencyRate = 1.0m;

            // Om kontot inte är SEK, hämta valutakursen
            if (!selectedAccount.Currency.Equals("SEK", StringComparison.OrdinalIgnoreCase))
            {
                var exchangeService = new bank.Services.ExchangerateService();
                var rates = exchangeService.getAllRates();

                var accountRate = rates.FirstOrDefault(r =>
                    (!string.IsNullOrWhiteSpace(r.CustomCode) && r.CustomCode == selectedAccount.Currency)
                    || r.Code.ToString() == selectedAccount.Currency);

                currencyRate = accountRate?.Rate ?? 1.0m;
                balanceInSEK = selectedAccount.Balance * currencyRate;
            }
            else
            {
                // SEK är basvalutan
                balanceInSEK = selectedAccount.Balance;
            }

            // Beräkna framtida saldo i SEK
            decimal futureBalanceSEK = balanceInSEK * (decimal)Math.Pow((double)(1 + rate / 12), months);
            decimal interestEarnedSEK = futureBalanceSEK - balanceInSEK;

            Console.WriteLine($"\nCurrent balance: {selectedAccount.Balance:N2} {selectedAccount.Currency}");
            if (!selectedAccount.Currency.Equals("SEK", StringComparison.OrdinalIgnoreCase))
                Console.WriteLine($"Converted to SEK: {balanceInSEK:N2} SEK");

            Console.WriteLine($"Interest rate: {bank.DefaultSavingsInterestRate}% for {months} months");

            Console.WriteLine($"\nProjected balance (in SEK): {futureBalanceSEK:N2}");
            Console.WriteLine($"Total interest earned: {interestEarnedSEK:N2} SEK");

            // Om kontot inte är i SEK, visa även uppskattad motsvarighet i originalvalutan
            if (!selectedAccount.Currency.Equals("SEK", StringComparison.OrdinalIgnoreCase))
            {
                decimal reverseRate = 1 / currencyRate;
                decimal futureBalanceInOriginal = futureBalanceSEK * reverseRate;
                Console.WriteLine($"≈ {futureBalanceInOriginal:N2} {selectedAccount.Currency} (approx.)");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
