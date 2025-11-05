using bank.Core;
using bank.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace bank.Utils
{

    // Alexander 2025-11-05

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
        public void WriteJson<T>(T data, string? name = null)
        {



            string FileName = name ?? typeof(T).Name + ".json";
            string _defaultPath =  Path.Combine(AppContext.BaseDirectory, FileName);
            List<T> existingData = new List<T>();


            if (File.Exists(_defaultPath))
            {

                try
                {
                    string json = File.ReadAllText(_defaultPath);
                    existingData = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
                catch
                {
                    existingData = new List<T>(); // Sends empty list so we dont get any crashes
                }

                existingData.Add(data);
                string output = JsonSerializer.Serialize(existingData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_defaultPath, output);
                ConsoleHelper.WriteSuccess($"Json saved to this path: {_defaultPath}");
            }
            else
            {
                existingData.Add(data);
                string output = JsonSerializer.Serialize(existingData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_defaultPath, output);
                ConsoleHelper.WriteSuccess($"Json saved to this path: {_defaultPath}"); // TODO switch with the response helper method later on
            }
        }
        


        // This helper takes in any query and file to find data in json.
        // It works by deserialising the json doc into a List and then we use LINQ to find the data

        public static List<T> DeserializeJson<T>(string filePath)
        {

            if (!File.Exists(filePath)) return new List<T>();
            var jsonData = File.ReadAllText(filePath);

            if (jsonData == null)
                return new List<T>();

            try
            {
                return JsonSerializer.Deserialize<List<T>>(jsonData) ?? new List<T>(); ; // Returns empty list instead of null to avoid annoying crashes

            }
            catch (JsonException e)
            {
                ConsoleHelper.WriteError($"Json couldn't deserialize the file! Info: {e.Message} ");
                return new List<T>();
            }

        }



        public void RemoveJsonByField<T>(Func<T, bool> field, string filePath)
        {


            if (!File.Exists(filePath))
            {
                ConsoleHelper.WriteWarning("File not found, nothing to remove.");

                return;
            }


            var dataSet = DeserializeJson<T>(filePath);
            if (dataSet == null || dataSet.Count == 0)
            {
                ConsoleHelper.WriteWarning("No data found.");
                return;
            }


            var exists = dataSet.FirstOrDefault(field);
            if (exists == null)
            {
                ConsoleHelper.WriteWarning("No data to remove. ");
                return;
            }

            dataSet.Remove(exists);
            //File.Copy(filePath, filePath + ".bak", overwrite: true);

            string output = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, output);

            ConsoleHelper.WriteSuccess("Data successfully removed!");
        }






        public void UpdateJson<T>(Func<T, bool> fieldToUpdate, T newData, string filePath)
        {

            if (fieldToUpdate == null)
            {
                ConsoleHelper.WriteWarning("Field to update required.");
                return;
            };

            if (!File.Exists(filePath))
            { 
                WriteJson(newData, filePath);
                return;
            }

            var dataSet = DeserializeJson<T>(filePath);




            if (dataSet == null)
            {
               Console.WriteLine("No data found.");
                return;
            };


            var exists = dataSet.FirstOrDefault(fieldToUpdate);
            if (exists == null)
            {
                dataSet.Add(newData);
            }

            else
            {
                var matching = dataSet.IndexOf(exists);
                if (matching >= 0)
                {
                    dataSet[matching] = newData;
                }
                else
                    dataSet.Add(newData);

            }
            string output = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, output);
            ConsoleHelper.WriteSuccess("Data successfully updated!");
        }


    }
}
