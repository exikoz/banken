using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace bank.Core
{
    public class CheckingAccount : Account
    {
         public decimal OverdraftLimit { get; private set; }

             public decimal AvailableFunds => Balance + OverdraftLimit;

        public CheckingAccount(string accountNumber, User owner, decimal overdraftLimit = 1000m)
            : base(accountNumber, owner)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.", nameof(accountNumber));
            if (owner is null)
                throw new ArgumentNullException(nameof(owner), "Owner (User) cannot be null.");
            if (overdraftLimit < 0)
                throw new ArgumentOutOfRangeException(nameof(overdraftLimit), "OverdraftLimit must be ≥ 0.");

            OverdraftLimit = overdraftLimit;
        }

  
        public void SetOverdraftLimit(decimal newLimit)
        {
            if (newLimit < 0)
                throw new ArgumentOutOfRangeException(nameof(newLimit), "OverdraftLimit must be ≥ 0.");
            OverdraftLimit = newLimit;
        }


        public override void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nWithdraw failed: amount must be greater than 0.");
                return;
            }

            decimal projected = Balance - amount;
            decimal minAllowed = -OverdraftLimit;

            if (projected < minAllowed)
            {
                Console.WriteLine(
                    $"\nWithdraw denied: overdraft limit exceeded." +
                    $"\nAmount: {amount} | Balance: {Balance} | Available: {AvailableFunds} | Min allowed balance: {minAllowed}");
                return;
            }

            Balance = projected;

            
            Transactions.Add(new Transaction(
                id: Guid.NewGuid().ToString("N"),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Withdraw",
                amount: amount
            ));

            Console.WriteLine($"\nWithdraw succeeded: {amount} kr. New balance = {Balance} kr.");
        }

       
    }
}
