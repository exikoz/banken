using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    
    public class SavingsAccount : Account
    {
        private int freeWithdrawals = 5;
        private decimal fee = 15;
        private List<Transaction> transactions = new List<Transaction>();
        private static int transactionCounter = 0;

        public SavingsAccount(string accountNumber, User owner) : base(accountNumber, owner) { }

        private string GenerateTransactionId()
        {
            transactionCounter++;
            return $"TX-{AccountNumber}-{transactionCounter:D4}";
        }

        public override void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Deposit must be greater than 0.");
                return;
            }

            base.Deposit(amount);
            var tx = new Transaction(GenerateTransactionId(), AccountNumber, DateTime.Now, "Deposit", amount);
            transactions.Add(tx);
            Console.WriteLine($"Deposited {amount:C}. Balance: {Balance:C}");
        }

        public override void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Withdrawal must be greater than 0.");
                return;
            }

            decimal total = amount;
            if (freeWithdrawals > 0)
                freeWithdrawals--;
            else
            {
                total += fee;
                Console.WriteLine($"Withdrawal fee of {fee:C} applied.");
            }

            if (total > Balance)
            {
                Console.WriteLine($"Not enough balance for withdrawal of {amount:C} (total with fee: {total:C})");
                return;
            }

            base.Withdraw(total);
            var tx = new Transaction(GenerateTransactionId(), AccountNumber, DateTime.Now, "Withdraw", amount);
            transactions.Add(tx);
            Console.WriteLine($"Withdrew {amount:C}. Balance: {Balance:C}");
        }
    }
 }
