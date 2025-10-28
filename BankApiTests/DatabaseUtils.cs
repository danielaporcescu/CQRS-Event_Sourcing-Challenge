using System.Data;
using BankApi.Queries;
using Dapper;
using Npgsql;

namespace BankApiTests;

public static class DatabaseUtils
{
    public static void ExecuteNonQuery(string sqlText)
    {
        using var npgsqlConnection = GetConnection();
        npgsqlConnection.Open();
        using (var dbCommand = npgsqlConnection.CreateCommand())
        {
            dbCommand.CommandText = sqlText;
            dbCommand.ExecuteNonQuery();
        }

        npgsqlConnection.Close();
    }
        
    public static List<AccountReadModel> ExecuteReader(string sqlText)
    {
        List<AccountReadModel> result;
        using var npgsqlConnection = GetConnection();
        {
            npgsqlConnection.Open();
            using (var dbCommand = npgsqlConnection.CreateCommand())
            {
                dbCommand.CommandText = sqlText;
                using (var reader = dbCommand.ExecuteReader())
                {
                    result = SqlMapper.Parse<AccountReadModel>(reader).ToList();
                }
            }

            npgsqlConnection.Close();
        }

        return result;
    }

    private static IDbConnection GetConnection()
    {
        var connectionString =
            "User ID=postgres;Password=eventflow;Host=localhost;Port=5432;Database=eventflow;CommandTimeout=300;KeepAlive=300";
        return new NpgsqlConnection(connectionString);
    }
}