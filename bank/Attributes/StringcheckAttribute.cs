using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace bank.Attributes
{


    // Alexander - 2025-10-29


    /*
     * This attribute acts like a label. We annotate a method in this case
     * with the attribute, and then using reflection we can perform logic
     * on the labelled method. 
     * 
     * https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy?view=net-9.0
     * 
     */

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)] // only to be placed on methods
    public class ValidateInput : Attribute
    {
        public string? Name { get; }

        // Incase we want to set a range between x to y OR just a min / max amount
        public string? Min { get; set; }
        public string? Max { get; set; }




        public ValidateInput(){}



        // This method takes the instance, methodname and (nullable) arguments of method. 
        public static object? Call(object instance, string methodName, params object?[] args)
        {

               // I use these for decimals so we can hold both default and non-default values for checking range of numnbers (not less than or more than)
              decimal _minDecimal = 0;

              decimal _maxDecimal = 99999;

              int _minInt = 0;

              int _maxInt  = 99999;

            // We fetch the method of the instance
            var method = instance.GetType().GetMethod(methodName);

            // Then we can fetch the attribute on the method (for checking custom error handling like min max)
            var attr = method.GetCustomAttribute<ValidateInput>();

            // We also fetch the parameters of the method so we know what kind of input we are validating
            var parameters = method.GetParameters();

            if (attr == null) { return null; } // This just makes sure nothing happens if attribute isnt placed but this is called for some reason


            // If dev added their own minimum or/and max amount to decimal we set the private fields to that value
            if(attr.Min != null) _minDecimal = decimal.Parse(attr.Min);


            // Since this is written by the developer in code, I didn't think it was necessary to use a safer parsing.
            if (attr.Max != null) _maxDecimal = decimal.Parse(attr.Max); 


            // Now we iterate the parameters of the method
            for (int i  = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                // Get which datatype it is and the name of it ( example string username = datatype:string , value:username )
                var dataType = param.ParameterType;
                var value = args[i];


                // Here we check the different cases because validation is treated differently on different datatypes.
                // If we want to make even more validation on classes (like User) etc, that is possible. 

                if (dataType == typeof(string))
                {
                    var str = value as string;
                    if(string.IsNullOrWhiteSpace(str))
                    {
                        Console.WriteLine($"{param.Name} cannot be empty!");
                        return null;
                    }
                }

                if (dataType == typeof(decimal))
                {
                    var dec = (decimal)value;

                    if (dec <= _minDecimal || dec > _maxDecimal)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{param.Name} cannot be less than {_minDecimal} or higher than  {_maxDecimal}");
                        Console.ResetColor();
                        return null;
                    }
                }

                if (dataType == typeof(int) )
                {
                    var val = (int)value;
                    if (val <= _minInt || val > _maxInt)
                    {
                        Console.WriteLine($"{param.Name} cannot be less than {_minInt} or higher than  {_maxInt}");
                        return null;
                    }
                }
            }
            return method.Invoke(instance, args);
        }

    }


    /* 


    public class StringcheckFactory<T> : DispatchProxy
    {
    


        private  T? _proxyObject;
        public static T CreateProxy(T prox)
        {
            var p = Create<T, StringcheckFactory<T>>()!;
            var proxyInstance = (StringcheckFactory<T>)(object)p;
            proxyInstance._proxyObject = prox;
            return p;
        }




        public static object? LogMethod(object instance, string methodName, object?[]? arguments)
        {
            // Currently accepted access
            BindingFlags accessRules = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

            var method = instance.GetType().GetMethod(methodName, accessRules);

            if (method == null)  throw new MissingMethodException("Not found");

            var attribute = method!.GetCustomAttribute<ValidateInput>();
            if (attribute != null) 
            {
                Console.WriteLine($"{method.Name} was called at {DateTime.Now} + {attribute.Name}");
            }
            return method.Invoke(instance, arguments);  
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            throw new NotImplementedException();
        }
    }


    */
}