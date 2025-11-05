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

        public bool canTransfer { get; set; }
        public Account(string accountNumber, User owner, string accountType = "Generic", string currency = "SEK")
        {
            AccountNumber = accountNumber;
            Owner = owner;
            AccountType = accountType;
            Currency = currency;
            Balance = 0;
            canTransfer = true;
        }

        // Deposit logic with transaction logging
        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0 )
                return;

            if (!canTransfer)
                return;

            Balance += amount;

            var tx = new Transaction(
                id: Bank.GenerateTransactionId(),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Deposit",
                amount: amount,
                currency: Currency,
                accountType: AccountType,
                fromAccount: "0000",
                toAccount: AccountNumber,
                fromUser: "ATM",
                toUser: Owner.Name
            );

            tx.IsPending = true;
            tx.Status = "Pending";

            Transactions.Add(tx);
        }

        // Withdraw logic with transaction logging
        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
                return;

            if (amount > Balance)
                return;

            if (!canTransfer)
                return;


            var tx = new Transaction(
                id: Bank.GenerateTransactionId(),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Withdraw",
                amount: amount,
                currency: Currency,
                accountType: AccountType,
                fromAccount: AccountNumber,
                toAccount: "0000",
                fromUser: Owner.Name,
                toUser: "ATM"
            );

            tx.IsPending = true;
            tx.Status = "Pending";

            Transactions.Add(tx);
        }

    }
}
