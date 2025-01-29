using BlazorApp1.Models;
using Npgsql;

namespace BlazorApp1.Database;

public class AccountRepository
{
    private const string ConnectionString = "Host=localhost;Username=postgres;Password=mysecretpassword;Database=postgres";

    public async Task CreateTable()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        
        const string createTableSql = @"CREATE TABLE IF NOT EXISTS Accounts(
                        id uuid primary key,
                        first_name text,
                        last_name text,
                        email text,
                        balance numeric,
                        currency text,
                        password text);";
        
        await using var cmd = new NpgsqlCommand(createTableSql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
    
    public async Task<List<Account>> GetAccounts()
    {
        try
        {
            const string sql = @"SELECT * FROM accounts";
            await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
            await using var cmd = dataSource.CreateCommand(sql);
            await using var reader = await cmd.ExecuteReaderAsync();

            var accounts = new List<Account>();
            while (await reader.ReadAsync())
            {
                accounts.Add(new Account(id: reader.GetGuid(0), firstName: reader.GetString(1),
                    lastName: reader.GetString(2), email: reader.GetString(3), balance: reader.GetDecimal(4),
                    currency: reader.GetString(5), password: reader.GetString(6)));
            }
            
            return accounts;
        }
        catch (NpgsqlException ex)
        {
            throw new Exception($"Error getting accounts. Reason: {ex.Message}");
        }
    }
    
    public async Task<List<Account>> GetAccount()
    {
        try
        {
            const string sql = @"SELECT * 
                                FROM accounts 
                                WHERE first_name = @FirstName
                                AND last_name = @LastName";
            
            await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
            await using var cmd = dataSource.CreateCommand(sql);
            await using var reader = await cmd.ExecuteReaderAsync();

            var accounts = new List<Account>();
            while (await reader.ReadAsync())
            {
                accounts.Add(
                    new Account(id: reader.GetGuid(0), 
                        firstName: reader.GetString(1), 
                        lastName: reader.GetString(2), 
                        email: reader.GetString(3), 
                        balance: reader.GetDecimal(4), 
                        currency: reader.GetString(5), 
                        password: reader.GetString(6)));
            }
            
            return accounts;
        }
        catch (NpgsqlException ex)
        {
            throw new Exception($"Error getting accounts. Reason: {ex.Message}");
        }
    }
    
    public async Task AddAccount(Account account)
    {
        try
        {
            const string sql = @"INSERT INTO accounts (id,first_name, last_name, email, balance, password, currency)
                                 VALUES (@Id, @FirstName, @LastName, @Email,  @Balance, @Password, @Currency);";
            
            await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
            await using var cmd = dataSource.CreateCommand(sql);
            
            cmd.Parameters.AddWithValue("@Id", account.Id);
            cmd.Parameters.AddWithValue("@FirstName", account.FirstName);
            cmd.Parameters.AddWithValue("@LastName", account.LastName);
            cmd.Parameters.AddWithValue("@Email", account.Email);
            cmd.Parameters.AddWithValue("@Balance", account.Balance);
            cmd.Parameters.AddWithValue("@Currency", account.Currency);
            cmd.Parameters.AddWithValue("@Password", account.Password);
            
            await cmd.ExecuteNonQueryAsync();
        }
        catch (NpgsqlException ex)
        {
            throw new Exception($"Error getting accounts. Reason: {ex.Message}");
        }
    }
    
    public async Task<decimal> GetBalance(Account account)
    {
        try
        {
            const string sql = @"SELECT balance 
                                FROM accounts 
                                WHERE id = @Id
                                AND first_name = @FirstName
                                AND last_name = @LastName
                                LIMIT 1";
            
            await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
            await using var cmd = dataSource.CreateCommand(sql);
            
            cmd.Parameters.AddWithValue("@Id", account.Id);
            cmd.Parameters.AddWithValue("@FirstName", account.FirstName);
            cmd.Parameters.AddWithValue("@LastName", account.LastName);
            
            await using var reader = await cmd.ExecuteReaderAsync();

            decimal balance = 0;
            while (await reader.ReadAsync())
            {
                balance = reader.GetDecimal(0);
            }

            return balance;
        }
        catch (NpgsqlException ex)
        {
            throw new Exception($"Error getting accounts. Reason: {ex.Message}");
        }
    }
    
    public async Task UpdateBalance(Account account, decimal balance)
    {
        try
        {
            const string sql = """
                               UPDATE accounts
                               SET balance = @Balance
                               WHERE id = @Id 
                               AND first_name = @FirstName
                               AND last_name = @LastName
                               AND email = @Email
                               """;
            
            await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
            await using var cmd = dataSource.CreateCommand(sql);
            
            cmd.Parameters.AddWithValue("@Id", account.Id);
            cmd.Parameters.AddWithValue("@Balance", balance);
            cmd.Parameters.AddWithValue("@FirstName", account.FirstName);
            cmd.Parameters.AddWithValue("@LastName", account.LastName);
            cmd.Parameters.AddWithValue("@Email", account.Email);
            
            await cmd.ExecuteNonQueryAsync();
        }
        catch (NpgsqlException ex)
        {
            throw new Exception($"Error getting accounts. Reason: {ex.Message}");
        }
    }
}