using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    public class User
    {
        public string Id { get; }
        public string Name { get; }
        public List<Account> Accounts { get; } = new();
        public User(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id))
                Console.WriteLine(nameof(id), " saknas eller kunde inte hämtas");
            if (string.IsNullOrWhiteSpace(name))
                Console.WriteLine(nameof(name), " saknas eller kunde inte hämtas");
            Id = id;
            Name = name;
        }
    }

}
