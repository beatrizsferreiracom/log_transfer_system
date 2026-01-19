using LogTransfer.Core;
using System.Net.Sockets;

namespace LogTransfer.Client
{
    public class LogTransferer
    {
        private readonly string _filePath;
        private readonly string _filePattern;
        private readonly string _serverIp;
        private readonly int _serverPort;

        public LogTransferer(
            string filePath,
            string filePattern, 
            string serverIp, 
            int serverPort)
        {
            _filePath = filePath;
            _filePattern = filePattern;
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if(!Directory.Exists(_filePath))
            {
                Console.WriteLine($"The directory {_filePath} does not exist.");
                return;
            }

            using var client = new TcpClient();
            await client.ConnectAsync(_serverIp, _serverPort);

            using NetworkStream networkStream = client.GetStream();
            using var writer = new StreamWriter(
                networkStream,
                SocketProtocol.Encoding);

            string[] files = Directory.GetFiles(
                _filePath,
                _filePattern,
                SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Operation cancelled.");
                    break;
                }

                Console.WriteLine($"Reading file: {file}");

                using var stream = new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                using var reader = new StreamReader(stream);

                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Operation cancelled.");
                        break;
                    }
                    await writer.WriteLineAsync(line);
                }

                await writer.FlushAsync();
            }
        }
    }
}