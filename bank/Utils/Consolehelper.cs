using System;

namespace bank.Utils
{
    public static class ConsoleHelper
    {
        // Success message
        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        // Error message
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        // Warning message
        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        // Info message
        public static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }

        // Header with border
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

        // Simple header
        public static void WriteSimpleHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n=== {title} ===\n");
            Console.ResetColor();
        }

        // Menu option
        public static void WriteMenuOption(string number, string description)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{number}. ");
            Console.ResetColor();
            Console.WriteLine(description);
        }

        // Separator
        public static void WriteSeparator(char character = '-', int length = 50)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(character, length));
            Console.ResetColor();
        }

        // Highlighted text
        public static void WriteHighlight(string message, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Prompt for text
        public static string Prompt(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}: ");
            Console.ResetColor();
            return Console.ReadLine() ?? "";
        }

        // Account balance with color
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

        // Pause
        public static void PauseWithMessage(string message = "Press any key to continue...")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        // Clear screen
        public static void ClearScreen()
        {
            Console.Clear();
            Console.ResetColor();
        }

        // Boxed text
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

        // Colored transaction entry
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

        // Prompt with ESC support
        public static string PromptWithEscape(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}: ");
            Console.ResetColor();

            string input = "";

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return "<ESC>";
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        // Masked input (e.g. PIN)
        public static string PromptWithEscapeMasked(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}: ");
            Console.ResetColor();

            string input = "";

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return "<ESC>";
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write("*"); // masked character
                }
            }
        }
    }
}
