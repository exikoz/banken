using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace bank.Utils
{
    public static class ValidationHelper
    {

        // Generic method, can take all kinds of datatypes
        public static bool IsValid<T>(T data)

        {
            string name = typeof(T).Name ?? "Data"; // checks the name of the field for output info ( example if T = (string username) then name = "username")


            // Perform a nullcheck first, because all of them can be null, better to throw early return in that case
            if (data == null)
            {
                ConsoleHelper.WriteWarning($"{name} cannot be null!");
                return false;
            }


            // We check what datatype the input is, and return the appropriate error handling on each case. 
            // The point is that the input will only trigger on matching if statement and pass the others

            if (data is string s) return StringCheck(s);
            if (data is long l) return MinMax(0L, long.MaxValue, l);
            if (data is int i) return MinMax(0, int.MaxValue, i);
            if (data is decimal d) return MinMax(0m, decimal.MaxValue, d);
            if (data is Array a) return FixedSizeContainerCheck(a);
            if (data is ICollection c) return  DynamicContainerCheck(c);
            
            return true;
        }


        // Here we just perform standard null or whitespace checks, and set a range. For our console app I decided 90 to be enough. 
        public static bool StringCheck(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                ConsoleHelper.WriteWarning($"Input cannot be empty!");
                return false;
            }
            if (input.Length > 90)
            {
                ConsoleHelper.WriteWarning($"Too many characters! Max amount of character allowed is 90");
                return false;
            }
            return true;

        }


        /* 
         * Fixed size containers use .Length instead of Count, this is because
           fixed size containers takes up a predefined space in memory. 
           So ".Length()" works here because it just reads the memory of the whole
           object, since the size never changes during runtime. 
         */
        public static bool FixedSizeContainerCheck(Array container)
        {
            if(container.Length == 0)
            {
                ConsoleHelper.WriteWarning($"List is empty!");
                return false;
            }
            return true;

        }


        /* 
         * Ín comparison to above, we  have to count each element in the containers
         * that are dynamic, since the sizes are not fixed. 
         */
        public static bool DynamicContainerCheck(ICollection container)
        {
            if(container.Count == 0)
            {
                ConsoleHelper.WriteWarning($"List is empty!");
                return false;
            }
            return true;
        }


        /*
         * 
         * Just a general helper for min and max value checks. 
         * Can technically be reused throughout the application, but
         * is mainly used within the class. 
         * 
         * IComparable interface performs a check during runtime so that the two
         * values being compared are comparable. This interface is implemented by
         * default by int,long, decimal etc.
         */


        public static bool MinMax<T>(T min, T max,T value) where T : IComparable<T>
        {
            if(value.CompareTo(min) < 0 )
            {
                ConsoleHelper.WriteWarning($"Value cannot be lower than {min}");
                return false;
            }
            if (value.CompareTo(max) > 0)
            {
                ConsoleHelper.WriteWarning($"Value cannot be higher than {max}");
                return false;
            }

            return true;
        }


        public static void TestSelf()
        {
            TestingInput(input =>
            {
                bool result = IsValid(input);
                Console.WriteLine($"Result is: {result}");

            }
            );
        }


        // A helper method made for testing all kinds of forbidden input. 
        public static void TestingInput(Action<object> action)
        {
            var IlegalInput = new List<object>
            {
                null,
                "",
                " ",
                -1,
                0,
                int.MaxValue,
                long.MaxValue,
                0L,
                decimal.MaxValue,
                new int[0],
                new List<string>(),
              };



            foreach (var input in IlegalInput)
            {
                action.Invoke(input);
            }
        }
    }
}
