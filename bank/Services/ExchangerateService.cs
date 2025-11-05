using bank.Core;
using bank.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bank.Services
{
    public class ExchangerateService
    {
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "docs", "exchangeRates.json");

        public JsonHelper jsonHelper = new JsonHelper();

        public ExchangeRate rates = new ExchangeRate();
        public ExchangerateService()
        {
            var folder = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (!File.Exists(filePath))
            {
                initJson();
            }
        }
            

        public IEnumerable<ExchangeRate> getAllRates()
        {
            return JsonHelper.DeserializeJson<ExchangeRate>(filePath);
        }



        public decimal ConvertCurrency(decimal amount, string from, string to)
        {
            var rates = getAllRates().ToList();
            Enum.TryParse(from, true, out CurrencyCode fromCode);
            Enum.TryParse(to, true, out CurrencyCode toCode);
            var fromRate = rates.FirstOrDefault(r => r.Code == fromCode);
            var toRate = rates.FirstOrDefault(r => r.Code == toCode);
            if (fromRate == null || toRate == null) Console.WriteLine("Invalid currency code.");
            decimal amountInBase = amount * fromRate.Rate;
            decimal result = amountInBase / toRate.Rate;
            return Math.Round(result, 2);

        }
        public void DeleteField(CurrencyCode code)
        {
            string fileName = typeof(ExchangeRate).Name + ".json";
            string filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            jsonHelper.DeleteByPredicate<ExchangeRate>(
                r => r.Code == code && r.CustomCode == null,
                filePath
            );
        }

        public void DeleteCustomField(string customCode)
        {
            string fileName = typeof(ExchangeRate).Name + ".json";
            string filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            jsonHelper.DeleteByPredicate<ExchangeRate>(
                r => r.CustomCode != null && r.CustomCode.Equals(customCode, StringComparison.OrdinalIgnoreCase),
                filePath
            );
        }
        public void CreateFile(ExchangeRate rates)
        {
            jsonHelper.WriteJson(rates, filePath);
        }

        public void AddRates(ExchangeRate rate) 
        { 
            if (rate == null) Console.WriteLine("Rate cannot be null.");
            jsonHelper.UpdateJson(c => c.Code == rate.Code, rate, filePath);
        }

        // Only used as a safety measure if the file doesnt exist
        public void initJson()
        {
            var defaultRates = new List<ExchangeRate>()
                {
                    new ExchangeRate(CurrencyCode.SEK, 1.00m),
                    new ExchangeRate(CurrencyCode.USD, 0.091m),
                    new ExchangeRate(CurrencyCode.GBP, 0.078m),
                    new ExchangeRate(CurrencyCode.JPY, 13.50m),
                    new ExchangeRate(CurrencyCode.CHF, 0.087m),
                    new ExchangeRate(CurrencyCode.AUD, 0.60m),
                    new ExchangeRate(CurrencyCode.CAD, 0.65m),
                    new ExchangeRate(CurrencyCode.NOK, 0.095m),
                    new ExchangeRate(CurrencyCode.DKK, 0.13m)
                };
            var json = System.Text.Json.JsonSerializer.Serialize(defaultRates, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Created exchangeRates.json at {filePath}");

        }

        // Converts any currency amount to SEK based on exchangeRates.json
        public decimal ConvertToSek(string currencyCode, decimal amount)
        {
            if (string.Equals(currencyCode, "SEK", StringComparison.OrdinalIgnoreCase))
                return amount; // Base currency, no conversion needed

            var rates = getAllRates();
            var rate = rates.FirstOrDefault(r =>
                (r.CustomCode != null && r.CustomCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
                || r.Code.ToString().Equals(currencyCode, StringComparison.OrdinalIgnoreCase));

            if (rate == null || rate.Rate <= 0)
            {
                Console.WriteLine($"Warning: No valid exchange rate found for {currencyCode}. Assuming 1:1.");
                return amount;
            }

            // Convert to SEK
            return amount * rate.Rate;
        }

    }
}
