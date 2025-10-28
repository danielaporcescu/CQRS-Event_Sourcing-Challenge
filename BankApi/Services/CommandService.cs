using BankApi.Commands;
using BankApi.Models;
using BankApi.Queries;
using EventFlow;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Queries;

namespace BankApi.Services;

public class CommandService(ICommandBus commandBus, QueryService queryService)
{
    public async Task CreateAccount(Account account)
    {
        await commandBus.PublishAsync(new AccountCreatedCommand(AccountId.New, account), CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task Deposit(Account account)
    {
        await commandBus.PublishAsync(
                new DepositCommand(
                    new AccountId(account.AccountId),
                    TransferId.New,
                    account.Amount),
                CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task Withdraw(Account account)
    {
        await commandBus.PublishAsync(
                new WithdrawCommand(
                    new AccountId(account.AccountId),
                    TransferId.New,
                    account.Amount),
                CancellationToken.None)
            .ConfigureAwait(false);
    }

    public async Task Transfer(string from, string to, decimal amount,
        CancellationToken cancellationToken)
    {
        await commandBus
            .PublishAsync(
                new TransferCommand(TransferId.New, new AccountId(from), new AccountId(to), amount),
                cancellationToken).ConfigureAwait(false);
    }
}