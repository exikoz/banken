using bank.Attributes;
using System;

namespace bank.Core
{
    public class SavingsAccount : Account
    {
        private int freeWithdrawals = 3;
        private decimal fee = 15;
        private static int transactionCounter = 0;

        public int FreeWithdrawalsLeft => freeWithdrawals;
        public decimal WithdrawFee => fee;

        public SavingsAccount(string accountNumber, User owner, string accountType, string currency)
            : base(accountNumber, owner, accountType, currency) { }

        private string GenerateTransactionId()
        {
            transactionCounter++;
            return $"TX-{AccountNumber}-{transactionCounter:D4}";
        }

        public override void Deposit(decimal amount)
        {
            base.Deposit(amount);

            var tx = new Transaction(GenerateTransactionId(), AccountNumber, DateTime.Now, "Deposit", amount)
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
            decimal total = amount;

            if (freeWithdrawals <= 0)
                total += fee;

            if (total > Balance)
                return;

            base.Withdraw(total);

            if (freeWithdrawals > 0)
                freeWithdrawals--;

            var tx = new Transaction(GenerateTransactionId(), AccountNumber, DateTime.Now, "Withdraw", amount)
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
