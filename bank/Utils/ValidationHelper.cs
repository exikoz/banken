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

        public static bool IsValid<T>(T data)

        {
            string name = typeof(T).Name ?? "Data";

            if (data == null)
            {
                ConsoleHelper.WriteWarning($"{name} cannot be null!");
                return false;
            }

            if (data is string s) return StringCheck(s);
            if (data is long l) return MinMax(0L, long.MaxValue, l);
            if (data is int i) return MinMax(0, int.MaxValue, i);
            if (data is decimal d) return MinMax(0m, decimal.MaxValue, d);
            if (data is ICollection c) return  DynamicContainerCheck(c);
            if (data is Array a) return FixedSizeContainerCheck(a);
            
            return true;
        }



        public static bool StringCheck(string container)
        {
            if(string.IsNullOrEmpty(container))
            {
                ConsoleHelper.WriteWarning($"Input cannot be empty!");
                return false;
            }
            return true;

        }

        public static bool FixedSizeContainerCheck(Array container)
        {
            if(container.Length == 0)
            {
                ConsoleHelper.WriteWarning($"List is empty!");
                return false;
            }
            return true;

        }

        public static bool DynamicContainerCheck(ICollection container)
        {
            if(container.Count == 0)
            {
                ConsoleHelper.WriteWarning($"List is empty!");
                return false;
            }
            return true;
        }

        public static bool MinMax<T>(T min, T max,T value) where T : IComparable<T> // IComparable checks that the values can be compared during compil time
        {
            if(value.CompareTo(min) <0 )
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
    }
}
