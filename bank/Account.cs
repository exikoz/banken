using bank.poco;
using System;
using System.Collections.Generic;

namespace bank
{
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; protected set; }
        public User Owner { get; set; }

        
        public List<Transaction> Transactions { get; } = new();

        public Account(string accountNumber, User owner)
        {
            AccountNumber = accountNumber;
            Owner = owner;
            Balance = 0;
        }

        
        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Deposit failed: beloppet måste vara större än 0.");
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

            Console.WriteLine($"Deposit lyckades: +{amount} kr. Ny balans = {Balance} kr.");
        }

        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Withdraw failed: beloppet måste vara större än 0.");
                return;
            }

            if (amount > Balance)
            {
                Console.WriteLine("Withdraw failed: otillräckligt saldo.");
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

            Console.WriteLine($"Withdraw lyckades: -{amount} kr. Ny balans = {Balance} kr.");
        }
    }
}
