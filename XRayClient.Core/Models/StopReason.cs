using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using System.Runtime.InteropServices;
using System.IO;
using ZY.Logging;

namespace XRayClient.Core
{
    /// <summary>
    /// 停机原因上传
    /// </summary>
    public class StopReason : ObservableObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                         string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);

        static private readonly string _configFile = Path.Combine(Environment.CurrentDirectory, "CheckStatistics.ini");


        static private DateTime _stopTime = DateTime.Now;   //停机时间，即运行结束时间
        static private DateTime _startTime = DateTime.Now;  //启动时间，即停机结束时间
        static private DateTime _tempTime_RunningPending = DateTime.Now;   //等待物料时间节点缓存
        static bool _stopReasonPagShow = false;       
        static bool _isFinishStartupValidate = false;  //已完成开机校验标志
        static bool _isFirstOpenXray = false;          //是否首次开机
        static string _reasonMaintainCode = string.Empty;      //停机原因代码
        static string _reasonString = string.Empty;                  //停机原因
        static private DateTime _time = DateTime.Now;

        static public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }

        static public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
            }
        }

        static public DateTime TempTime_RunningPending
        {
            get
            {
                return _tempTime_RunningPending;
            }
            set
            {
                _tempTime_RunningPending = value;
            }
        }

        static public DateTime StopTime
        {
            get
            {
                if (!File.Exists(_configFile))
                {
                    WritePrivateProfileString("StopReason", "StartStopTime", _stopTime.ToString("yyyy-MM-dd HH:mm:ss"), _configFile);
                }

                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("StopReason", "StartStopTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), builder, 1024, _configFile);
                _stopTime = DateTime.Parse(builder.ToString());

                return _stopTime;
            }
            set
            {
                _stopTime = value;

                WritePrivateProfileString("StopReason", "StartStopTime", _stopTime.ToString("yyyy-MM-dd HH:mm:ss"), _configFile);
            }
         }

        static public bool StopReasonPagShow
        {
            get
            {
                //if (!File.Exists(_configFile))
                //{
                //    WritePrivateProfileString("StopReason", "StopReasonPagShow", _stopReasonPagShow.ToString(), _configFile);
                //}

                //StringBuilder builder = new StringBuilder(1024);
                //GetPrivateProfileString("StopReason", "StopReasonPagShow", "false", builder, 1024, _configFile);
                //_stopReasonPagShow = bool.Parse(builder.ToString());

                return _stopReasonPagShow;
            }
            set
            {
                _stopReasonPagShow = value;
            //    WritePrivateProfileString("StopReason", "StopReasonPagShow", _stopReasonPagShow.ToString(), _configFile);
            }
        }

        static public bool IsFinishStartupValidate
        {
            get
            {
                return _isFinishStartupValidate;
            }
            set
            {
                _isFinishStartupValidate = value;
            }
        }

        static public bool IsFirstOpenXray
        {
            get
            {
                return _isFirstOpenXray;
            }
            set
            {
                _isFirstOpenXray = value;
            }
        }

        static public string ReasonMaintainCode
        {
            get
            {
                return _reasonMaintainCode;
            }
            set
            {
                _reasonMaintainCode = value;
            }
        }

        static public string ReasonString
        {
            get
            {
                return _reasonString;
            }
            set
            {
                _reasonString = value;
            }
        }

        /// <summary>
        /// 接口调用应用
        /// </summary>
        /// <param name="startTime">开始计时时间</param>
        /// <param name="endtime">结束计时时间</param>
        static public void AddStopRecordAppFunction(DateTime startTime, DateTime endtime)
        {
            try
            {
                if (StopReason.ReasonMaintainCode == string.Empty || StopReason.ReasonString == string.Empty)
                {
                    throw new Exception("Error with :ReasonMaintainCode or ReasonString is Empty");
                }

                

                StopReason.ReasonMaintainCode = string.Empty;
                StopReason.ReasonString = string.Empty;
            }
            catch (System.Exception ex)
            {
                StopReason.ReasonMaintainCode = string.Empty;
                StopReason.ReasonString = string.Empty;
                LoggingIF.Log(string.Format("Failed to Add StopRecord : {0}", ex.Message.ToString()), LogLevels.Error, "Bot");
            }
        }
    }
}
