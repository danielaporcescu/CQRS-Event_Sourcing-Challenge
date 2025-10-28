using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Extensions;
using EventFlow.Queries;
using EventFlow.ReadStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankApiTests;

public class Tests
{
    [Test]
    public async Task ExampleTest()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEventFlow(ef => ef
            .AddDefaults(typeof(ExampleAggregate).Assembly)
            .UseInMemoryReadStoreFor<ExampleReadModel>());
        
        services.AddLogging();

        await using var serviceProvider = services.BuildServiceProvider();
        
        var commandBus = serviceProvider.GetRequiredService<ICommandBus>();
        var queryProcessor = serviceProvider.GetRequiredService<IQueryProcessor>();
        var exampleId = ExampleId.New;

        // Act
        await commandBus.PublishAsync(new ExampleCommand(exampleId, 42), CancellationToken.None)
            .ConfigureAwait(false);
        var exampleReadModel = await queryProcessor.ProcessAsync(new ReadModelByIdQuery<ExampleReadModel>(exampleId), CancellationToken.None)
            .ConfigureAwait(false);

        // Assert
        Assert.That(exampleReadModel.MagicNumber, Is.EqualTo(42));
    }
}

public class ExampleId : Identity<ExampleId>
{
    public ExampleId(string value) : base(value) { }
}

public class ExampleAggregate : AggregateRoot<ExampleAggregate, ExampleId>,
    IEmit<ExampleEvent>
{
    private int? _magicNumber;

    public ExampleAggregate(ExampleId id) : base(id) { }

    public IExecutionResult SetMagicNumber(int magicNumber)
    {
        if (_magicNumber.HasValue)
        {
            return ExecutionResult.Failed("Magic number already set");
        }

        Emit(new ExampleEvent(magicNumber));
        return ExecutionResult.Success();
    }

    public void Apply(ExampleEvent aggregateEvent)
    {
        _magicNumber = aggregateEvent.MagicNumber;
    }
}

[EventVersion("example", 1)]
public class ExampleEvent : AggregateEvent<ExampleAggregate, ExampleId>
{
    public ExampleEvent(int magicNumber)
    {
        MagicNumber = magicNumber;
    }

    public int MagicNumber { get; }
}

public class ExampleCommand : Command<ExampleAggregate, ExampleId>
{
    public ExampleCommand(ExampleId aggregateId, int magicNumber)
        : base(aggregateId)
    {
        MagicNumber = magicNumber;
    }

    public int MagicNumber { get; }
}

public class ExampleCommandHandler : CommandHandler<ExampleAggregate, ExampleId, ExampleCommand>
{
    public override Task<IExecutionResult> ExecuteCommandAsync(
        ExampleAggregate aggregate,
        ExampleCommand command,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(aggregate.SetMagicNumber(command.MagicNumber));
    }

    public override Task ExecuteAsync(ExampleAggregate aggregate, ExampleCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class ExampleReadModel : IReadModel,
    IAmReadModelFor<ExampleAggregate, ExampleId, ExampleEvent>
{
    public int MagicNumber { get; private set; }

    public Task ApplyAsync(
        IReadModelContext context,
        IDomainEvent<ExampleAggregate, ExampleId, ExampleEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        MagicNumber = domainEvent.AggregateEvent.MagicNumber;
        return Task.CompletedTask;
    }
}