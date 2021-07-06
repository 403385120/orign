using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ZY.BarCodeReader;

namespace ZY.Logging
{
    class StatusLog
    {
        private BinaryWriter _writer = null;
        private Stream _stream = null;

        //文件容量上限
        private const long _maxcapt = 2000000;

        private string _currentlogfile = string.Empty;
        private LogConfig _logConfig = new LogConfig();
        private string _logFilePath = ".";

        public StatusLog()
        {
            _logFilePath = this._logConfig.LogDir;

            _logFilePath = Path.Combine(this._logConfig.LogDir, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), "MachineStatus");

            try
            {
                if (!Directory.Exists(_logFilePath))
                {
                    Directory.CreateDirectory(_logFilePath);
                }
            }
            catch (System.Exception)
            {
                _logFilePath = "C:\\";
            }

            this.CreateLogFile(_logFilePath);
        }

        private int _flushThres = 100;
        private int _writeCnt = 0;
        private object _syncObject = new object();
        public void Log(string content, LogLevels level = LogLevels.Info, string tag = "")
        {
            //if ((int)level < (int)_logConfig.LogLevel)
            //{
            //    return;
            //}

            if (null == this._stream)
            {
                return;
            }

#if false
            // 此函数是多线程调用，在此创建会有问题：
            // 1. 创建时不能关掉就的stream，如果关掉可能导致一个正在写的线程出错
            // 2. 每次检测严重消耗资源
            //判断当前的文件大小是否超标
            if(Getfilecap(_currentlogfile) > _maxcapt)
            {
                //建立新的文件在新的文件中读写
                CreateLogFile(_logFilePath);
            }
#endif

            string lineContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss :\t") + "\t" + tag + "\t" + content;
            this._writer.Write(Encoding.UTF8.GetBytes(lineContent + "\r\n"));

            // 及时保存，否则会导致日志混乱
            this._writer.Flush();
            /*  this._writeCnt++;
              if(this._writeCnt >= this._flushThres)
              {
                  this._writeCnt = 0;
                  lock (this._syncObject)
                  {
                      this._writer.Flush();
                  }
              }*/

            //this._writer.Close();
        }




        public void LogImage(Bitmap bmp)
        {
            if (null == bmp) return;

            try
            {
                bmp.Save(Path.Combine(this._logFilePath, DateTime.Now.ToString("yyyyMMddHHmmssffff" + ".bmp")));
            }
            catch { }
        }

        ~StatusLog()
        {
            try
            {
                if (this._writer != null)
                {
                    this._writer.Close();
                }
                if (this._stream != null)
                {
                    this._stream.Close();
                }
            }
            catch
            {
            }
        }

        public long Getfilecap(string filePath)
        {
            long temp = 0;

            //判断当前路径所指向的是否为文件
            if (File.Exists(filePath) == false)
            {
                string[] str1 = Directory.GetFileSystemEntries(filePath);
                foreach (string s1 in str1)
                {
                    temp += Getfilecap(s1);
                }
            }
            else
            {

                //定义一个FileInfo对象,使之与filePath所指向的文件向关联,

                //以获取其大小
                FileInfo fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            return temp;
        }

        private int GetDixNum(string logfilename)
        {
            //获取文件名后缀的数字
            string[] arrTemp = logfilename.Split('\\');

            int nlength = arrTemp.Length;

            //判断是否有下划线
            if (arrTemp[nlength - 1].Contains("_"))
            {
            }
            else
            {
                return 0;
            }

            string[] arrtemp1 = arrTemp[nlength - 1].Split('_');
            nlength = arrtemp1.Length;

            string temp = arrtemp1[nlength - 1].Substring(0, arrtemp1[nlength - 1].Length - 4);

            return int.Parse(temp);
            // return 0;
        }

        private string Searchfilepath(string LogFiledir)
        {
            //获取指定路径下的文件名后缀最大的文件返回
            DirectoryInfo folder = new DirectoryInfo(LogFiledir);
            int nmax = -1;

            string resultfilename = string.Empty;

            foreach (FileInfo file in folder.GetFiles("*.log"))
            {
                int nlenth = GetDixNum(file.FullName);
                if (nmax < nlenth)
                {
                    resultfilename = file.FullName;
                    nmax = nlenth;
                }
            }

            if (nmax == -1)
            {

            }
            else
            {
                return resultfilename;
            }

            return string.Empty;
        }

        private string GetDirfromstring(string LogFilepath)
        {
            //获取路径
            string[] arrTemp = LogFilepath.Split('\\');

            int nlength = arrTemp.Length;

            int stringlength = arrTemp[nlength - 1].Length;

            string dir = LogFilepath.Substring(0, LogFilepath.Length - stringlength);


            return dir;
        }

        private string RenameFilepath(string LogFilepath)
        {
            //截取前9个字节后面的转化为整数
            int length = GetDixNum(LogFilepath);

            length++;

            string resultfilename = string.Empty;
            resultfilename = Path.Combine(GetDirfromstring(LogFilepath), DateTime.Now.ToString("yyyyMMdd"));
            resultfilename = resultfilename + "_";
            resultfilename = resultfilename + length.ToString();
            resultfilename = resultfilename + ".log";

            return resultfilename;
        }


        private void CreateLogFile(string logFilePath)
        {
            string logFileName = string.Empty;

            if (Searchfilepath(logFilePath) == string.Empty)
            {
                logFileName = Path.Combine(logFilePath, DateTime.Now.ToString("yyyyMMdd'.log'"));
            }
            else
            {
                //此时已存在文件,获取后缀最大的文件
                logFileName = Searchfilepath(logFilePath);

                if (Getfilecap(logFileName) > _maxcapt)
                {
                    //建立新的文件在新的文件中读写
                    logFileName = RenameFilepath(logFileName);
                }
            }

            try
            {
                this._stream = new FileStream(logFileName, FileMode.Append);
                this._writer = new BinaryWriter(this._stream);
            }
            catch (System.Exception)
            {
                try
                {
                    logFileName = Path.Combine(logFilePath, DateTime.Now.ToString("yyyyMMdd'_1_11.log'"));
                    this._stream = new FileStream(logFileName, FileMode.Append);
                    this._writer = new BinaryWriter(this._stream);
                }
                catch (System.Exception)
                {
                    this._stream = null;
                    this._writer = null;
                }

                return;
            }

            _currentlogfile = logFileName;
        }

    }
    }
