using bank.Attributes;
using System.Threading;
using System.Transactions;

namespace bank.Core
{
    public class Program
    {
        static void Main(string[] args)
        {



            Console.WriteLine("Type in a number between 1-10");



            bool flag = true;
            while (flag)
            {
                string input = Console.ReadLine();





                var user = new User("id","name","pib", UserRole.Admin);

                var SavingsAccount = new SavingsAccount("123", user);
                decimal value = decimal.Parse(input);


                ValidateInput.Call(SavingsAccount, "Deposit", value);

            }




            /*
            var menu = new Menu();
            menu.DrawUI();
            //menu.BankApp();
            */

        }


        public static void testss(string hello)
        {
            Console.Write(hello);
        }
    }









}


