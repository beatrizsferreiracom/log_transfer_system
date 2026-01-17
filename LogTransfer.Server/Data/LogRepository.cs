using LogTransfer.Core;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LogTransfer.Server.Data
{
    public static class LogRepository
    {
        private static readonly DatabaseContext _dbContext = new();

        public static void BulkInsert(IEnumerable<LogEntry> logEntries)
        {
            using SqlConnection connection = _dbContext.CreateConnection();

            using SqlBulkCopy bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = "AndroidLogs"
            };

            DataTable table = new DataTable();
            table.Columns.Add("LogDate", typeof(string));
            table.Columns.Add("Pid", typeof(int));
            table.Columns.Add("Tid", typeof(int));
            table.Columns.Add("Level", typeof(string));
            table.Columns.Add("Component", typeof(string));
            table.Columns.Add("Content", typeof(string));

            foreach (var entry in logEntries)
            {
                table.Rows.Add(
                    entry.LogDate, 
                    entry.Pid, 
                    entry.Tid, 
                    entry.Level, 
                    entry.Component, 
                    entry.Content);
            }

            bulkCopy.WriteToServer(table);
        }
    }
}