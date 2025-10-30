using System;

namespace bank.Utils
{
    /// <summary>
    /// Utility class for colored console output
    /// Follows Single Responsibility Principle - handles only console formatting
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Write success message in green
        /// </summary>
        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Write error message in red
        /// </summary>
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Write header with simple format
        /// </summary>
        public static void WriteHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n=== {title} ===\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Clear screen and reset colors
        /// </summary>
        public static void ClearScreen()
        {
            Console.Clear();
            Console.ResetColor();
        }
    }
}