using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace BankApi.Events;

[EventVersion("WithdrawEvent", 1)]
public class WithdrawEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public WithdrawEvent(TransferId transferId, decimal amount)
    {
        TransferId = transferId;
        Amount = amount;
    }
    
    public TransferId? TransferId { get; } 
    public decimal Amount { get; }
}