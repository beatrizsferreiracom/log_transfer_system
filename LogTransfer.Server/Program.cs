using LogTransfer.Server;
using LogTransfer.Server.Processing;
using System.Net;
using System.Net.Sockets;

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
        var processor = new LogProcessor();
        processor.HandleClient(client);
    });
}