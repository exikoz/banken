using System;

namespace bank.Core
{
    public class CheckingAccount : Account
    {
        public CheckingAccount(string accountNumber, User owner, string accountType, string currency)
            : base(accountNumber, owner, accountType, currency) { }

      
    }
}
