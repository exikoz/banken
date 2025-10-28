public void OfferLoanUI(User currentUser)
{
    Console.Clear();
    Console.WriteLine("=== LOAN SIMULATION ===\n");

    if (currentUser == null)
    {
        Console.WriteLine("You must be logged in to use this feature.");
        Console.ReadKey();
        return;
    }

    // Räkna ut total balans
    decimal totalBalance = currentUser.Accounts.Sum(a => a.Balance);

    // Stoppa om användaren inte har pengar alls
    if (totalBalance <= 0)
    {
        Console.WriteLine("\nYou must have money in your account to apply for a loan.");
        Console.WriteLine("Deposit funds before trying again.");
        Console.ReadKey();
        return;
    }

    // Räkna ut maxlån = 5x total balans
    decimal maxLoanAllowed = totalBalance * 5;

    Console.WriteLine($"Your total balance: {totalBalance:C}");
    Console.WriteLine($"Maximum loan allowed: {maxLoanAllowed:C}\n");

    Console.Write("How much do you want to borrow (kr): ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal loanAmount) || loanAmount <= 0)
    {
        Console.WriteLine("\nInvalid loan amount.");
        Console.ReadKey();
        return;
    }

    // Spärr för lån över 5x saldot
    if (loanAmount > maxLoanAllowed)
    {
        Console.WriteLine("\nLoan denied.");
        Console.WriteLine($"You can only borrow up to {maxLoanAllowed:C} (5x your total balance).");
        Console.ReadKey();
        return;
    }

    Console.Write("Enter annual interest rate in % (example: 6): ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal annualRate) || annualRate <= 0)
    {
        Console.WriteLine("\nInvalid interest rate.");
        Console.ReadKey();
        return;
    }

    Console.Write("Enter loan duration in years: ");
    if (!int.TryParse(Console.ReadLine(), out int years) || years <= 0)
    {
        Console.WriteLine("\nInvalid duration.");
        Console.ReadKey();
        return;
    }

    decimal totalToRepay = CalculateTotalLoanCost(loanAmount, annualRate, years);
    decimal totalInterest = totalToRepay - loanAmount;

    Console.WriteLine("\n-------------------------------------------");
    Console.WriteLine($"Requested loan:      {loanAmount:C}");
    Console.WriteLine($"Interest rate:       {annualRate}%");
    Console.WriteLine($"Duration:            {years} year(s)");
    Console.WriteLine($"Total interest cost: {totalInterest:C}");
    Console.WriteLine($"Total to repay:      {totalToRepay:C}");
    Console.WriteLine("-------------------------------------------");

    Console.WriteLine("\nPress any key to return...");
    Console.ReadKey();
}
