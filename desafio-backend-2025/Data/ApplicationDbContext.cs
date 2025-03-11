using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient;

public class DatabaseConnection
{
    private readonly IConfiguration _configuration;

    public DatabaseConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection GetConnection()
    {
        return new MySqlConnection(_configuration.GetConnectionString("Connection"));
    }
}
