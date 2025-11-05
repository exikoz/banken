using System;

namespace bank.Core
{
    public class Loan
    {
        public string LoanId { get; }
        public User Borrower { get; }
        public decimal Principal { get; private set; }
        public decimal RemainingBalance { get; private set; }
        public decimal InterestRate { get; }
        public int TermMonths { get; }
        public DateTime StartDate { get; }

        public Loan(string loanId, User borrower, decimal principal, decimal interestRate, int termMonths)
        {
            LoanId = loanId;
            Borrower = borrower;
            Principal = principal;
            RemainingBalance = principal;
            InterestRate = interestRate;
            TermMonths = termMonths;
            StartDate = DateTime.Now;
        }

        public decimal CalculateTotalRepayment()
        {
            decimal interest = Principal * (InterestRate / 100) * (TermMonths / 12m);
            return Principal + interest;
        }

        public void MakePayment(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Payment amount must be greater than zero.");
                return;
            }

            if (amount > RemainingBalance)
                amount = RemainingBalance;

            RemainingBalance -= amount;

            Console.WriteLine($"\nPayment of {amount:C} made. Remaining balance: {RemainingBalance:C}");
        }
    }
}
