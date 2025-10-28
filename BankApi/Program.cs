using BankApi.Queries;
using BankApi.Services;
using EventFlow.Extensions;
using EventFlow.PostgreSql;
using EventFlow.PostgreSql.Connections;
using EventFlow.PostgreSql.EventStores;
using EventFlow.PostgreSql.Extensions;
using EventFlow.ReadStores;

var builder = WebApplication.CreateBuilder(args);

var postgres = PostgreSqlConfiguration.New
    .SetConnectionString(
        "User ID=postgres;Password=eventflow;Host=localhost;Port=5432;Database=eventflow;CommandTimeout=300;KeepAlive=300")
    .SetTransientRetryCount(3);

builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Services.AddControllers();

builder.Services.AddEventFlow(ef => ef
    .AddDefaults(typeof(Program).Assembly)
    .ConfigurePostgreSql(postgres)
    .UsePostgreSqlEventStore()
    .UsePostgreSqlReadModel<AccountReadModel>()

    .AddSagas()
);

builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<QueryService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<IPostgreSqlDatabaseMigrator>();
    await EventFlowEventStoresPostgreSql.MigrateDatabaseAsync(migrator, CancellationToken.None);

    var populator = scope.ServiceProvider.GetRequiredService<IReadModelPopulator>();
    await populator.PurgeAsync<AccountReadModel>(CancellationToken.None);
    await populator.PopulateAsync<AccountReadModel>(CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();
app.Run();

public partial class Program { }
