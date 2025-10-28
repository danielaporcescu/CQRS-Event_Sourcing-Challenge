using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.EventStores;

[EventVersion("TransferEvent", 1)]
public class TransferEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public AccountId SourceAccountId { get; }
    public AccountId TargetAccountId { get; }
    public  TransferId TransferId { get; }
    public decimal Amount { get; }

    public TransferEvent(AccountId sourceAccountId, 
        AccountId targetAccountId,  TransferId transferId, decimal amount)
    {
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        TransferId = transferId;
        Amount = amount;
    }
}