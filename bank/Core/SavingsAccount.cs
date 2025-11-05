using bank.Attributes;
using System;

namespace bank.Core
{
    public class SavingsAccount : Account
    {
        private int freeWithdrawals = 3;
        private decimal fee = 15;

        public int FreeWithdrawalsLeft => freeWithdrawals;
        public decimal WithdrawFee => fee;

        public SavingsAccount(string accountNumber, User owner, string accountType, string currency)
            : base(accountNumber, owner, accountType, currency) { }

        public override void Withdraw(decimal amount)
        {
            decimal total = amount;
            if (freeWithdrawals <= 0)
                total += fee;

            if (total > Balance)
                return;

            base.Withdraw(total);
            if (freeWithdrawals > 0) freeWithdrawals--;
        }
        // No custom TX logging here
    }
}
