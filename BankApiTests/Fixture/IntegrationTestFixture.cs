using System.Runtime.CompilerServices;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace BankApiTests.Fixture
{
    public class IntegrationTestFixture : IDisposable
    {
        private IHost _hostRunner;
        private IServiceProvider _serviceProvider;
        private readonly PostgreSqlContainer _postgreSqlContainer;
        private readonly ConfiguredTaskAwaitable _postgresAwait;
        
        private readonly string _postgresdb = "eventflow";
        private readonly string _postgresuser = "postgres";
        private readonly string _postgrespassword = "eventflow";

         public IntegrationTestFixture()
        {
            var source = $"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}{Path.DirectorySeparatorChar}Sql{Path.DirectorySeparatorChar}init-integration-tests.sh";
            if (!File.Exists(source))
                throw new Exception($"Cannot execute test, required database init file is missing at path '{source}'");
            var destination = "/docker-entrypoint-initdb.d/002_init_user_db.sh";
            
            _postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithName("postgres-" + Guid.NewGuid().ToString("D"))
                .WithEnvironment("POSTGRES_PASSWORD", _postgrespassword)
                .WithEnvironment("POSTGRES_USER", _postgresuser)
                .WithEnvironment("POSTGRES_DB", _postgresdb)
                .WithExposedPort(5432)
                .WithPortBinding(5432)
                .WithBindMount(source, destination)
                .Build();
            
            _postgresAwait = _postgreSqlContainer.StartAsync()
                .ConfigureAwait(false);
        }
         
        public IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }

        public async Task WaitForContainers()
        {
            await _postgresAwait;
        }
        
        public void ClearEnvironment()
        {
            DatabaseUtils.ExecuteNonQuery("delete from \"ReadModel-Account\";");
            DatabaseUtils.ExecuteNonQuery("delete from eventflow;");
        }
        
        public void Dispose()
        {
            // _hostRunner.Dispose();
            CastAndDispose(_postgreSqlContainer);
            return;

            static void CastAndDispose(IAsyncDisposable resource)
            {
                if (resource is IDisposable resourceDisposable)
                    resourceDisposable.Dispose();
                else
                    _ = resource.DisposeAsync().AsTask();
            }
        }
    }
}
