using BankApi.Aggregates;
using BankApi.Commands;
using BankApi.Events;
using BankApi.Models;
using EventFlow.Aggregates;
using EventFlow.Sagas;
using EventFlow.Sagas.AggregateSagas;

namespace BankApi.Sagas;

public class TransferSaga(TransferId id) : AggregateSaga<TransferSaga, TransferId, TransferSagaLocator>(id),
    ISagaIsStartedBy<AccountAggregate, AccountId, TransferEvent>,
    ISagaHandles<AccountAggregate, AccountId, WithdrawEvent>,
    ISagaHandles<AccountAggregate, AccountId, DepositEvent>
{
    private AccountId? SourceAccountId { get; set; }
    private AccountId? TargetAccountId { get; set; }
    private decimal Amount { get; set; }
    
    public Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, TransferEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var aggregateEvent = domainEvent.AggregateEvent;
        Emit(new TransferSagaStartedEvent(aggregateEvent.SourceAccountId, aggregateEvent.TargetAccountId,
            aggregateEvent.Amount));
        Publish(new WithdrawCommand(aggregateEvent.SourceAccountId, Id, aggregateEvent.Amount));
        return Task.CompletedTask;
    }
    
    public Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, WithdrawEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Publish(new DepositCommand(TargetAccountId ?? throw new InvalidOperationException(), Id,
            domainEvent.AggregateEvent.Amount));
        return Task.CompletedTask;
    }

    public Task HandleAsync(IDomainEvent<AccountAggregate, AccountId, DepositEvent> domainEvent,
        ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        Emit(new TransferCompletedEvent(SourceAccountId!, TargetAccountId!, Amount));
        Complete();
        return Task.CompletedTask;
    }
    
    public void Apply(TransferSagaStartedEvent transferSagaStartedEvent)
    {
        SourceAccountId = transferSagaStartedEvent.SourceAccountId;
        TargetAccountId = transferSagaStartedEvent.TargetAccountId;
        Amount = transferSagaStartedEvent.Amount;
    }

    public void Apply(TransferCompletedEvent transferCompletedEvent)
    {
        SourceAccountId = transferCompletedEvent.SourceAccountId;
        TargetAccountId = transferCompletedEvent.TargetAccountId;
        Amount = transferCompletedEvent.Amount;
    }
}