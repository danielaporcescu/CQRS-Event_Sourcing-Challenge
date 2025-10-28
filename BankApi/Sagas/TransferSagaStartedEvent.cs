using BankApi.Models;
using EventFlow.Aggregates;

namespace BankApi.Sagas;

public class TransferSagaStartedEvent(
    AccountId sourceAccountId,
    AccountId targetAccountId,
    decimal amount)
    : AggregateEvent<TransferSaga, TransferId>
{
    public AccountId SourceAccountId { get; } = sourceAccountId;
    public AccountId TargetAccountId { get; } = targetAccountId;
    public decimal Amount { get; } = amount;
}