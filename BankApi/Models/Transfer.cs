namespace BankApi.Models;

public class Transfer
{
    public string? SourceAccountId { get; set; }
    public string? TargetAccountId { get; set; }
    public decimal Amount { get; set; }
}