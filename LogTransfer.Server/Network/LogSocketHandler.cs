using LogTransfer.Core;
using LogTransfer.Server.Data;
using LogTransfer.Server.Processing;
using System.Net.Sockets;

namespace LogTransfer.Server.Network
{
    public class LogSocketHandler
    {
        private const int BATCH_SIZE = SocketProtocol.ServerBatchSize;
        private readonly LogParser _parser = new();

        public void HandleClient(TcpClient client)
        {
            var logEntries = new List<LogEntry>(BATCH_SIZE);

            try
            {
                using (client)
                using (NetworkStream networkStream = client.GetStream())
                using (StreamReader reader = new StreamReader(networkStream))
                {
                    string? line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!_parser.TryParse(line, out LogEntry entry))
                            continue;

                        logEntries.Add(entry);

                        if (logEntries.Count >= BATCH_SIZE)
                        {
                            LogRepository.BulkInsert(logEntries);
                            logEntries.Clear();
                        }
                    }

                    if (logEntries.Count > 0)
                    {
                        LogRepository.BulkInsert(logEntries);
                        logEntries.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client processing error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}