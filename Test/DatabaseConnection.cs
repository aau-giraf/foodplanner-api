using FoodplannerDataAccessSql;

namespace testing;

static class DatabaseConnection
{
    private static string host = "10.92.0.69";
    private static string port = "5432";
    private static string database = "giraf_foodplanner_db_sw5_10";
    private static string username = "postgres";
    private static string password = "yjF45K#6rmRd@2RJbbLA";

    public static PostgreSQLConnectionFactory GetConnection()
    {
        return new PostgreSQLConnectionFactory(host, port, database, username, password);
    }
}