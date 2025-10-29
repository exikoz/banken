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
        /// Finds an account by its number.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public Account? FindAccount(string accountNumber)
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }


        public Account OpenAccount(User user, string accountType)
        {
            // Säkerställ att användaren är registrerad
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            // Ta bort "U" från användarens ID (ex: U003 → 003)
            string numericId = user.Id.Replace("U", "").Trim();

            // Skapa automatiskt kontonummer baserat på antal befintliga konton
            string accountNumber = $"{numericId}-{user.Accounts.Count + 1:D2}";

            Account account;

            switch (accountType.ToLower().Trim())
            {
                case "savings":
                    account = new SavingsAccount(accountNumber, user);
                    break;
                case "checking":
                    account = new CheckingAccount(accountNumber, user);
                    break;
                default:
                    account = new Account(accountNumber, user);
                    break;
            }

            Accounts.Add(account);
            user.Accounts.Add(account);

            Console.WriteLine($"\n New {accountType} account created: {accountNumber}");
            return account;
        }



        public Account OpenAccount(User user, string accountNumber, string accountType)
        {
            // Register user if not already tracked by the bank
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);

            Account account;

            switch (accountType.ToLower().Trim())
            {
                case "savings":
                    // Assuming SavingsAccount exists and inherits from Account
                    account = new SavingsAccount(accountNumber.Trim(), user);
                    break;

                case "checking":
                    // Assuming CheckingAccount exists and inherits from Account
                    account = new CheckingAccount(accountNumber.Trim(), user);
                    break;

                default:
                    // Use the base Account class for unknown types
                    account = new Account(accountNumber.Trim(), user);
                    break;
            }

            Accounts.Add(account);
            user.Accounts.Add(account);

            Console.WriteLine($"Bank: {accountType}-account {account.AccountNumber} created for user {user.Id}");
            return account;
        }


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
            // Note: This logic forces transfers only between the user's OWN accounts.
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

        public void RegisterUser(User user)
        {
            if (!Users.Any(u => u.Id == user.Id))
                Users.Add(user);
        }

        public User? FindUser(string userId) => Users.FirstOrDefault(u => u.Id == userId);
    }
}