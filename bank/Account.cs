using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    internal class Account
    {
        public int id {  get; set; }
        public int user_id { get; set; }
        public int accountNumber { get; set; }
        public decimal balance { get; set; }
           
        public DateTime timestamp { get; set; }

    }
}
