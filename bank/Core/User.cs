using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Core
{
    public enum UserRole
    {
        Customer,
        Admin
    }


    public class User
    {
        public string Id { get; }
        public string Name { get; }
        public string PIN { get; private set; }
        public UserRole Role { get; set; }
        public List<Account> Accounts { get; set; } = new();
        public bool IsLocked { get; private set; } = false; // blir true när användaren blockeras
        public void Lock() => IsLocked = true;              // blockera användaren
        public void Unlock() => IsLocked = false;           // för admin senare om vi vill


        public User(string id, string name, string pin, UserRole role = UserRole.Customer)
        {
            if (string.IsNullOrWhiteSpace(id))
                Console.WriteLine(nameof(id), " missing or couldn't be fetched");
            if (string.IsNullOrWhiteSpace(name))
                Console.WriteLine(nameof(name), " missing or couldn't be fetched");
            Id = id;
            Name = name;
            PIN = pin;
            Role = role;
        }

        public bool ValidatePIN(string pin) => PIN == pin;

        public bool IsAdmin() => Role == UserRole.Admin;
    }

}
