using LogTransfer.Client;

class Program
{
    static async Task Main()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var reader = new LogReader();
        await reader.ExecuteAsync();

        sw.Stop();
        Console.WriteLine($"Transfer completed in {sw.Elapsed}");
    }
}
