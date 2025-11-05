using System;

namespace bank.Core
{
    public class Transaction
    {
        // Core fields that always exist for a transaction
        public string Id { get; }
        public string AccountNumber { get; }
        public DateTime TimeStamp { get; } = DateTime.Now;
        public DateTime ProcessDuration => TimeStamp.AddMinutes(1);

        public bool IsProcessed => DateTime.Now > ProcessDuration;


        public string Type { get; } // Deposit | Withdraw | Transfer
        public decimal Amount { get; }
        public bool IsInternal { get; set; }


        // Optional metadata to describe the context of the transaction
        public string? Currency { get; set; }
        public string? AccountType { get; set; }    // Checking, Savings, etc.
        public string? FromAccount { get; set; }    // For transfers
        public string? ToAccount { get; set; }      // For transfers
        public string? FromUser { get; set; }       // For transfers
        public string? ToUser { get; set; }         // For transfers

        // Pending transfer support
        public bool IsPending { get; set; }         // Marks if transaction is pending
        public DateTime? ReleaseAt { get; set; }    // When it should complete
        public string? Status { get; set; }         // Pending | Completed


        // Basic constructor used for simple deposit and withdraw operations
        public Transaction(string id, string accountNumber, DateTime timeStamp, string type, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(id))
                Console.WriteLine("Transaction: Id is required.");
            if (string.IsNullOrWhiteSpace(accountNumber))
                Console.WriteLine("Transaction: Account number is required.");
            if (string.IsNullOrWhiteSpace(type))
                Console.WriteLine("Transaction: Type is required.");
            if (amount <= 0)
                Console.WriteLine("Transaction: Amount must be greater than zero.");

            Id = id;
            AccountNumber = accountNumber;
            TimeStamp = timeStamp;
            Type = type;
            Amount = amount;

            // Default pending values for standard transactions
            IsPending = false;
            Status = "Completed";
            ReleaseAt = null;
        }


        // Extended constructor used when more context is needed (for example transfers)
        public Transaction(
            string id,
            string accountNumber,
            DateTime timeStamp,
            string type,
            decimal amount,
            string? currency,
            string? accountType,
            string? fromAccount,
            string? toAccount,
            string? fromUser,
            string? toUser)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(id))
                Console.WriteLine("Transaction: Id is required.");
            if (string.IsNullOrWhiteSpace(accountNumber))
                Console.WriteLine("Transaction: Account number is required.");
            if (string.IsNullOrWhiteSpace(type))
                Console.WriteLine("Transaction: Type is required.");
            if (amount <= 0)
                Console.WriteLine("Transaction: Amount must be greater than zero.");

            Id = id;
            AccountNumber = accountNumber;
            TimeStamp = timeStamp;
            Type = type;
            Amount = amount;

            // Optional context fields
            Currency = currency;
            AccountType = accountType;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            FromUser = fromUser;
            ToUser = toUser;

            // Default pending values
            IsPending = false;
            Status = "Completed";
            ReleaseAt = null;
        }
    }
}
