namespace BankApi.Models;

public class Account(string firstName, string lastName, decimal amount, string currency)
{
    public string? AccountId { get; set; }
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public decimal Amount { get; set; } = amount;
    public string Currency { get; set; } = currency;
}