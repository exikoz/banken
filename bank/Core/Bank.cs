using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{   
        public class Bank
    {

        public List<User> Users { get; } = new();
        public List<Account> Accounts { get; } = new();


        public Bank() { }
        /// <summary>
        /// /ss
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public Account? FindAccount(string accountNumber) 
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber); 
        }

        public Account OpenAccount(User user, string accountNumber, string accountType = "checking")
        {
           
            if (user == null)
            {
                Console.WriteLine("Bank: User not found.");
                return null;
            }

            if (FindAccount(accountNumber) != null)
            {
                Console.WriteLine($"Bank: Kontot: {accountNumber} är redan upptaget. \n");
            };
            
            
            if (!Users.Contains(user)) 
            {
                Users.Add(user);
            }

            var account = new Account(accountNumber.Trim(), user);
            Accounts.Add(account);
            user.Accounts.Add(account);

            Console.WriteLine($"Bank: {accountType} account {account.AccountNumber} created for user {user.Name}.");
            return account;
        }

        public Account OpenAccount(User user, string accountNumber, string accountType)
        {
            Account account;

            switch (accountType.ToLower())
            {
                case "savings":
                    account = new SavingsAccount(accountNumber.Trim(), user);
                    break;

                case "checking":
                    account = new CheckingAccount(accountNumber.Trim(), user);
                    break;

                default:
                    account = new Account(accountNumber.Trim(), user);
                    break;
                    ;
            }

            Accounts.Add(account);
            user.Accounts.Add(account);

            Console.WriteLine($"Bank: {accountType}-account {account.AccountNumber} created for user {user.Id}");
            return account;
        }


        public void RegisterUser(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);
        }

        public User? FindUser(string userId) => Users.FirstOrDefault(u => u.Id == userId);


    }
}

