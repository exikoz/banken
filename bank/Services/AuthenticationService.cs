using bank.Core;
using System;
using System.Linq;

namespace bank.Services
{
    /// <summary>
    /// Handles ALL authentication-related operations including UI
    /// This class is completely independent from Menu
    /// </summary>
    public class AuthenticationService
    {
        private readonly Bank bank;

        public AuthenticationService(Bank bank)
        {
            this.bank = bank;
        }

        /// <summary>
        /// Shows the complete login UI and handles the login process
        /// Call this from Menu when user selects "Log in"
        /// </summary>
        /// <returns>The logged-in user, or null if login failed/cancelled</returns>
        public User? ShowLoginUI()
        {
            Console.Clear();
            Console.WriteLine("=== LOG IN ===\n");

            Console.Write("Enter User ID: ");
            var userId = Console.ReadLine();

            var user = bank.FindUser(userId);

            if (user == null)
            {
                Console.WriteLine("\n✗ User not found!");
                Console.ReadKey();
                return null;
            }

            // PIN validation with max 3 attempts
            return ValidatePINWithRetries(user);
        }

        /// <summary>
        /// Shows the complete registration UI and handles the registration process
        /// Call this from Menu when user selects "Register"
        /// </summary>
        /// <returns>True if registration was successful</returns>
        public bool ShowRegistrationUI()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTER NEW USER ===\n");

            // Get and validate User ID
            var userId = GetUserId();
            if (userId == null) return false;

            // Get and validate Name
            var name = GetName();
            if (name == null) return false;

            // Get and validate PIN
            var pin = GetAndConfirmPIN();
            if (pin == null) return false;

            // Create and register user
            var newUser = new User(userId, name, pin);
            bank.RegisterUser(newUser);

            Console.WriteLine("\n✓ User registered successfully!");

            // Optionally create an account for the new user
            //OfferAccountCreation(newUser);

            Console.WriteLine("\nYou can now log in with your credentials.");
            Console.ReadKey();
            return true;
        }

        /// <summary>
        /// Validates PIN with a maximum of 3 attempts
        /// </summary>
        private User? ValidatePINWithRetries(User user, int maxAttempts = 3)
        {
            int attempts = 0;
            while (attempts < maxAttempts)
            {
                Console.Write("Enter PIN (4 digits): ");
                var pin = ReadPassword();

                if (user.ValidatePIN(pin))
                {
                    Console.WriteLine($"\n✓ Welcome {user.Name}!");
                    Console.ReadKey();
                    return user;
                }

                attempts++;
                Console.WriteLine($"\n✗ Incorrect PIN! Attempts remaining: {maxAttempts - attempts}");
            }

            Console.WriteLine("\n✗ Too many incorrect attempts. Returning to the menu.");
            Console.ReadKey();
            return null;
        }

        /// <summary>
        /// Gets and validates a User ID from input
        /// </summary>
        private string? GetUserId()
        {
            Console.Write("Enter User ID (e.g., U003): ");
            var userId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.WriteLine("\n✗ User ID cannot be empty!");
                Console.ReadKey();
                return null;
            }

            if (bank.FindUser(userId) != null)
            {
                Console.WriteLine("\n✗ User ID already exists!");
                Console.ReadKey();
                return null;
            }

            return userId;
        }

        /// <summary>
        /// Gets and validates a name from input
        /// </summary>
        private string? GetName()
        {
            Console.Write("Enter your name: ");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("\n✗ Name cannot be empty!");
                Console.ReadKey();
                return null;
            }

            return name;
        }

        /// <summary>
        /// Gets and confirms a PIN from user input
        /// </summary>
        private string? GetAndConfirmPIN()
        {
            Console.Write("Create a PIN (4 digits): ");
            var pin = ReadPassword();

            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                Console.WriteLine("\n✗ PIN must be exactly 4 digits!");
                Console.ReadKey();
                return null;
            }

            Console.Write("Confirm PIN: ");
            var confirmPin = ReadPassword();

            if (pin != confirmPin)
            {
                Console.WriteLine("\n✗ PINs do not match!");
                Console.ReadKey();
                return null;
            }

            return pin;
        }

        ///// <summary>
        ///// Offers the user to create an account immediately after registration
        ///// </summary>
        //private void OfferAccountCreation(User user)
        //{
        //    Console.Write("\nWould you like to create an account? (y/n): ");
        //    var createAccount = Console.ReadLine()?.ToLower();

        //    if (createAccount == "y" || createAccount == "yes")
        //    {
        //        Console.Write("Enter account number (e.g., ACC003): ");
        //        var accountNumber = Console.ReadLine();

        //        if (!string.IsNullOrWhiteSpace(accountNumber))
        //        {
        //            bank.OpenAccount(user, accountNumber);
        //            Console.WriteLine($"\n✓ Account {accountNumber} created successfully!");
        //        }
        //    }
        //}

        /// <summary>
        /// Reads password input with masked characters (asterisks)
        /// </summary>
        private string ReadPassword()
        {
            var password = "";
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.KeyChar >= '0' && key.KeyChar <= '9')
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return password;
        }
    }
}