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

        // hanterar null vid hämtning
        public Account? FindAccount(string accountNumber) 
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber); // stannar vid första match annars null(default)
        }

        public Account OpenAccount(User user, string accountNumber)
        {
            if(user == null) throw new ArgumentNullException("Användare kunde inte hittas");
            // Inte null och inte heller " "
            if (string.IsNullOrWhiteSpace(accountNumber)) Console.WriteLine("Bank: Kontonummer måste innehålla något");
            if(FindAccount(accountNumber) != null) Console.WriteLine("Bank: Kontonummer måste innehålla något");
            if (!Users.Contains(user)) 
            {
                Users.Add(user);
            } 
   
            var account = new Account(accountNumber, user);
            Accounts.Add(account);
            user.Accounts.Add(account);
            return account;
        }


    }
}

