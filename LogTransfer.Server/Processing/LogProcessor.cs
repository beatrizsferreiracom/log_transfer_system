using LogTransfer.Core;
using LogTransfer.Server.Data;
using System.Diagnostics;
using System.Net.Sockets;

namespace LogTransfer.Server.Processing
{
    public class LogProcessor
    {
        private const int BATCH_SIZE = SocketProtocol.ServerBatchSize;
        private readonly LogParser _parser = new();

        public void HandleClient(TcpClient client)
        {
            var logEntries = new List<LogEntry>(BATCH_SIZE);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using (client)
                {
                    NetworkStream networkStream = client.GetStream();
                    StreamReader reader = new StreamReader(networkStream);
                       
                    string? line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!_parser.TryParse(line, out LogEntry entry))
                            continue;

                        logEntries.Add(entry);

                        if (logEntries.Count >= BATCH_SIZE)
                        {
                            LogBulk.BulkInsert(logEntries);
                            logEntries.Clear();
                        }
                    }

                    if (logEntries.Count > 0)
                    {
                        LogBulk.BulkInsert(logEntries);
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
                stopwatch.Stop();

                double seconds = stopwatch.Elapsed.TotalSeconds;

                Console.WriteLine("Client processing completed");
                Console.WriteLine($"Total time           : {stopwatch.Elapsed}");
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}