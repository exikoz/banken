using System;

namespace bank.Core
{
    public class CheckingAccount : Account
    {
        public decimal OverdraftLimit { get; private set; }

        public decimal AvailableFunds => Balance + OverdraftLimit;

        public CheckingAccount(string accountNumber, User owner, string accountType, string currency, decimal overdraftLimit = 1000m)
            : base(accountNumber, owner, accountType, currency)
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
                    $"\nAmount: {amount} {Currency} | Balance: {Balance} {Currency} | Available: {AvailableFunds} {Currency} | Min allowed balance: {minAllowed} {Currency}");
                return;
            }

            // Apply withdrawal
            Balance = projected;

            // Log transaction in shared transaction list
            var tx = new Transaction(
                id: Guid.NewGuid().ToString("N"),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Withdraw",
                amount: amount
            )
            {
                Currency = this.Currency,
                FromUser = "Internal",
                ToUser = "Internal",
                FromAccount = AccountNumber,
                ToAccount = AccountNumber
            };
            Transactions.Add(tx);

            Console.WriteLine($"\nWithdraw succeeded: {amount} {Currency}. New balance = {Balance} {Currency}.");
        }

        public override void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nDeposit failed: amount must be greater than 0.");
                return;
            }

            Balance += amount;

            // Log transaction in shared transaction list
            var tx = new Transaction(
                id: Guid.NewGuid().ToString("N"),
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Deposit",
                amount: amount
            )
            {
                Currency = this.Currency,
                FromUser = "Internal",
                ToUser = "Internal",
                FromAccount = AccountNumber,
                ToAccount = AccountNumber
            };
            Transactions.Add(tx);

            Console.WriteLine($"\nDeposit succeeded: {amount} {Currency}. New balance = {Balance} {Currency}.");
        }
    }
}
