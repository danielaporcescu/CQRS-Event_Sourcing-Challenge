using BankApi.Aggregates;
using BankApi.Models;
using BankApi.Queries;
using EventFlow.Aggregates;
using EventFlow.EventStores;
using EventFlow.Queries;

namespace BankApi.Services;

public class QueryService(
    IQueryProcessor queryProcessor,
    IEventStore eventStore)
{
    public async Task<AccountReadModel> GetAccount(string id)
    {
        var accountReadModel = await queryProcessor
            .ProcessAsync(new ReadModelByIdQuery<AccountReadModel>(new AccountId(id)), CancellationToken.None)
            .ConfigureAwait(false);

        return accountReadModel;
    }

    public async Task<decimal> GetBalance(string id)
    {
        var accountReadModel = await queryProcessor
            .ProcessAsync(new ReadModelByIdQuery<AccountReadModel>(new AccountId(id)), CancellationToken.None)
            .ConfigureAwait(false);

        return accountReadModel.Balance;
    }

    public async Task<IReadOnlyCollection<IDomainEvent<AccountAggregate, AccountId>>>
        GetTransactionHistoryAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var domainEvents = await eventStore.LoadEventsAsync<AccountAggregate, AccountId>(
            accountId,
            cancellationToken);

        return domainEvents.ToList();
    }
}