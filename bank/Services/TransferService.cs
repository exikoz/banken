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

            while (true)
            {
                // Filtrera endast konton med saldo > 0
                var sourceAccounts = currentUser.Accounts.Where(a => a.Balance > 0).ToList();
                if (!sourceAccounts.Any())
                {
                    Console.WriteLine("No accounts with positive balance available for transfer.");
                    Console.ReadKey();
                    return;
                }

                Console.Clear();
                Console.WriteLine("=== SELECT SOURCE ACCOUNT ===\n");
                for (int i = 0; i < sourceAccounts.Count; i++)
                {
                    var acc = sourceAccounts[i];
                    Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
                }

                Console.Write("\nChoose source (number): ");
                if (!int.TryParse(Console.ReadLine(), out var srcIdx) || srcIdx < 1 || srcIdx > sourceAccounts.Count)
                {
                    Console.WriteLine("\nInvalid selection. Please try again.");
                    Console.ReadKey();
                    continue; // börja om i loopen istället för att avsluta
                }

                var fromAcc = sourceAccounts[srcIdx - 1];

                // Välj mål-konto (alla andra konton, även tomma)
                var destinationAccounts = currentUser.Accounts.Where(a => a.AccountNumber != fromAcc.AccountNumber).ToList();

                Console.Clear();
                Console.WriteLine("=== SELECT DESTINATION ACCOUNT ===\n");
                for (int i = 0; i < destinationAccounts.Count; i++)
                {
                    var acc = destinationAccounts[i];
                    Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
                }

                Console.Write("\nChoose destination (number): ");
                if (!int.TryParse(Console.ReadLine(), out var dstIdx) || dstIdx < 1 || dstIdx > destinationAccounts.Count)
                {
                    Console.WriteLine("\nInvalid selection. Please try again.");
                    Console.ReadKey();
                    continue;
                }

                var toAcc = destinationAccounts[dstIdx - 1];

                Console.Write($"\nAmount ({fromAcc.Currency}): ");
                if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
                {
                    Console.WriteLine("\nInvalid amount. Please enter a positive number.");
                    Console.ReadKey();
                    continue;
                }

                if (amount > fromAcc.Balance)
                {
                    Console.WriteLine("\nInsufficient funds in selected account.");
                    Console.ReadKey();
                    continue;
                }

                // Utför valutakonvertering om nödvändigt
                decimal convertedAmount = amount;
                if (fromAcc.Currency != toAcc.Currency)
                {
                    convertedAmount = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAcc.Currency);
                    Console.WriteLine($"\nCurrency converted: {amount:N2} {fromAcc.Currency} = {convertedAmount:N2} {toAcc.Currency}");
                }

                // Genomför överföringen
                fromAcc.Withdraw(amount);
                toAcc.Deposit(convertedAmount);

                Console.WriteLine($"\n✓ Transfer completed: {amount:N2} {fromAcc.Currency} from {fromAcc.AccountNumber} to {toAcc.AccountNumber}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                break;
            }
        }


        // Reworked to show user's own accounts selection directly, then ask recipient details
        public void DoTransferToOther(User currentUser)
        {
            Console.Clear();
            Console.WriteLine("=== TRANSFER TO ANOTHER CUSTOMER ===\n");

            while (true)
            {
                // Visa endast konton med saldo > 0
                var availableAccounts = currentUser.Accounts.Where(a => a.Balance > 0).ToList();
                if (!availableAccounts.Any())
                {
                    Console.WriteLine("You have no accounts with a positive balance to transfer from.");
                    Console.ReadKey();
                    return;
                }

                Console.Clear();
                Console.WriteLine("=== SELECT YOUR ACCOUNT TO SEND FROM ===\n");
                for (int i = 0; i < availableAccounts.Count; i++)
                {
                    var acc = availableAccounts[i];
                    Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Balance: {acc.Balance:N2} {acc.Currency}");
                }

                Console.Write("\nChoose source (number): ");
                if (!int.TryParse(Console.ReadLine(), out var srcIdx) || srcIdx < 1 || srcIdx > availableAccounts.Count)
                {
                    Console.WriteLine("\nInvalid selection. Please try again.");
                    Console.ReadKey();
                    continue; // användaren får försöka igen
                }

                var fromAcc = availableAccounts[srcIdx - 1];

                Console.Clear();
                Console.WriteLine("=== ENTER RECIPIENT DETAILS ===\n");

                Console.Write("Recipient name: ");
                var recipientName = Console.ReadLine()?.Trim();

                Console.Write("Recipient account number: ");
                var toAccNumber = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(toAccNumber))
                {
                    Console.WriteLine("\nInvalid recipient account number.");
                    Console.ReadKey();
                    continue;
                }

                var toAccount = bank.FindAccount(toAccNumber);
                if (toAccount == null)
                {
                    Console.WriteLine("\nRecipient account not found.");
                    Console.ReadKey();
                    continue;
                }

                if (toAccount.Owner == currentUser)
                {
                    Console.WriteLine("\nThis account belongs to you. Use 'transfer between own accounts' instead.");
                    Console.ReadKey();
                    continue;
                }

                Console.Write($"\nAmount ({fromAcc.Currency}): ");
                if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
                {
                    Console.WriteLine("\nInvalid amount. Please enter a positive number.");
                    Console.ReadKey();
                    continue;
                }

                if (amount > fromAcc.Balance)
                {
                    Console.WriteLine("\nInsufficient funds in selected account.");
                    Console.ReadKey();
                    continue;
                }

                // Valutakonvertering om nödvändigt
                decimal convertedAmount = amount;
                if (fromAcc.Currency != toAccount.Currency)
                {
                    convertedAmount = exchangeRateService.ConvertCurrency(amount, fromAcc.Currency, toAccount.Currency);
                    Console.WriteLine($"\nCurrency converted: {amount:N2} {fromAcc.Currency} = {convertedAmount:N2} {toAccount.Currency}");
                }

                // Genomför överföring
                fromAcc.Withdraw(amount);
                toAccount.Deposit(convertedAmount);

                Console.WriteLine($"\n✓ Successfully transferred {amount:N2} {fromAcc.Currency} from {fromAcc.AccountNumber} to {recipientName} ({toAccNumber}).");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                break; // avsluta loopen efter lyckad överföring
            }
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
