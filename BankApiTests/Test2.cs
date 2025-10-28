using BankApi.Aggregates;
using BankApi.Commands;
using BankApi.Models;
using BankApi.Queries;
using BankApi.Services;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BankApiTests;

public class Test2
{
    [Test]
    public async Task Test()
    {
        var services = new ServiceCollection();
        services.AddEventFlow(ef => ef
            .AddDefaults(typeof(AccountAggregate).Assembly)
            .UseInMemoryReadStoreFor<AccountReadModel>());
        
        services.AddLogging();

        await using var serviceProvider = services.BuildServiceProvider();
        
        var commandBus = serviceProvider.GetRequiredService<ICommandBus>();
        var queryProcessor = serviceProvider.GetRequiredService<IQueryProcessor>();
        var accountId = AccountId.New;

        // Act
        // await commandBus.PublishAsync(new AccountCreatedCommand(accountId, "john","smith", 100, "opaa"), CancellationToken.None)
        //     .ConfigureAwait(false);
        //
        // await commandBus.PublishAsync(new DepositCommand(accountId, "john",170), CancellationToken.None)
        //     .ConfigureAwait(false);
        var accountReadModel = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<AccountReadModel>(accountId), CancellationToken.None)
            .ConfigureAwait(false);

        // Assert
        Assert.That(accountReadModel.FirstName, Is.EqualTo("daniela"));
    }

    // [Test]
    // public void Test3()
    // {
    //     var commandService = new CommandService();
    // }
}

