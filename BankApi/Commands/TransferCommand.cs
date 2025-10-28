using EventFlow.Commands;
using EventFlow.Aggregates.ExecutionResults;
using BankApi.Aggregates;
using BankApi.Models;

namespace BankApi.Commands;

public class TransferCommand(TransferId transferId, AccountId fromAccountId, AccountId toAccountId, decimal amount)
    : Command<AccountAggregate, AccountId, IExecutionResult>(fromAccountId)
{
    public TransferId TransferId { get; } = transferId;
    public AccountId ToAccountId { get; } = toAccountId;
    public decimal Amount { get; } = amount;
}

public class TransferCommandHandler :
    CommandHandler<AccountAggregate, AccountId, IExecutionResult, TransferCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(
        AccountAggregate aggregate,
        TransferCommand command,
        CancellationToken cancellationToken)
    {
        var result = aggregate.Transfer(command.ToAccountId, command.TransferId, command.Amount);
        return Task.FromResult(result);
    }
}