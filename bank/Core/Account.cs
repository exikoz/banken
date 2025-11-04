using System;
using System.Collections.Generic;

namespace bank.Core
{
    /// <summary>
    /// Represents a basic bank account. Can be inherited by CheckingAccount, SavingsAccount, etc.
    /// </summary>
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; internal set; }  // internal set allows modification inside project
        public User Owner { get; set; }
        public string AccountType { get; protected set; }
        public string Currency { get; set; }

        // List of all transactions linked to this account
        public List<Transaction> Transactions { get; } = new();

        public Account(string accountNumber, User owner, string accountType = "Generic", string currency = "SEK")
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            AccountType = accountType;
            Currency = currency;
            Balance = 0;
        }

        /// <summary>
        /// Deposit money into the account.
        /// </summary>
        public virtual void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nDeposit failed: amount must be greater than 0.");
                return;
            }

            Balance += amount;
            Console.WriteLine($"\nDeposited {amount:N2} {Currency}. New balance: {Balance:N2} {Currency}.");
        }

        /// <summary>
        /// Withdraw money from the account if funds are available.
        /// </summary>
        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nWithdraw failed: amount must be greater than 0.");
                return;
            }

            if (amount > Balance)
            {
                Console.WriteLine("\nWithdraw failed: insufficient balance.");
                return;
            }

            Balance -= amount;
            Console.WriteLine($"\nWithdrawn {amount:N2} {Currency}. New balance: {Balance:N2} {Currency}.");
        }
    }
}
