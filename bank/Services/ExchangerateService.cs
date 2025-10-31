using bank.Core;
using bank.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Services
{
    public class ExchangerateService
    {

        private readonly string filePath = "exchangeRates.json";
        public JsonHelper jsonHelper = new JsonHelper();

        public ExchangeRate rates = new ExchangeRate();


        public IEnumerable<ExchangeRate> getAllRates()
        {
            return JsonHelper.DeserializeJson<ExchangeRate>(filePath);
        }



        public decimal ConvertCurrency(decimal amount, CurrencyCode from, CurrencyCode to)
        {
            var rates = getAllRates().ToList();
            var fromRate = rates.FirstOrDefault(r => r.Code == from);
            var toRate = rates.FirstOrDefault(r => r.Code == to);
            if (fromRate == null || toRate == null) Console.WriteLine("Invalid currency code.");
            decimal amountInBase = amount * fromRate.Rate;
            decimal result = amountInBase / toRate.Rate;
            return Math.Round(result, 2);

        }



        // Admin functions  - todo add admin
        public void CreateFile(ExchangeRate rates)
        {
            jsonHelper.WriteJson(rates, filePath);
        }

        public void AddRates(ExchangeRate rate) 
        { 
            jsonHelper.UpdateJson<ExchangeRate>(c => c.Code == rate.Code, rate, filePath);
        }




    }
}
