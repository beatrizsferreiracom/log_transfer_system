namespace LogTransfer.Server.Data
{
    public class LogEntry
    {
        public string LogDate { get; set; }
        public int Pid { get; set; }
        public int Tid { get; set; }
        public string Level { get; set; }
        public string Component { get; set; }
        public string Content { get; set; }
    }
}