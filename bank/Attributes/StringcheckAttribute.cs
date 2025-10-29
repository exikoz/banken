using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace bank.Attributes
{

    /*
     * This attribute acts like a label. We annotate a method in this case
     * with the attribute, and then using reflection we can perform logic
     * on the labelled method. 
     * 
     * https://learn.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy?view=net-9.0
     * 
     */

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateInput : Attribute
    {
        public string? Name { get; }
        public string? Min { get; set; }
        public string? Max { get; set; }

        private static decimal _minDecimal { get; set; } = 0;

        private static decimal _maxDecimal { get; set; } = 99999;



        public ValidateInput(){}

        public static object? Call(object instance, string methodName, params object[] args)
        {
            var method = instance.GetType().GetMethod(methodName);
            var attr = method.GetCustomAttribute<ValidateInput>();
            var parameters = method.GetParameters();

            if(attr.Min != null)
            {
                _minDecimal = decimal.Parse(attr.Min);
            }


            if (attr.Max != null)
            {
                _maxDecimal = decimal.Parse(attr.Max);
            }


            for (int i  = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var dataType = param.ParameterType;
                var value = args[i];


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

                if(dataType == typeof(int))
                {
                    var val = (int)value;
                    if (val <= 0 || val > 999999)
                    {
                        Console.WriteLine("No less than 0 or higher than 999999");
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