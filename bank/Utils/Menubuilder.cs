using System;
using System.Collections.Generic;

namespace bank.Utils
{
    /// <summary>
    /// Builds and manages menu options dynamically
    /// Follows Single Responsibility Principle - handles only menu creation and execution
    /// </summary>
    public class MenuBuilder
    {
        private List<(string description, Action action)> options = new();

        /// <summary>
        /// Add a menu option - numbering is automatic
        /// </summary>
        public MenuBuilder AddOption(string description, Action action)
        {
            options.Add((description, action));
            return this; // Fluent interface
        }

        /// <summary>
        /// Display menu and get user choice
        /// </summary>
        public void Display()
        {
            // Display all options with automatic numbering
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i].description}");
            }

            Console.Write("\nChoose option: ");
            var choice = Console.ReadLine();

            // Execute chosen option
            if (int.TryParse(choice, out int index) && index > 0 && index <= options.Count)
            {
                options[index - 1].action();
            }
            else
            {
                ConsoleHelper.WriteError("Invalid choice. Press any key to try again.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Clear all options (useful for rebuilding menu)
        /// </summary>
        public void Clear()
        {
            options.Clear();
        }
    }
}