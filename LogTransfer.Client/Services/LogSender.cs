using LogTransfer.Core;
using System.Net.Sockets;
namespace LogTransfer.Client.Services
{
    internal class LogSender
    {
        private readonly string _serverIp;
        private readonly int _serverPort;

        public LogSender(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        public async Task SendAsync(
            IAsyncEnumerable<string> lines,
            CancellationToken cancellationToken = default)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_serverIp, _serverPort);

            using NetworkStream networkStream = client.GetStream();
            using var writer = new StreamWriter(
                networkStream,
                SocketProtocol.Encoding,
                bufferSize: 1024 * 64,
                leaveOpen: false);
            
            await foreach (var line in lines.WithCancellation(cancellationToken))
            {
                await writer.WriteLineAsync(line);

            }

            await writer.FlushAsync();
        }
    }
}