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
        public void DrawUI(){}

        public void BankApp()
        {
            var bank = new Bank();
            var user = new User(id: "A1234", name: "Alexander");
            var accountNumber = "N1234";
            var acc = bank.OpenAccount(user, accountNumber);

            var accountResult = bank.FindAccount(accountNumber)?.AccountNumber ?? "  kunde inte hitta kontot..! \n";
            Console.WriteLine($"Konto: {accountResult} \n");


            Console.Write("Sätt in pengar: \n");
            acc.Deposit(100);

            Console.Write("Ta ut pengarna igen: \n ");
            acc.Withdraw(100);


        }
    } 
}
