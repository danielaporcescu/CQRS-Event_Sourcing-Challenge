using Npgsql;

namespace BlazorApp1.Repository;

public class ConnectDb
{
    public static async Task ConnectToDb()
    {
        var connectionString = "Host=localhost;Username=postgres;Password=mysecretpassword;Database=postgres";
        await using var dataSource = NpgsqlDataSource.Create(connectionString);

        var connection = new NpgsqlConnection(connectionString);

        var createTableSql = @"CREATE TABLE IF NOT EXISTS accounts(
                        id uuid primary key,
                        first_name text,
                        last_name text,
                        email text,
                        balance numeric,
                        currency text,
                        password text);";
        
        await using var command = dataSource.CreateCommand(createTableSql);
        await command.ExecuteNonQueryAsync();
    }
}