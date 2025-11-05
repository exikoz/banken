using bank.Utils;
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

        public int FailedLoginAttempts { get;  set; } = 0;
        public int MaxFailedLoginAttempts { get;  set; } = 3; // added setter incase we would want to increase/decrease based on user (for example if they have been blocked alot before etc)

        public bool isBlocked => FailedLoginAttempts >= MaxFailedLoginAttempts;



        public List<Account> Accounts { get; } = new();
        public User(string id, string name, string pin, UserRole role = UserRole.Customer)
        {
            ValidationHelper.IsValid(id);
            ValidationHelper.IsValid(name);
            ValidationHelper.IsValid(pin);
            Id = id;
            Name = name;
            PIN = pin;
            Role = role;
        }

        public bool ValidatePIN(string pin) => PIN == pin;

        public bool IsAdmin() => Role == UserRole.Admin;


    }

}
