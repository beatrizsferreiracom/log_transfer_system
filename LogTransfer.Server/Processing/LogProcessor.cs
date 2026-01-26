using LogTransfer.Server.Data;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace LogTransfer.Server.Processing
{
    public class LogProcessor
    {
        private const int BATCH_SIZE = 50000;
        private const int BUFFER_SIZE = 1024 * 256;

        private readonly LogParser _parser = new();

        public void HandleClient(TcpClient client)
        {
            var queue = new BlockingCollection<LogEntry>(BATCH_SIZE * 2);
            var stopwatch = Stopwatch.StartNew();

            int totalLines = 0;

            var bulkTask = Task.Run(() =>
            {
                var batch = new List<LogEntry>(BATCH_SIZE);

                foreach (var entry in queue.GetConsumingEnumerable())
                {
                    batch.Add(entry);
                    totalLines++;

                    if (batch.Count >= BATCH_SIZE)
                    {
                        LogBulk.BulkInsert(batch);
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                    LogBulk.BulkInsert(batch);
            });

            try
            {
                using NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[BUFFER_SIZE];
                char[] charBuffer = new char[BUFFER_SIZE];
                var decoder = Encoding.UTF8.GetDecoder();
                var sb = new StringBuilder();

                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    int charsRead = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0);

                    for (int i = 0; i < charsRead; i++)
                    {
                        char c = charBuffer[i];

                        if (c == '\n')
                        {
                            var span = sb.ToString().AsSpan();

                            if (_parser.TryParse(span, out LogEntry entry))
                                queue.Add(entry);

                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                queue.CompleteAdding();
                bulkTask.Wait();

                stopwatch.Stop();

                Console.WriteLine("Client processing completed");
                Console.WriteLine($"Total time           : {stopwatch.Elapsed}");
                Console.WriteLine($"Total lines inserted : {totalLines}");
                Console.WriteLine("Client disconnected.");
            }
        }
    }
}