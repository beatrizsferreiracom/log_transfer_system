using LogTransfer.Core;
using System.Security.Cryptography;

namespace LogTransfer.Server.Processing
{
    public class LogParser
    {
        static int SkipSpaces(string s, int i)
        {
            while (i < s.Length && s[i] == ' ')
                i++;
            return i;
        }

        public bool TryParse(string? line, out LogEntry logEntry)
        {
            logEntry = null!;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            int index = 0;

            int part1 = line.IndexOf(' ');
            if (part1 < 0) return false;
            index = SkipSpaces(line, part1 + 1);

            int part2 = line.IndexOf(' ', index);
            if (part2 < 0) return false;

            string logDate = line.Substring(0, part2);

            index = SkipSpaces(line, part2 + 1);

            int part3 = line.IndexOf(' ', index);
            if (part3 < 0) return false;

            if (!int.TryParse(line.Substring(index, part3 - index), out int pid))
                return false;

            index = SkipSpaces(line, part3 + 1);

            int part4 = line.IndexOf(' ', index);
            if (part4 < 0) return false;

            if (!int.TryParse(line.Substring(index, part4 - index), out int tid))
                return false;

            index = SkipSpaces(line, part4 + 1);

            int part5 = line.IndexOf(' ', index);
            if (part5 < 0) return false;

            string level = line.Substring(index, part5 - index);

            index = SkipSpaces(line, part5 + 1);

            int separator = line.IndexOf(':', index);
            
            string component;
            string content;

            if (separator < 0)
            {
                component = "Unknown";
                content = line.Substring(index).Trim();
            }
            else
            {
                component = line.Substring(index, separator - index).Trim();
                content = line.Substring(separator + 1).Trim();
            }

            if (component.Length == 0)
                component = "Unknown";

            if (content.Length == 0)
                content = string.Empty;

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