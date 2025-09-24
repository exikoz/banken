namespace bank
{
    public class Transaction
    {
        public string Id { get;  }
        public string AccountNumber { get; }

        public DateTime TimeStamp { get; }

        public string Type { get; }

        public Transaction(string id, string accountNumber, DateTime timeStamp, string type)
        {
            if (string.IsNullOrWhiteSpace(id))
                Console.WriteLine($"Transaction: Id: {id} är inte godkänt input. Måste innehålla något!!");

            if (string.IsNullOrWhiteSpace(accountNumber))
                Console.WriteLine($"Transaction: Kontonummer: {accountNumber}  är inte godkänt input. Måste innehålla något!!");
            if (string.IsNullOrWhiteSpace(type))
                Console.WriteLine($"Transaction: Transaktiontyp: {type}  är inte ett möjligt val.");
            Id = id;
            AccountNumber = accountNumber;
            TimeStamp = timeStamp; 
            Type = type;

        }
    }

}