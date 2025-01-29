namespace BlazorApp1.Models;

public class Account
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public decimal Balance { get; set; } = 100;
    public string Currency { get; set; } = "DKK";
    public string Password { get; set; }
}