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
            Id = id ?? "Transaction: Id kunde inte hittas!!";
            AccountNumber = accountNumber ?? "Transaction: Kontonummer kune inte hittas!";
            TimeStamp = timeStamp;
            Type = type ?? "Typ kunde inte hittas!!!!";
        }


    }

}