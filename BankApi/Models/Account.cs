using System.Text.Json.Serialization;

namespace BankApi.Models;

[method: JsonConstructor]
public class Account(
    string? id,
    string firstName,
    string lastName,
    decimal amount,
    string currency)
{
    public string Id { get; set; } = id ?? AccountId.New.ToString();
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public decimal Amount { get; set; } = amount;
    public string Currency { get; set; } = currency;
}