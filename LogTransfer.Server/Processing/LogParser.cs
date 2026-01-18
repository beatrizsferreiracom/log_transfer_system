using LogTransfer.Core;
using System.Security.Cryptography;

namespace LogTransfer.Server.Processing
{
    public class LogParser
    {
        public bool TryParse(string? line, out LogEntry logEntry)
        {
            logEntry = null!;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            int lenght = line.Length;
            int index = 0;

            int part1 = line.IndexOf(' ', index);
            if (part1 < 0) return false;
            index = part1;
            index++;

            int part2 = line.IndexOf(' ', index);
            if (part2 < 0) return false;
            index = part2;
            index++;

            string logDate = line.Substring(0, part2);

            int part3 = line.IndexOf(' ', index);
            if (part3 < 0) return false;

            if(!int.TryParse(line.Substring(index, part3 - index), out int pid))
                return false;

            index = part3;
            index++;

            int part4 = line.IndexOf(' ', index);
            if (part4 < 0) return false;

            if (!int.TryParse(line.Substring(index, part4 - index), out int tid))
                return false;

            index = part4;
            index++;

            int part5 = line.IndexOf(' ', index);
            if (part5 < 0) return false;

            string level = line.Substring(index, part5 - index);

            index = part5;
            index++;

            int separator = line.IndexOf(':', index);
            if (separator < 0) return false;

            string component = line.Substring(index, separator - index).Trim();
            string content = line.Substring(separator + 1).Trim();

            if (component.Length == 0 || content.Length == 0)
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