namespace ZY.Logging
{
    public class LogEventHandlerArgs
    {
        public string Content = string.Empty;
        public LogLevels Loglevel = LogLevels.Info;
        public string Tag = string.Empty;

        public LogEventHandlerArgs(string content, LogLevels logLevel, string tag)
        {
            this.Content = content;
            this.Loglevel = logLevel;
            this.Tag = tag;
        }
    }

    public delegate void LogEventHandler(
        object sender, LogEventHandlerArgs e);
}
