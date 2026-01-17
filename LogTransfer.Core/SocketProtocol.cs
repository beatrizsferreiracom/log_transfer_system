using System.Text;

namespace LogTransfer.Core
{
    public static class SocketProtocol
    {
        // Protocol version identifier
        public const string ProtocolVersion = "1.0";

        // Encoding used for socket communication
        public static readonly Encoding Encoding = Encoding.UTF8;

        // Delimiter for log entries
        public const char LineDelimiter = '\n';

        // Default port for the log transfer server
        public const int DefaultPort = 5000;

        // Maximum size of log batch for processing
        public const int ServerBatchSize = 50000;

        // Indicates if the server sends a response after processing logs
        public const bool ServerSendsResponded = false;
    }
}