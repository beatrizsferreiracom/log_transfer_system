using Microsoft.Data.SqlClient;

namespace LogTransfer.Server.Data
{
    public class DatabaseContext
    {
        private readonly string _connectionString = @"Server=NTB-2KV14W2\SQLEXPRESS;Database=TRANSFER_DB;Trusted_Connection=True;TrustServerCertificate=True;";

        public SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}