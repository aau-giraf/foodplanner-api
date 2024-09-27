using Npgsql;

namespace foodplannerApi.Services;

public class PostgreConnectionFactory{

    private readonly string _connectionString;

    public PostgreConnectionFactory(string connectionString){
        _connectionString = connectionString;
    }

    public NpgsqlConnection Create(){
        return new NpgsqlConnection(_connectionString);
    }
}