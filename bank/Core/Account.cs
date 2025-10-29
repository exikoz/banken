using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{

    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; protected set; }
        public User Owner { get; set; }
        public string AccountType { get; protected set; }


        // Transaction v40 - lista för att lagra transaktioner
        public List<Transaction> Transactions { get; } = new();

        public Account(string accountNumber, User owner, string accountType = "Generic")
        {
            AccountNumber = accountNumber;
            Owner = owner;
            Balance = 0;
            AccountType = accountType;
        }


        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nDeposit failed: The amount must be greater then 0..");
                return;
            }

            Balance += amount;
            
            Transactions.Add(new Transaction(
                id: Guid.NewGuid().ToString("N"),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Deposit",
                amount: amount
            ));

            Console.WriteLine($"\nDeposit succeeded: {amount} kr. New Balance = {Balance} kr.");
        }
        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nWithdraw failed: The amount must be greater then 0.");
                return;
            }

            if (amount > Balance)
            {
                Console.WriteLine("\nWithdraw failed: unsufficient balance");
                return;
            }

            Balance -= amount;
          
            Transactions.Add(new Transaction(
                id: Guid.NewGuid().ToString("N"),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Withdraw",
                amount: amount
            ));

            Console.WriteLine($"\nWithdraw succeded: -{amount} kr. New balance = {Balance} kr.");
        }
    }
}
