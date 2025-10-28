using BankApi.Aggregates;
using BankApi.Models;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace BankApi.Commands;

public class AccountCreatedCommand(
    AccountId aggregateId,
    Account account) : Command<AccountAggregate, AccountId, IExecutionResult>(aggregateId)
{
    public string FirstName { get; set; } = account.FirstName;
    public string LastName { get; set; } = account.LastName;
    public decimal Balance { get; set; } = account.Amount;
    public string Currency { get; set; } = account.Currency;
}

public class AccountCreatedCommandHandler : CommandHandler<AccountAggregate, AccountId, IExecutionResult,
    AccountCreatedCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(
        AccountAggregate aggregate,
        AccountCreatedCommand command, 
        CancellationToken cancellationToken)
    {
        var executionResult = aggregate.CreateAccount(command.FirstName, command.LastName, command.Balance, command.Currency);

        return Task.FromResult(executionResult);
    }
}