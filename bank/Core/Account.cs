using bank.poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    public class Account
    {

        public string AccountNumber { get; set; }
        public decimal Balance { get; private set; }
        public User Owner { get; set; }

        public Account(string accountNumber, User owner) { 
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Balance = 0;
        }
            

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(" Deposit failed: beloppet måste vara större än 0.");
                return; 
            }

            Balance += amount;
            Console.WriteLine($" Deposit lyckades: +{amount} kr. Ny balans = {Balance} kr.");
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(" Withdraw failed: beloppet måste vara större än 0.");
                return;
            }

            if (amount > Balance)
            {
                Console.WriteLine(" Withdraw failed: otillräckligt saldo.");
                return;
            }

            Balance -= amount;
            Console.WriteLine($"Withdraw lyckades: -{amount} kr. Ny balans = {Balance} kr.");
        }

    }
}