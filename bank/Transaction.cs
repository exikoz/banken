using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    Console.WriteLine($"Transaction: Id: {id} is not valid input. It must contain something!!");
    if (string.IsNullOrWhiteSpace(accountNumber))
    Console.WriteLine($"Transaction: Account number: {accountNumber} is not valid input. It must contain something!!");
    if (string.IsNullOrWhiteSpace(type))
    Console.WriteLine($"Transaction: Transaction type: {type} is not a valid option.");

        Id = id;
        AccountNumber = accountNumber;
        TimeStamp = timeStamp; 
        Type = type;
    }
}
}


