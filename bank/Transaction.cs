using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    internal class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public DateTime Timestamp { get; set; }

    }
}


