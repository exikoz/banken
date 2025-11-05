using bank.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bank.Services
{
    /// <summary>
    /// Handles all search operations
    /// </summary>
    public class SearchService
    {
        private readonly Bank bank;
        private readonly ExchangerateService exchangerateService;

        public SearchService(Bank bank, ExchangerateService exchangerateService)
        {
            this.bank = bank;
            this.exchangerateService = exchangerateService;
        }

        /// <summary>
        /// Find account by exact account number
        /// </summary>
        public Account? FindAccountByNumber(string accountNumber)
        {
            return bank.FindAccount(accountNumber);
        }

        /// <summary>
        /// Find all accounts belonging to users whose name contains the search term
        /// </summary>
        public List<Account> FindAccountsByUsername(string username)
        {
            var matchingUsers = bank.Users
                .Where(u => u.Name.Contains(username, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return matchingUsers
                .SelectMany(u => u.Accounts)
                .ToList();
        }

        /// <summary>
        /// Find users by name (partial match, case-insensitive)
        /// </summary>
        public List<User> FindUsersByName(string name)
        {
            return bank.Users
                .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Get all accounts in the system
        /// </summary>
        public List<Account> GetAllAccounts()
        {
            return bank.Accounts.ToList();
        }

        /// <summary>
        /// Get all users in the system
        /// </summary>
        public List<User> GetAllUsers()
        {
            return bank.Users.ToList();
        }



        /// <summary>
        /// Get all exchangerates in the system
        /// </summary>
        public IEnumerable<ExchangeRate> GetAllExchangeRates()
        {
            return exchangerateService.getAllRates();
        }
    }
}