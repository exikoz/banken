using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    internal class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; private set; }
        public User Owner { get; set; }
        public Account(string accountNumber, User owner)
        {
            AccountNumber = accountNumber;
            Owner = owner;
            Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(" Deposit failed: The amount must be greater then 0.");
                return;
            }
            Balance += amount;
            Console.WriteLine($"Deposit succeeded: {amount} kr. New Balance = {Balance} kr.");
        }
        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(" Withdraw failed: Yhe amount must be greater then 0.");
                return;
            }
            if (amount > Balance)
            {
                Console.WriteLine(" Withdraw failed: unsufficient balance ");
                return;
            }
            Balance -= amount;
            Console.WriteLine($"Withdraw succeded: -{amount} kr. New balance = {Balance} kr.");
        }




    }
}
