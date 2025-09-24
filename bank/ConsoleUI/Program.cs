using bank.Core.Tests;
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


/*
string id = "ID1234";
string name = "Alexander";
string accountNumber = "N1234";
var tests = new Tests();

tests.testingTransactions_invalidRange(id, name, accountNumber);
tests.testingTransactions_success(id, name, accountNumber);
tests.testingUser_nullInput();
tests.testingDuplicate();
tests.testingFindAccount_fail(accountNumber);
*/
