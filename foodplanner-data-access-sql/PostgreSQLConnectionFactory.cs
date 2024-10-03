using Npgsql;

namespace foodplanner_data_access_sql;

public class PostgreSQLConnectionFactory{

    private readonly string _connectionString;

    public PostgreSQLConnectionFactory(string connectionString){
        _connectionString = connectionString;
    }

    public NpgsqlConnection Create(){
        return new NpgsqlConnection(_connectionString);
    }
}