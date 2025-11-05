using System;

namespace bank.Core
{
    public class CheckingAccount : Account
    {
        private static int transactionCounter = 0;

        public CheckingAccount(string accountNumber, User owner, string accountType, string currency)
            : base(accountNumber, owner, accountType, currency) { }

        private string GenerateTransactionId()
        {
            transactionCounter++;
            return $"TX-{AccountNumber}-{transactionCounter:D4}";
        }

        public override void Deposit(decimal amount)
        {
            base.Deposit(amount);

            var tx = new Transaction(
                GenerateTransactionId(),
                AccountNumber,
                DateTime.Now,
                "Deposit",
                amount)
            {
                Currency = Currency,
                FromUser = "Internal",
                ToUser = "Internal",
                FromAccount = AccountNumber,
                ToAccount = AccountNumber
            };

            Transactions.Add(tx);
        }

        public override void Withdraw(decimal amount)
        {
            if (amount <= 0)
                return;

            if (Balance < amount)
                return;

            Balance -= amount;

            var tx = new Transaction(
                GenerateTransactionId(),
                AccountNumber,
                DateTime.Now,
                "Withdraw",
                amount)
            {
                Currency = Currency,
                FromUser = "Internal",
                ToUser = "Internal",
                FromAccount = AccountNumber,
                ToAccount = AccountNumber
            };

            Transactions.Add(tx);
        }
    }
}
