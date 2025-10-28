namespace BankApi.Models;

public class Transfer(string sourceAccountId, string targetAccountId, decimal amount)
{
    public string? SourceAccountId { get; set; } = sourceAccountId;
    public string? TargetAccountId { get; set; } = targetAccountId;
    public decimal Amount { get; set; } = amount;
}