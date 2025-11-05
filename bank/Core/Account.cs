using System;
using System.Collections.Generic;

namespace bank.Core
{
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; internal set; }
        public User Owner { get; set; }
        public string AccountType { get; protected set; }
        public string Currency { get; set; }
        public List<Transaction> Transactions { get; } = new();

        public Account(string accountNumber, User owner, string accountType = "Generic", string currency = "SEK")
        {
            AccountNumber = accountNumber;
            Owner = owner;
            AccountType = accountType;
            Currency = currency;
            Balance = 0;
        }

        // Deposit logic only, no UI
        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0)
                return;

            Balance += amount;
        }

        // Withdraw logic only, no UI
        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
                return;

            if (amount > Balance)
                return;

            Balance -= amount;
        }
    }
}
