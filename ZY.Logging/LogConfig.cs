using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ZY.Logging
{
    class LogConfig
    {
        private readonly string _configFile = Path.Combine(Environment.CurrentDirectory, "LogConfig.ini");

        private LogLevels _loglevel = LogLevels.Info;
        private string _logDir = @"D:\";

        public LogLevels LogLevel
        {
            get { return this._loglevel; }
            set { this._loglevel = value; }
        }

        public string LogDir
        {
            get { return this._logDir; }
            set { this._logDir = value; }
        }

        public LogConfig()
        {
            if (!File.Exists(this._configFile))
            {
                this.WriteConfig();
            }

            this.ReadConfig();
        }

        ~LogConfig()
        {
            this.WriteConfig();
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                             string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);
        
        /// <summary>
        /// 
        /// </summary>
        private void ReadConfig()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("Options", "LogDir", @"D:\", builder, 1024, this._configFile);
            this._logDir = builder.ToString();
            //this._logDir = @"D:\测量数据\运行日志\";

            GetPrivateProfileString("Options", "LogLevel", "Info", builder, 1024, this._configFile);
            var result = Enum.TryParse(builder.ToString(), out this._loglevel);
            if (!result)
            {
                this._loglevel = LogLevels.Info;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteConfig()
        {
            WritePrivateProfileString("Options", "LogDir", this.LogDir, this._configFile);
            WritePrivateProfileString("Options", "LogLevel", this._loglevel.ToString(), this._configFile);
        }
    }
}
