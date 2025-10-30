using System;
using bank.Core;

namespace bank.Services
{
    /// <summary>
    /// Public service class that handles transfers between a user's own accounts.
    /// Validates accounts, ownership, and coverage (including overdraft on CheckingAccount).
    /// </summary>
    public class TransferService
    {
        private readonly Bank bank;

        public TransferService(Bank bank)
        {
            this.bank = bank ?? throw new ArgumentNullException(nameof(bank));
        }

        public bool Transfer(User user, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (user == null)
            {
                Console.WriteLine("Transfer: Invalid user.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(fromAccountNumber) || string.IsNullOrWhiteSpace(toAccountNumber))
            {
                Console.WriteLine("Transfer: From and To account numbers are required.");
                return false;
            }
            if (fromAccountNumber == toAccountNumber)
            {
                Console.WriteLine("Transfer: Cannot transfer to the same account.");
                return false;
            }
            if (amount <= 0)
            {
                Console.WriteLine("Transfer: Amount must be greater than 0.");
                return false;
            }

            var from = bank.FindAccount(fromAccountNumber);
            var to = bank.FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                Console.WriteLine("Transfer: One or both accounts do not exist.");
                return false;
            }

            
            if (from.Owner != user || to.Owner != user)
            {
                Console.WriteLine("Transfer: You can only transfer between your own accounts.");
                return false;
            }

            
            bool hasCoverage = from is CheckingAccount ca
                ? (from.Balance - amount) >= -ca.OverdraftLimit
                : amount <= from.Balance;

            if (!hasCoverage)
            {
                Console.WriteLine("Transfer: Insufficient funds (including overdraft limit).");
                return false;
            }

           
            var before = from.Balance;
            from.Withdraw(amount); 
            var withdrew = (from.Balance == before - amount);
            if (!withdrew)
            {
                Console.WriteLine("Transfer: Withdrawal failed.");
                return false;
            }

            to.Deposit(amount); 
            Console.WriteLine($"Transfer: {amount} transferred from {fromAccountNumber} to {toAccountNumber}.");
            return true;
        }

        public void DoTransfer(User currentUser)
        {
            Console.Write("\nFrom account number: ");
            var from = Console.ReadLine()?.Trim();

            Console.Write("To account number: ");
            var to = Console.ReadLine()?.Trim();

            Console.Write("Amount: ");
            var amountRaw = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                Console.WriteLine("✗ Invalid account numbers.");
                Console.ReadKey();
                return;
            }
            if (!decimal.TryParse(amountRaw, out var amount))
            {
                Console.WriteLine("✗ Invalid amount.");
                Console.ReadKey();
                return;
            }

            var ok = Transfer(currentUser, from, to, amount);
            Console.WriteLine(ok ? "✓ Transfer completed." : "✗ Transfer failed.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
