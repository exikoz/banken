using System;

namespace bank.Utils
{
    /// <summary>
    /// Helper class for console output with colors and formatting
    /// Provides consistent styling across the application
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Write a success message in green
        /// </summary>
        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Write an error message in red
        /// </summary>
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Write a warning message in yellow
        /// </summary>
        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Write an info message in cyan
        /// </summary>
        public static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Write a header with a border
        /// </summary>
        public static void WriteHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            int width = Math.Max(title.Length + 8, 50);
            string border = new string('═', width);

            Console.WriteLine($"╔{border}╗");
            Console.WriteLine($"║   {title.PadRight(width - 3)}║");
            Console.WriteLine($"╚{border}╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Write a simple header with equal signs
        /// </summary>
        public static void WriteSimpleHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n=== {title} ===\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Write a menu option with highlighting
        /// </summary>
        public static void WriteMenuOption(string number, string description)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{number}. ");
            Console.ResetColor();
            Console.WriteLine(description);
        }

        /// <summary>
        /// Write a separator line
        /// </summary>
        public static void WriteSeparator(char character = '-', int length = 50)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(character, length));
            Console.ResetColor();
        }

        /// <summary>
        /// Write highlighted text
        /// </summary>
        public static void WriteHighlight(string message, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Prompt for input with a colored prompt
        /// </summary>
        public static string Prompt(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}: ");
            Console.ResetColor();
            return Console.ReadLine() ?? "";
        }

        /// <summary>
        /// Display account balance with color based on amount
        /// </summary>
        public static void WriteBalance(decimal balance)
        {
            if (balance > 5000)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (balance > 1000)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (balance >= 0)
                Console.ForegroundColor = ConsoleColor.White;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"Balance: {balance:C}");
            Console.ResetColor();
        }

        /// <summary>
        /// Wait for user to press any key with a colored message
        /// </summary>
        public static void PauseWithMessage(string message = "Press any key to continue...")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Clear console and reset colors
        /// </summary>
        public static void ClearScreen()
        {
            Console.Clear();
            Console.ResetColor();
        }

        /// <summary>
        /// Write a box around text
        /// </summary>
        public static void WriteBox(string message, ConsoleColor borderColor = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = borderColor;
            int width = message.Length + 4;
            string border = new string('─', width);

            Console.WriteLine($"┌{border}┐");
            Console.WriteLine($"│  {message}  │");
            Console.WriteLine($"└{border}┘");
            Console.ResetColor();
        }

        /// <summary>
        /// Write transaction with appropriate color
        /// </summary>
        public static void WriteTransaction(string type, decimal amount)
        {
            if (type.ToLower() == "deposit")
                Console.ForegroundColor = ConsoleColor.Green;
            else if (type.ToLower() == "withdraw")
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"{type}: {amount:C}");
            Console.ResetColor();
        }
    }
}