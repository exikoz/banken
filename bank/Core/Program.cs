using System.Threading;

namespace bank.Core
{
    public class Program
    {
        static void Main(string[] args)
        {
            var menu = new Menu();
            menu.DrawUI();
            //menu.BankApp();
        }
    }
}

