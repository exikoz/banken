using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    public class SavingsAccount : Account
    {

        private int _freeWithdrawals = 5;
        private decimal _fee = 15;
        public SavingsAccount(string accountNumber, User owner) : base(accountNumber, owner){}



        public override void Withdraw(decimal amount)
        {
            if (_freeWithdrawals > 0) 
            { 
                base.Withdraw(amount);
                _freeWithdrawals--; // minus 1 per call
            }
            else
            {
                base.Withdraw(amount + _fee); // drar av en avgift
            }
        }
    }
}
