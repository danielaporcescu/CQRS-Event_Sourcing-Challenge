using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace BankApi.Events;

[EventVersion("DepositEvent", 1)]
public class DepositEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public DepositEvent(TransferId transferId, decimal amount)
    {
        TransferId = transferId;
        Amount = amount;
    }

    public TransferId? TransferId { get; }
    public decimal Amount { get; }
}