using BankApi.Events;
using EventFlow.Aggregates;
using EventFlow.Sagas;

namespace BankApi.Sagas;

public class TransferSagaLocator : ISagaLocator
{
    public Task<ISagaId?> LocateSagaAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        return domainEvent.GetAggregateEvent() switch
        {
            TransferEvent e => Task.FromResult<ISagaId?>(e.TransferId),
            WithdrawEvent e => Task.FromResult<ISagaId?>(e.TransferId),
            DepositEvent e => Task.FromResult<ISagaId?>(e.TransferId),
            _ => Task.FromResult<ISagaId?>(null)
        };
    }
}