using LogTransfer.Client.Services;
using LogTransfer.Core;

class Program
{
    static async Task Main(string[] args)
    {
        if(args.Length < 2)
        {
            Console.WriteLine("Usage: LogTransfer.Client <logPath> <serverIp> [serverPort]");
            return;
        }

        string logPath = args[0];
        string serverIp = args[1];
        int serverPort = args.Length >= 3 
            ? int.Parse(args[2])
            : SocketProtocol.DefaultPort;

        Console.WriteLine("Starting log transfer...");
        Console.WriteLine($"Path: {logPath}");
        Console.WriteLine($"Server: {serverIp}:{serverPort}");

        var scanner = new FileScanner(logPath);
        var sender = new LogSender(serverIp, serverPort);

        await sender.SendAsync(scanner.ReadLinesAsync());

        Console.WriteLine("Log transfer completed.");
    }
}