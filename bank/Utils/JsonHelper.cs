using bank.Core;
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


        // If no other path is specified in argument then the file will be created in the root folder (where the .exe/.dll is)
        private readonly string _defaultPath = Path.Combine(AppContext.BaseDirectory, "ExchangeRates.json");

        /*
         * Generic
         */
        public void createJsonFile<T>(T data,string? path = null)
        {

            string targetPath = path ?? _defaultPath; 






            var output = JsonSerializer.Serialize
                (
                data,
                new JsonSerializerOptions { WriteIndented = true } // Adds indentations to the JSON data
                );
            File.WriteAllText( path, output );
            Console.WriteLine($"Json saved to this path: {path}"); // TODO switch with the response helper method later on
        }


        public void writeToJson<T>(T data, string? path = null)
        {
            string targetPath = path ?? _defaultPath;

        }


        public void removeFromJsonFile()
        {

        }


        public void findByCode(string Code)
        {

        }


    }
}
