using LogTransfer.Core;

namespace LogTransfer.Server.Processing
{
    public class LogParser
    {
        public bool TryParse(string? line, out LogEntry logEntry)
        {
            logEntry = null!;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 6)
                return false;

            string logDate = $"{parts[0]} {parts[1]}";

            if (!int.TryParse(parts[2], out int pid))
                return false;

            if (!int.TryParse(parts[3], out int tid))
                return false;

            string level = parts[4];

            string rest = string.Join(' ', parts.Skip(5));

            int separator = rest.IndexOf(':');
            if (separator == -1)
                return false;

            string component = rest.Substring(0, separator).Trim();
            string content = rest.Substring(separator + 1).Trim();

            if (string.IsNullOrEmpty(component) || string.IsNullOrEmpty(content))
                return false;

            logEntry = new LogEntry
            {
                LogDate = logDate,
                Pid = pid,
                Tid = tid,
                Level = level,
                Component = component,
                Content = content
            };    
            
            return true;
        }
    }
}