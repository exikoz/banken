using bank.Core;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using bank.Utils;

namespace bank.Services
{
    public class AuthenticationService
    {
        private readonly Bank bank;

        public AuthenticationService(Bank bank)
        {
            this.bank = bank;
        }

        // Shows login UI
        public User? ShowLoginUI()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("LOG IN");

            var userId = ConsoleHelper.PromptWithEscape("Enter User SSN (e.g. YYYYMMDD-XXXX)");

            // ESC → back
            if (userId == "<ESC>") return null;

            if (string.IsNullOrWhiteSpace(userId))
            {
                ConsoleHelper.WriteError("SSN cannot be empty");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            var user = bank.FindUser(userId);

            if (user == null)
            {
                ConsoleHelper.WriteError("User not found");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            if (user.isBlocked)
            {
                ShowBlockedScreen(user);
                return null;
            }

            return ValidatePINWithRetries(user);
        }

        // Shows registration UI
        public bool ShowRegistrationUI()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("REGISTER NEW USER");

            var userId = GetUserId();
            if (userId == "<ESC>" || userId == null) return false;

            var name = GetName();
            if (name == "<ESC>" || name == null) return false;

            var pin = GetAndConfirmPIN();
            if (pin == "<ESC>" || pin == null) return false;

            var newUser = new User(userId, name, pin, UserRole.Customer);
            bank.RegisterUser(newUser);

            ConsoleHelper.WriteSuccess("User registered");
            ConsoleHelper.PauseWithMessage("You can now log in");
            return true;
        }


        // Validates PIN with retries
        private User? ValidatePINWithRetries(User user)
        {
            int maxAttempts = user.MaxFailedLoginAttempts;

            while (user.FailedLoginAttempts < maxAttempts)
            {
                var pin = ConsoleHelper.PromptWithEscape("Enter PIN (4 digits)");

                // ESC → go back
                if (pin == "<ESC>") return null;

                if (user.ValidatePIN(pin))
                {
                    user.FailedLoginAttempts = 0;
                    ConsoleHelper.WriteSuccess($"Welcome {user.Name}");
                    ConsoleHelper.PauseWithMessage();
                    return user;
                }

                user.FailedLoginAttempts++;
                int remaining = maxAttempts - user.FailedLoginAttempts;

                ConsoleHelper.WriteError($"Incorrect PIN. Attempts left: {remaining}");
            }

            ConsoleHelper.WriteError("Too many attempts. User is blocked");
            ConsoleHelper.PauseWithMessage();
            return null;
        }

        // Gets valid SSN
        private string? GetUserId()
        {
            var userId = ConsoleHelper.PromptWithEscape("Enter User SSN (e.g. YYYYMMDD-XXXX)");

            if (userId == "<ESC>") return "<ESC>";

            if (string.IsNullOrWhiteSpace(userId))
            {
                ConsoleHelper.WriteError("SSN cannot be empty");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            if (!Regex.IsMatch(userId, @"^\d{8}-\d{4}$"))
            {
                ConsoleHelper.WriteError("Invalid SSN format");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            if (bank.FindUser(userId) != null)
            {
                ConsoleHelper.WriteError("SSN already registered");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            return userId;
        }


        // Gets name
        private string? GetName()
        {
            var name = ConsoleHelper.PromptWithEscape("Enter your name");

            if (name == "<ESC>") return "<ESC>";

            if (string.IsNullOrWhiteSpace(name))
            {
                ConsoleHelper.WriteError("Name cannot be empty");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            return name;
        }


        // Gets PIN and confirms it
        private string? GetAndConfirmPIN()
        {
            var pin = ConsoleHelper.PromptWithEscape("Create PIN (4 digits)");

            if (pin == "<ESC>") return "<ESC>";

            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                ConsoleHelper.WriteError("PIN must be 4 digits");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            var confirmPin = ConsoleHelper.PromptWithEscape("Confirm PIN");

            if (confirmPin == "<ESC>") return "<ESC>";

            if (pin != confirmPin)
            {
                ConsoleHelper.WriteError("PINs do not match");
                ConsoleHelper.PauseWithMessage();
                return null;
            }

            return pin;
        }

        private void ShowBlockedScreen(User user)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ACCOUNT LOCKED");

            ConsoleHelper.WriteBox($"User: {user.Name}", ConsoleColor.Cyan);

            Console.WriteLine();
            ConsoleHelper.WriteWarning("Your account has been temporarily locked");
            ConsoleHelper.WriteInfo("This is a security measure after several incorrect PIN attempts.");

            Console.WriteLine();
            ConsoleHelper.WriteHighlight("To unlock your account, please contact support:", ConsoleColor.Yellow);

            Console.WriteLine();
            Console.WriteLine("• Phone: 0200 112 233");
            Console.WriteLine("• Email: support@yourbank.se");
            Console.WriteLine("• Opening hours: Mon–Fri 08:00–18:00");

            Console.WriteLine();
            ConsoleHelper.WriteWarning("For your protection, no online actions are available during lockout.");

            ConsoleHelper.PauseWithMessage();
        }

    }
}
