namespace bank.Core
{
    public class ExchangeRate
    {
        public string Code { get; set; }
        public DateTime LastUpdated { get; set; }
        public Dictionary<CurrencyCode, decimal> Rates { get; set; } = new();

    }


    public enum CurrencyCode
    {
        SEK = 0,
        USD = 1,
        GBP = 2,
    }
}
