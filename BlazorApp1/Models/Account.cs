namespace BlazorApp1.Models;

public class Account
{
    public Account(string firstName, string lastName, string email, decimal balance)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Balance = balance;
    }

    public Account(Guid id, string firstName, string lastName, string email, string password)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }

    public Account(Guid id, string firstName, string lastName, string email, decimal balance, string currency, string password)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Balance = balance;
        Currency = currency;
        Password = password;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public decimal Balance { get; set; } = 100;
    public string Currency { get; set; } = "DKK";
    public string Password { get; set; }
}