using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.EventStores;

namespace BankApi.Events;

[EventVersion("AccountCreatedEvent", 1)]
public class AccountCreatedEvent : AggregateEvent<AccountAggregate, AccountId>
{
    public AccountCreatedEvent(string firstName, string lastName, decimal amount, string currency)
    {
        FirstName = firstName;
        LastName = lastName;
        Amount = amount;
        Currency = currency;
    }
    
    public string FirstName { get; }
    public string LastName { get; }
    public decimal Amount { get; }
    public string Currency { get; }
}