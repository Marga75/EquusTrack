using MySql.Data.MySqlClient;

namespace EquusTrackBackend
{
    public class Database
    {
        private const string connectionString = "server=localhost;database=EquusTrackDB;user=root;password=123456789;";

        public static MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
