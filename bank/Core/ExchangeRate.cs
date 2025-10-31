namespace bank.Core
{
    public class ExchangeRate
    {
        public CurrencyCode Code { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public decimal Rate { get; set; }


        public ExchangeRate() { }
        public ExchangeRate(CurrencyCode code, decimal rate)
        {
            Code = code;
            Rate = rate;
        }


    }


    public enum CurrencyCode
    {
        SEK = 0,
        USD = 1,
        GBP = 2,
        JPY = 4,
        CHF = 5,
        AUD = 6,
        CAD = 7,
        NOK = 8,
        DKK = 9
    }
}
