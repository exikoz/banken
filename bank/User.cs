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
                Console.WriteLine(nameof(id), " missing or couldn't be fetched");
            if (string.IsNullOrWhiteSpace(name))
                Console.WriteLine(nameof(name), " missing or couldn't be fetched");
            Id = id;
            Name = name;
        }
    }

}
