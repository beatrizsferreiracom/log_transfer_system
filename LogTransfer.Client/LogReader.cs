using System.Net.Sockets;

namespace LogTransfer.Client
{
    public class LogReader
    {
        private const string FILE_PATH = @"C:\dev\local\log_transfer_system\Android_v2";
        private const string FILE_PATTERN = "*.log";

        private const string SERVER_IP = "127.0.0.1";
        private const int PORT = 5000;

        private const int CHUNK_SIZE = 1024 * 256;

        public async Task ExecuteAsync()
        {
            var files = Directory
                .GetFiles(FILE_PATH, FILE_PATTERN, SearchOption.AllDirectories);

            Console.WriteLine($"Total files: {files.Length}");

            using var client1 = new TcpClient(SERVER_IP, PORT);
            using var client2 = new TcpClient(SERVER_IP, PORT);
            using var client3 = new TcpClient(SERVER_IP, PORT);

            var stream1 = client1.GetStream();
            var stream2 = client2.GetStream();
            var stream3 = client3.GetStream();

            int slice = files.Length / 3;

            var task1 = Task.Run(() => ProcessFiles(files, 0, slice, stream1));
            var task2 = Task.Run(() => ProcessFiles(files, slice, slice * 2, stream2));
            var task3 = Task.Run(() => ProcessFiles(files, slice * 2, files.Length, stream3));

            await Task.WhenAll(task1, task2, task3);
        }

        private async Task ProcessFiles(
            string[] files,
            int start,
            int end,
            NetworkStream stream)
        {
            var buffer = new byte[CHUNK_SIZE];

            for (int i = start; i < end; i++)
            {
                Console.WriteLine(
                    $"[Thread {Thread.CurrentThread.ManagedThreadId}] Reading file: {files[i]}");

                using var fs = new FileStream(
                    files[i],
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    CHUNK_SIZE,
                    useAsync: true);

                int read;
                while ((read = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, read);
                }
            }

            await stream.FlushAsync();
        }
    }
}