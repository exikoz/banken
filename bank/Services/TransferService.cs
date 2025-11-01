using System;
using bank.Core;

namespace bank.Services
{
    /// <summary>
    /// Handles all transfers between accounts.
    /// </summary>
    public class TransferService
    {
        private readonly Bank bank;
        private readonly AccountService accountService;
        private readonly ExchangerateService exchangeRateService = new ExchangerateService();

        public TransferService(Bank bank, AccountService accountService)
        {
            this.bank = bank ?? throw new ArgumentNullException(nameof(bank));
            this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

        }

        public void DoTransferOwn(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSFER BETWEEN YOUR OWN ACCOUNTS ===\n");

            if (currentUser.Accounts.Count < 2)
            {
                Console.WriteLine("You need at least two accounts to transfer between your own accounts.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

          

            Console.Clear();
            Console.WriteLine("=== SELECT SOURCE ACCOUNT ===\n");
            for (int i = 0; i < currentUser.Accounts.Count; i++)
            {
                var acc = currentUser.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance} {acc.Currency}");
            }

            Console.Write("\nChoose source (number): ");
            if (!int.TryParse(Console.ReadLine(), out var srcIdx) || srcIdx < 1 || srcIdx > currentUser.Accounts.Count)
            {
                Console.WriteLine("✗ Invalid selection.");
                Console.ReadKey();
                return;
            }

            var fromAcc = currentUser.Accounts[srcIdx - 1];

            Console.Clear();
            Console.WriteLine("=== SELECT DESTINATION ACCOUNT ===\n");
            for (int i = 0; i < currentUser.Accounts.Count; i++)
            {
                var acc = currentUser.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance} {acc.Currency}");
            }

            Console.Write("\nChoose destination (number): ");
            if (!int.TryParse(Console.ReadLine(), out var dstIdx) || dstIdx < 1 || dstIdx > currentUser.Accounts.Count)
            {
                Console.WriteLine("✗ Invalid selection.");
                Console.ReadKey();
                return;
            }

            if (dstIdx == srcIdx)
            {
                Console.WriteLine("✗ Destination must be different from source.");
                Console.ReadKey();
                return;
            }

            var toAcc = currentUser.Accounts[dstIdx - 1];

            Console.Write($"\nAmount ({fromAcc.Currency}): ");
            var amountRaw = Console.ReadLine();
            if (!decimal.TryParse(amountRaw, out var amount))
            {
                Console.WriteLine("✗ Invalid amount.");
                Console.ReadKey();
                return;
            }

            var ok = TransferOwn(currentUser, fromAcc.AccountNumber, toAcc.AccountNumber, amount);
            Console.WriteLine(ok ? "✓ Transfer completed." : "✗ Transfer failed.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // Reworked to show user's own accounts selection directly, then ask recipient details
        public void DoTransferToOther(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSFER TO ANOTHER CUSTOMER ===\n");

            if (currentUser.Accounts.Count == 0)
            {
                Console.WriteLine("You have no accounts to transfer from.");
                Console.ReadKey();
                return;
            }

            

            Console.Clear();
            Console.WriteLine("=== SELECT YOUR ACCOUNT TO SEND FROM ===\n");
            for (int i = 0; i < currentUser.Accounts.Count; i++)
            {
                var acc = currentUser.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance} {acc.Currency}");
            }

            Console.Write("\nChoose source (number): ");
            if (!int.TryParse(Console.ReadLine(), out var srcIdx) || srcIdx < 1 || srcIdx > currentUser.Accounts.Count)
            {
                Console.WriteLine("✗ Invalid selection.");
                Console.ReadKey();
                return;
            }

            var fromAcc = currentUser.Accounts[srcIdx - 1];

            // Ask for recipient details
            Console.Clear();
            Console.WriteLine("=== ENTER RECIPIENT DETAILS ===\n");

            Console.Write("Recipient name: ");
            var recipientName = Console.ReadLine()?.Trim();

            Console.Write("Recipient account number: ");
            var toAccNumber = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(toAccNumber))
            {
                Console.WriteLine("✗ Invalid recipient account number.");
                Console.ReadKey();
                return;
            }

            var toAccount = bank.FindAccount(toAccNumber);
            if (toAccount == null)
            {
                Console.WriteLine("✗ Recipient account not found.");
                Console.ReadKey();
                return;
            }

            if (toAccount.Owner == currentUser)
            {
                Console.WriteLine("✗ This account belongs to you. Use 'own account' transfer instead.");
                Console.ReadKey();
                return;
            }

            Console.Write($"\nAmount ({fromAcc.Currency}): ");
            var amountRaw = Console.ReadLine();
            if (!decimal.TryParse(amountRaw, out var amount))
            {
                Console.WriteLine("✗ Invalid amount.");
                Console.ReadKey();
                return;
            }

            var ok = TransferToOther(currentUser, fromAcc.AccountNumber, toAccNumber!, amount);

            if (ok)
            {
                Console.WriteLine($"✓ Successfully transferred {amount} {fromAcc.Currency} from {fromAcc.AccountNumber} to {recipientName} ({toAccNumber}).");
            }
            else
            {
                Console.WriteLine("✗ Transfer failed.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }






        public bool TransferOwn(User user, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (user == null)
                return false;

            var from = bank.FindAccount(fromAccountNumber);
            var to = bank.FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                Console.WriteLine("Transfer: One or both accounts not found.");
                return false;
            }

            if (from.Owner != user || to.Owner != user)
            {
                Console.WriteLine("Transfer: You can only transfer between your own accounts.");
                return false;
            }

            if (!HasCoverage(from, amount))
            {
                Console.WriteLine("Transfer: Insufficient funds.");
                return false;
            }
            decimal convertedAmount = amount;
            if (from.Currency != to.Currency)
            {
                convertedAmount = exchangeRateService.ConvertCurrency(amount, from.Currency, to.Currency);
            }
            from.Withdraw(amount);
            to.Deposit(convertedAmount);

            Console.WriteLine($"Transfer: {amount} {from.Currency} transferred from {fromAccountNumber} to {toAccountNumber}.");
            return true;
        }





        public bool TransferToOther(User sender, string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (sender == null)
                return false;

            var from = bank.FindAccount(fromAccountNumber);
            var to = bank.FindAccount(toAccountNumber);

            if (from == null || to == null)
            {
                Console.WriteLine("Transfer: One or both accounts not found.");
                return false;
            }

            if (from.Owner != sender)
            {
                Console.WriteLine("Transfer: You can only send money from your own account.");
                return false;
            }

            if (to.Owner == sender)
            {
                Console.WriteLine("Transfer: Use the 'own account' option instead.");
                return false;
            }

            if (!HasCoverage(from, amount))
            {
                Console.WriteLine("Transfer: Insufficient funds or overdraft limit reached.");
                return false;
            }
            decimal convertedAmount = amount;
            if (from.Currency != to.Currency)
            {
                convertedAmount = exchangeRateService.ConvertCurrency(amount, from.Currency, to.Currency);
            }
            from.Withdraw(amount);
            to.Deposit(convertedAmount);

            Console.WriteLine($"Transfer: {amount} {from.Currency} sent from {sender.Name} ({from.AccountNumber}) to {to.Owner.Name} ({to.AccountNumber}).");
            return true;

        }




        private bool HasCoverage(Account from, decimal amount)
        {
            if (from is CheckingAccount ca)
                return (from.Balance - amount) >= -ca.OverdraftLimit;
            else
                return (from.Balance - amount) >= -1000m; // allow up to -1,000 overdraft
        }
    }
}
