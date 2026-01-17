using LogTransfer.Server.Network;
using System.Net;
using System.Net.Sockets;

const int PORT = 5000;

TcpListener listener = new TcpListener(IPAddress.Any, PORT);
listener.Start();

Console.WriteLine($"Server listening on port {PORT}...");

while (true)
{
    TcpClient client = listener.AcceptTcpClient();
    Console.WriteLine("Client connected.");

    _ = Task.Run(() =>
    {
        var protocol = new LogSocketHandler();
        protocol.HandleClient(client);
    });
}