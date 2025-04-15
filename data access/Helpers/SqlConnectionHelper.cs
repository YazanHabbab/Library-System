using Microsoft.Data.SqlClient;

namespace data_access.Helpers
{
    // Sql Connection creation helper class
    public class SqlConnectionHelper
    {
        private readonly string _connectionString;

        public SqlConnectionHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}