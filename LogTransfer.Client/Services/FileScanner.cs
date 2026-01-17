using System.Runtime.CompilerServices;

namespace LogTransfer.Client.Services
{
    public class FileScanner
    {
        private readonly string _rootPath;
        private readonly string _filePattern;

        public FileScanner(string rootPath, string filePattern = "*applogcat*")
        {
            _rootPath = rootPath;
            _filePattern = filePattern;
        }   

        public async IAsyncEnumerable<string> ReadLinesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(_rootPath))
                yield break;

            IEnumerable<string> files = Directory.EnumerateFiles(
                _rootPath, 
                _filePattern, 
                SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using var stream = new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 1024 * 64,
                    useAsync: true);

                Console.WriteLine($"Reading file: {file}");

                using var reader = new StreamReader(stream);
                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    yield return line;
                }
            }
        }
    }
}