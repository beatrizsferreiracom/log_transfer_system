using LogTransfer.Server.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace LogTransfer.Server.Processing
{
    public class LogProcessor
    {
        private const int BATCH_SIZE = 200_000;
        private const int BUFFER_SIZE = 1024 * 256;

        private readonly LogParser _parser = new();

        public void HandleClient(TcpClient client)
        {
            var stopwatch = Stopwatch.StartNew();

            int clientTotalLines = 0;
            int batchCounter = 0;

            var batch = new List<LogEntry>(BATCH_SIZE);

            try
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(
                    stream,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: BUFFER_SIZE,
                    leaveOpen: false);

                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (_parser.TryParse(line.AsSpan(), out LogEntry entry))
                    {
                        batch.Add(entry);
                        batchCounter++;
                        clientTotalLines++;
                    }

                    if (batchCounter >= BATCH_SIZE)
                    {
                        LogBulk.BulkInsert(batch);
                        batch.Clear();
                        batchCounter = 0;
                    }
                }

                if (batchCounter > 0)
                {
                    LogBulk.BulkInsert(batch);
                    batch.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex}");
            }
            finally
            {
                stopwatch.Stop();

                System.Threading.Interlocked.Add(
                    ref LogTransfer.Server.Program.TotalInsertedLines, clientTotalLines);

                Console.WriteLine("Client processing completed");
                Console.WriteLine($"Total time           : {stopwatch.Elapsed}");
                Console.WriteLine($"Client lines inserted : {clientTotalLines}");
                Console.WriteLine($"Total lines inserted : {LogTransfer.Server.Program.TotalInsertedLines}");
                Console.WriteLine("Client disconnected");
            }
        }
    }
}