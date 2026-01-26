using LogTransfer.Server.Data;

namespace LogTransfer.Server.Processing
{
    public class LogParser
    {
        public bool ValidateLine(ReadOnlySpan<char> line)
        {
            if (line.Length < 38)
                return false;

            return char.IsDigit(line[0]);
        }

        public bool TryParse(ReadOnlySpan<char> line, out LogEntry entry)
        {
            entry = null!;

            if (!ValidateLine(line))
                return false;

            try
            {
                var logDate = line.Slice(0, 18);

                var pidSpan = line.Slice(19, 5);
                if (!int.TryParse(pidSpan, out int pid))
                    return false;

                var tidSpan = line.Slice(25, 5);
                if (!int.TryParse(tidSpan, out int tid))
                    return false;

                var level = line.Slice(31, 1);

                var content = line.Slice(33);

                int separator = content.IndexOf(':');
                if (separator < 0)
                    separator = content.IndexOf('>');

                ReadOnlySpan<char> component;
                ReadOnlySpan<char> message;

                if (separator < 0)
                {
                    component = "Unknown";
                    message = content;
                }
                else
                {
                    component = content.Slice(0, separator).Trim();
                    message = content.Slice(separator + 1).TrimStart();
                }

                entry = new LogEntry
                {
                    LogDate = logDate.ToString(),
                    Pid = pid,
                    Tid = tid,
                    Level = level.ToString(),
                    Component = component.Length == 0 ? "Unknown" : component.ToString(),
                    Content = message.ToString()
                };

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}