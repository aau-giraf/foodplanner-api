using Npgsql;

namespace FoodplannerDataAccessSql;

// Factory class for creating PostgreSQL database connections.
public class PostgreSQLConnectionFactory{
    private readonly string _host;
    private readonly string _port;
    private readonly string _database;
    private readonly string _username;
    private readonly string _password;
  
    public PostgreSQLConnectionFactory(string host, string port, string database, string username, string password)
    {
        _host = host;
        _port = port;
        _database = database;
        _username = username;
        _password = password;
    }

    // Creates and returns a new NpgsqlConnection.
    public NpgsqlConnection Create(){
        var connectionString = $"Server={_host};Port={_port};Database={_database};User Id={_username};Password={_password}";
        return new NpgsqlConnection(connectionString);
    }
}