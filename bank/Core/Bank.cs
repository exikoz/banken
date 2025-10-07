using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.poco
{   
        public class Bank
    {

        public List<User> Users { get; } = new();
        public List<Account> Accounts { get; } = new();


        public Bank() { }

        public Account? FindAccount(string accountNumber) 
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber); 
        }

        public Account OpenAccount(User user, string accountNumber)
        {
           
            if (string.IsNullOrWhiteSpace(accountNumber)) Console.WriteLine($"Bank: Kontonummer måste innehålla något. Du skrev in: {accountNumber} \n ");
            if (user == null) Console.WriteLine("Bank: User kunde inte hittas!");

            if (FindAccount(accountNumber) == null)
                Console.WriteLine($"Bank: Kontot: {accountNumber} är godkänt. \n");
            else
            {
                Console.WriteLine($"Bank: Kontot: {accountNumber} är redan upptaget. \n");
            };
            
            
            if (!Users.Contains(user)) 
            {
                Users.Add(user);
            } 
   
            var account = new Account(accountNumber, user);
            Accounts.Add(account);
            user.Accounts.Add(account);
            Console.WriteLine($"Bank: Kontot: {account.AccountNumber}  har skapats till användar ID: {user.Id} \n");
            return account;
        }


    }
}

