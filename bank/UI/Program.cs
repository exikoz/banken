using bank.ui;
using System.Threading;

namespace bank.ConsoleUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            var menu = new Menu();
            menu.DrawUI();
            menu.BankApp();
        }
    }
}

