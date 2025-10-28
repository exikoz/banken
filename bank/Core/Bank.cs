using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChackingAccount = bank.Core.CheckingAccount;


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

        public Account OpenAccount(User user, string accountNumber)
        {

            if (string.IsNullOrWhiteSpace(accountNumber)) Console.WriteLine($"Bank: Kontonummer måste innehålla något. Du skrev in: {accountNumber} \n ");
            if (user == null) Console.WriteLine("Bank: User kunde inte hittas!");

            if (FindAccount(accountNumber) == null)
                Console.WriteLine($"Bank: Kontot: {accountNumber} är godkänt. \n");
            else
            {
                Console.WriteLine($"Bank: Kontot: {accountNumber} är redan upptaget. \n");
            }
            ;


            if (!Users.Contains(user))
            {
                Users.Add(user);
            } 
         // Create new CheckingAccount by default INSTEAD OF SAVINGSACCOUNT 
         
            var account = new CheckingAccount(accountNumber, user);
            Accounts.Add(account);
            user.Accounts.Add(account);
            Console.WriteLine($"Bank: Kontot: {account.AccountNumber}  har skapats till användar ID: {user.Id} \n");
            return account;
        }

        public void RegisterUser(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);
        }

        public User? FindUser(string userId) => Users.FirstOrDefault(u => u.Id == userId);
        public bool Transfer(User user, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (user == null)
            {
                Console.WriteLine("Transfer: Ogiltig användare.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(fromAccountNumber) || string.IsNullOrWhiteSpace(toAccountNumber))
            {
                Console.WriteLine("Transfer: Från- och till-kontonummer måste anges.");
                return false;
            }
            if (fromAccountNumber == toAccountNumber)
            {
                Console.WriteLine("Transfer: Kan inte överföra till samma konto.");
                return false;
            }
            if (amount <= 0)
            {
                Console.WriteLine("Transfer: Belopp måste vara > 0.");
                return false;
            }

            var from = FindAccount(fromAccountNumber);
            var to = FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                Console.WriteLine("Transfer: Ett eller båda konton finns inte.");
                return false;
            }

            // Säkerställ att båda kontona ägs av den inloggade användaren
            if (from.Owner != user || to.Owner != user)
            {
                Console.WriteLine("Transfer: Du kan endast föra över mellan dina egna konton.");
                return false;
            }

            // Kontrollera täckning inkl. ev. övertrass på CheckingAccount
            bool hasCoverage;
            if (from is CheckingAccount ca)
                hasCoverage = (from.Balance - amount) >= -ca.OverdraftLimit;
            else
                hasCoverage = amount <= from.Balance;

            if (!hasCoverage)
            {
                Console.WriteLine("Transfer: Otillräckliga medel (inkl. övertrasseringsgräns).");
                return false;
            }

            // Använd befintliga regler för uttag/insättning så transaktioner loggas korrekt
            var before = from.Balance;
            from.Withdraw(amount); // kommer att neka om interna regler inte uppfylls
            var withdrew = (from.Balance == before - amount);

            if (!withdrew)
            {
                Console.WriteLine("Transfer: Uttaget misslyckades.");
                return false;
            }

            to.Deposit(amount);
            Console.WriteLine($"Transfer: {amount} kr överfört från {fromAccountNumber} till {toAccountNumber}.");
            return true;
        }
        


    }
    
    
}

