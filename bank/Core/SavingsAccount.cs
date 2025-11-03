using bank.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    public class SavingsAccount : Account
    {
        private int freeWithdrawals = 3;
        private decimal fee = 15;
        private List<Transaction> transactions = new List<Transaction>();
        private static int transactionCounter = 0;

        public SavingsAccount(string accountNumber, User owner, string accountType, string currency)
            : base(accountNumber, owner, accountType, currency) { }

        private string GenerateTransactionId()
        {
            transactionCounter++;
            return $"TX-{AccountNumber}-{transactionCounter:D4}";
        }


        [ValidateInput(Min ="10", Max = "1000")]
        public override void Deposit(decimal amount)
        {
            base.Deposit(amount);

            var tx = new Transaction(GenerateTransactionId(), AccountNumber, DateTime.Now, "Deposit", amount);
            transactions.Add(tx);

            Console.WriteLine($"Deposited {amount} {Currency}. Balance: {Balance} {Currency}");
        }





        public override void Withdraw(decimal amount)
        {
            decimal total = amount;
            bool feeApplied = false;

            // Kontrollera om avgift ska användas
            if (freeWithdrawals <= 0)
            {
                total += fee;
                feeApplied = true;
            }

            // Kontrollera saldo innan något dras
            if (total > Balance)
            {
                if (feeApplied)
                    Console.WriteLine($"Not enough balance for withdrawal of {amount} {Currency} (total with fee: {total} {Currency})");
                else
                    Console.WriteLine($"Not enough balance for withdrawal of {amount} {Currency}");
                return;
            }

            // Genomför uttaget
            base.Withdraw(total);

            // Minska fria uttag först EFTER lyckat uttag
            if (freeWithdrawals > 0)
                freeWithdrawals--;

            if (feeApplied)
                Console.WriteLine($"Withdrawal fee of {fee} {Currency} applied.");

            // Logga transaktion
            var tx = new Transaction(GenerateTransactionId(), AccountNumber, DateTime.Now, "Withdraw", amount);
            transactions.Add(tx);

            Console.WriteLine($"Withdrew {amount} {Currency}. Balance: {Balance} {Currency}");

            // Visa återstående fria uttag
            if (freeWithdrawals > 0)
                Console.WriteLine($"You have {freeWithdrawals} free withdrawal(s) left.");
            else
                Console.WriteLine("No free withdrawals remaining — next withdrawal will include a fee.");
        }

        // Dynamisk ränteberäkning med stigande ränta efter 12 månader
        public decimal CalculateFutureBalance(decimal annualInterestRate, int months)
        {
            if (annualInterestRate < 0 || months < 0)
            {
                Console.WriteLine("\nError: interest rate and months must be positive values.\n");
                return Balance;
            }

            // Grundränta hämtas från bankens standard
            decimal baseRate = annualInterestRate;
            decimal monthlyRateIncrease = 0.1m;

            decimal effectiveRate = baseRate;
            if (months > 12)
                effectiveRate += (months - 12) * monthlyRateIncrease;

            decimal interest = Math.Round(Balance * (effectiveRate / 100m) * (months / 12m), 2);
            decimal futureBalance = Balance + interest;

            Console.WriteLine($"\nInterest calculation summary:");
            Console.WriteLine($"Current balance: {Balance} {Currency}");
            Console.WriteLine($"Period: {months} months");
            Console.WriteLine($"Interest rate: {annualInterestRate:N2}%");
            Console.WriteLine($"Interest earned: {interest:N2} {Currency}");
            Console.WriteLine($"Future balance: {futureBalance:N2} {Currency}\n");


            return futureBalance;
        }

    }
}
