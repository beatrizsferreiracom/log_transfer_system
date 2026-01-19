using LogTransfer.Client;
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

        var transfer = new LogTransferer(
            logPath,
            "*.log",
            serverIp,
            serverPort);

        await transfer.ExecuteAsync();

        Console.WriteLine("Log transfer completed.");
    }
}