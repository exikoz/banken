using bank.poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core.Tests
{
    public class Tests
    {

        public Tests() { }

         public Account setup(string id, string username, string accountNumber)
        {


            var bank = new Bank();
            string userId = id + 1;

            var user = new User(userId, username);
            var account = bank.OpenAccount(user, accountNumber);
            var accountResult = bank.FindAccount(accountNumber)?.AccountNumber ?? "  kunde inte hitta kontot..!";
            if (accountResult != null)
            {
                Console.WriteLine($"Konto: {accountResult}");
            }
            else
            {
                Console.WriteLine("Meny: Kontot kunde inte skapas/hittas");
            }

            return account;
        }

        public void printTestMessage(string test, string name)
        {
            Console.WriteLine($"-------------- TESTAR: {test} i {name} --------------  \n");
        }
        public void printEndMessage(string name)
        {
            Console.WriteLine($"-------------- AVSLUTAR: {name} --------------  \n");
        }

        public void testingTransactions_success(string id, string username, string accountNumber)
        {
            printTestMessage("Ta ut rätt summa", "Transactions");

            Account acc = setup(id, username, accountNumber);
            Console.Write("Sätt in pengar: \n");
            acc.Deposit(100);
            Console.Write("Ta ut pengarna igen: \n ");
            acc.Withdraw(100);

            printEndMessage("Ta ut rätt summa test");
        }

        public void testingTransactions_invalidRange(string id, string username, string accountNumber)
        {
            printTestMessage("Ta ut fel summa", "Transactions");

            Account acc = setup(id, username, accountNumber);
            Console.Write("Ta ut pengar: \n ");
            acc.Withdraw(100);
            Console.Write("Sätt in pengar: \n");
            acc.Deposit(100);

            printEndMessage("Ta ut fel summa test");
        }

        public void testingFindAccount_fail(string accountNumber)
        {
            printTestMessage("Hitta kontonummer", "Bank: FindAccount()");
            Bank bank = new Bank();
            var accountResult = bank.FindAccount(accountNumber)?.AccountNumber ?? "  kunde inte hitta kontot..!";
            printEndMessage("Hitta kontonummer test");
        }



        public void testingDuplicate()
        {
            printTestMessage("Skapa dubletter", "User");

            var bank = new Bank();
            var oldUser = new User(id: "A1234", name: "Alexander");
            var newUser = new User(id: "A1234", name: "Alexander");

            printEndMessage("Skapa dubletter test");
        }

        public void testingUser_nullInput()
        {
            printTestMessage("Skicka in null/whitespace input", "User");

            var user = new User(id: " ", name: " ");

            printEndMessage("null input test");
        }


    }
}
