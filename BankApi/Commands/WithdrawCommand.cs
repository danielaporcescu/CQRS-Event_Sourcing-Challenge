using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace BankApi.Commands;

public class WithdrawCommand(
    AccountId accountId,
    TransferId transferId,
    decimal amount
    ) : Command<AccountAggregate, AccountId, IExecutionResult>(accountId)
{
    public TransferId TransferId { get; } = transferId;
    public decimal Amount { get; } = amount;
}

public class WithdrawCommandHandler : CommandHandler<AccountAggregate, AccountId, IExecutionResult,
    WithdrawCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(
        AccountAggregate aggregate,
        WithdrawCommand command,
        CancellationToken cancellationToken)
    {
        var executionResult = aggregate.WithdrawEvent(command.TransferId, command.Amount);

        return Task.FromResult(executionResult);
    }
}