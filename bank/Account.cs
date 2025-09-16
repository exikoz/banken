using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    internal class Account
    {
        public int Id {  get; set; }
        public int UserId { get; set; }
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }
           
        public DateTime Timestamp { get; set; }

    }
}
