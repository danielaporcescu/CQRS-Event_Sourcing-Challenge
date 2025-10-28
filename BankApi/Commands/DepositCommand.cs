using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace BankApi.Commands;

public class DepositCommand(
    AccountId accountId,
    TransferId transferId,
    decimal amount) : Command<AccountAggregate, AccountId, IExecutionResult>(accountId)
{
    public TransferId TransferId { get; } = transferId;
    public decimal Amount  { get; set; } = amount;
}

public class DepositCommandHandler : CommandHandler<AccountAggregate, AccountId, IExecutionResult,
    DepositCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(
        AccountAggregate aggregate,
        DepositCommand command, 
        CancellationToken cancellationToken)
    {
        var executionResult = aggregate.Deposit(command.TransferId, command.Amount);

        return Task.FromResult(executionResult);
    }
}