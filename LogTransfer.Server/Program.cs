using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LogTransfer.Server
{
    class Program
    {
        public static long TotalInsertedLines = 0;

        static void Main()
        {
            const int PORT = 5000;

            var listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();

            Console.WriteLine($"Server listening on port {PORT}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected");

                Task.Run(() =>
                {
                    var processor = new Processing.LogProcessor();
                    processor.HandleClient(client);
                });
            }
        }
    }
}