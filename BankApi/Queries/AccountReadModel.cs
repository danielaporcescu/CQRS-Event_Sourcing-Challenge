using System.ComponentModel.DataAnnotations.Schema;
using BankApi.Aggregates;
using BankApi.Events;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace BankApi.Queries;

[Table("ReadModel-Account")]
public class AccountReadModel : IReadModel,
    IAmReadModelFor<AccountAggregate, AccountId, AccountCreatedEvent>,
    IAmReadModelFor<AccountAggregate, AccountId, DepositEvent>,
    IAmReadModelFor<AccountAggregate, AccountId, WithdrawEvent>
{
    public string AggregateId { get; private set; } = null!;
    public string? AccountId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; }


    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, AccountCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        AccountId = domainEvent.GetIdentity().ToString();
        AggregateId = domainEvent.AggregateIdentity.Value;
        FirstName = domainEvent.AggregateEvent.FirstName;
        LastName = domainEvent.AggregateEvent.LastName;
        Balance = domainEvent.AggregateEvent.Amount;
        Currency = domainEvent.AggregateEvent.Currency;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, DepositEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = domainEvent.GetIdentity().ToString();
        AggregateId = domainEvent.AggregateIdentity.Value;
        Balance += domainEvent.AggregateEvent.Amount;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, WithdrawEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = domainEvent.GetIdentity().ToString();
        AggregateId = domainEvent.AggregateIdentity.Value;
        Balance -= domainEvent.AggregateEvent.Amount;

        return Task.CompletedTask;
    }
    
    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, TransferEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = domainEvent.GetIdentity().ToString();
        AggregateId = domainEvent.AggregateIdentity.Value;
        Balance = domainEvent.AggregateEvent.Amount;

        return Task.CompletedTask;
    }
}