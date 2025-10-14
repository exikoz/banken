using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    public class CheckingAccount : Account
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="owner"></param>
  
        public CheckingAccount(string accountNumber, User owner) : base(accountNumber, owner){}

    }
}
