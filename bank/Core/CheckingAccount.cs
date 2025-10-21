using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    public class CheckingAccount : Account
    {
        public decimal OverdraftLimit { get; private set; }

        public CheckingAccount(string accountNumber, User owner, decimal overdraftLimit = 1000)
            : base(accountNumber, owner)
        {
            OverdraftLimit = overdraftLimit;
        }
        public override void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("\nWithdraw failed: The amount must be greater than 0.");
                return;
            }

            // Tillåter övertrassering ned till OverdraftLimit
            if (Balance - amount < OverdraftLimit)
            {
                Console.WriteLine($"\nWithdraw failed: Overdraft limit of {OverdraftLimit} kr exceeded.");
                return;
            }

            Balance -= amount;

            //loggar en transaktion
            Transactions.Add(new Transaction(
                id: Guid.NewGuid().ToString("N"),   //Skapar ett unikt ID för transaktionen
                accountNumber: AccountNumber,
                timeStamp: DateTime.UtcNow,
                type: "Withdraw",
                amount: amount
            ));

            Console.WriteLine($"\nWithdraw succeeded: {amount} kr. New balance = {Balance} kr.");
        }
    }
}

