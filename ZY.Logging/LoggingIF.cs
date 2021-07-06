using System.Drawing;

namespace ZY.Logging
{
    public class LoggingIF
    {
        public static event LogEventHandler LogEventHandler = null;

        private static ZyLog _logInstance = null;
        private static StatusLog _statuslogInstance = null;
        public static bool Init()
        {
            if (null == _logInstance)
            {
                _logInstance = new ZyLog();
            }
            if (null == _statuslogInstance)
            {
                _statuslogInstance = new StatusLog();
            }
            return true;
        }

        public static bool UnInit()
        {
            return true;
        }

        /// <summary>
        /// Write log to file
        /// </summary>
        /// <param name="content">content for writing</param>
        /// <param name="level">Log level</param>
        public static void Log(string content, LogLevels level = LogLevels.Info, string tag = "")
        {
            try
            {
                if (null != LogEventHandler)
                    LogEventHandler(null, new LogEventHandlerArgs(content, level, tag));

                if (null == _logInstance) return;
                _logInstance.Log(content, level, tag);
            }
            catch { }
      
        }

        public static void StatusLog(string content, LogLevels level = LogLevels.Info, string tag = "")
        {
            try
            {
                if (null != LogEventHandler)
                    LogEventHandler(null, new LogEventHandlerArgs(content, level, tag));

                if (null == _logInstance) return;
                _statuslogInstance.Log(content, level, tag);
            }
            catch { }

        }

        public static void LogImage(Bitmap bmp)
        {
            _logInstance.LogImage(bmp);
        }

    }
}
