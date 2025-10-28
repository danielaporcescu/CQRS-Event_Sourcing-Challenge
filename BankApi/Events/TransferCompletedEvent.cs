using BankApi.Models;
using BankApi.Sagas;
using EventFlow.Aggregates;

public class TransferCompletedEvent : AggregateEvent<TransferSaga, TransferId>
{
    public AccountId SourceAccountId { get; }
    public AccountId TargetAccountId { get; }
    public decimal Amount { get; }

    public TransferCompletedEvent(AccountId source, AccountId target, decimal amount)
    {
        SourceAccountId = source;
        TargetAccountId = target;
        Amount = amount;
    }
}