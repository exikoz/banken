namespace bank.Core
{
    public class Transaction
    {
        public string Id { get; }
        public string AccountNumber { get; }
        public DateTime TimeStamp { get; }
        public string Type { get; } // "Deposit" | "Withdraw"
        public decimal Amount { get; }
        //rekommenderad, med Amount
        public Transaction(string id, string accountNumber, DateTime timeStamp, string type, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(id))
                Console.WriteLine($"Transaction: Id: {id} is not valid input. It must contain something!!");
            if (string.IsNullOrWhiteSpace(accountNumber))
                Console.WriteLine($"Transaction: Account number: {accountNumber} is not valid input. It must contain something!!");
            if (string.IsNullOrWhiteSpace(type))
                Console.WriteLine($"Transaction: Transaction type: {type} is not a valid option.");
            if (amount <= 0)
                Console.WriteLine("Transaction: Amount must be greater than 0.");

            Id = id;
            AccountNumber = accountNumber;
            TimeStamp = timeStamp;
            Type = type;
            Amount = amount;
        }
  
    }
}
