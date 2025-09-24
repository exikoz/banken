using bank.poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.ui
{
    public class Menu
    {

        public Menu() {  }
        public void DrawUI()
        {
            Console.WriteLine("----------------Bank----------------");

            var bank = new Bank();
            var user = new User(id: "A1234", name: "Alexander");
            var accountNumber = "N1234";
            var acc = bank.OpenAccount(user, accountNumber);

            var accountResult = bank.FindAccount(accountNumber)?.AccountNumber ?? "  kunde inte hitta kontot..!";
            Console.WriteLine($"Konto: {accountResult}");


            Console.Write("Sätt in pengar: \n");
            acc.Deposit(100);

            Console.Write("Ta ut pengarna igen: \n ");
            acc.Withdraw(100);

            Console.WriteLine("------------------------------------");

            Console.WriteLine("Testar fel: \n");

            var accountNumberS = " ";
            var accS = bank.OpenAccount(user, accountNumberS);

            var accountResultS = bank.FindAccount("dfdsfdsf")?.AccountNumber ?? "  kunde inte hitta kontot..!";
            Console.WriteLine($"Konto: {accountResultS}");


            Console.Write("Ta ut pengarna: ");
            acc.Withdraw(100);

            Console.Write("Sätt in pengar: ");
            acc.Deposit(100);

            var userr = new User(id: "A1234", name: "Alexander");
            var accountNumberr = "N1234";
            var accc = bank.OpenAccount(userr, accountNumberr);

        }
    }
}
