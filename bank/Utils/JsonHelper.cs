using bank.Core;
using bank.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace bank.Utils
{

    // Alexander 2025-10-28

    public class JsonHelper
    {

        /*

        public void CreateJson<T>(T data, string? path = null)
        {
            // Using typeof to fetch the name of the data, using this to title the file. 
            // Example if we would create a json doc for ExchangeRates data class, the file would be called
            // "ExchangeRates.json" 
            string FileName = typeof(T).Name + ".json";

            // If no other path is specified in argument then the file will be created in the root folder (where the .exe/.dll is)
            string _defaultPath = path ?? Path.Combine(AppContext.BaseDirectory, FileName);
            
            if (File.Exists(_defaultPath)) return;

            File.WriteAllText(_defaultPath, data);


        }

        */

        /*
         * Params: T data = any data to be added to the file
         *         path = where the file is located. If file doesnt exist it will be created on path.
         */
        public void WriteJson<T>(T data, string? path = null)
        {

            string FileName = typeof(T).Name + ".json";
            string _defaultPath = path ?? Path.Combine(AppContext.BaseDirectory, FileName);


            var output = JsonSerializer.Serialize(data,new JsonSerializerOptions { WriteIndented = true }); // Adds indentations to the JSON data

            if (!File.Exists(_defaultPath)) File.WriteAllText(_defaultPath, output);

            var json = File.ReadAllText(_defaultPath);
            File.WriteAllText(_defaultPath, output);
            Console.WriteLine($"Json saved to this path: {_defaultPath}"); // TODO switch with the response helper method later on
        }


        public void RemoveJson()
        {

        }


        // This helper takes in any query and file to find data in json.
        // It works by deserialising the json doc into a List and then we use LINQ to find the data

        public static IEnumerable<T> DeserializeJson<T>(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<List<T>>(jsonData) ?? new List<T>(); // Returns empty list instead of null to avoid annoying crashes
            return data;
        }



        // LINQ HELPERS

        public static IEnumerable<T> FindByQueryJson<T>(Func<T, bool> query, string filePath)
        {
            return DeserializeJson<T>(filePath).Where(query);
        }






    }
}
