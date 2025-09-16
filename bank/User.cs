using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PersonNumber { get; set; }
        public int Phone {  get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Adress { get; set; }

        public int PostalCode { get; set; }

        public DateTime  TimeStamp { get; set; }
    }

}
