using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using Esquel.BaseManager;
using HslCommunication.ModBus;
using SuperCom.DataCommunication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ZY.Systems
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public partial class Common
    {
        public bool isLog = false;
        /// <summary>
        /// 日志写入到RichText中,并显示出来
        /// </summary>
        /// <param name="rich">控件名称</param>
        /// <param name="text">日志内容</param>
        /// <param name="controlLog">是否允许写入</param>
        /// <param name="rowImportNum">总内容长度自动导出</param>
        public static void SetRichText(RichTextBox rich, string text, bool controlLog = false)
        {
            /*if (rich == null)*/ return;
            string path = @"D:\测量数据\" + rich.Name;
            try
            {
                try
                {

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = path + "\\" + DateTime.Now.ToString("yyyyMMdd")  + ".txt";

                    if (File.Exists(path))
                    {
                        //存在文件
                    }
                    else
                    {
                        //不存在文件
                        StreamWriter sw = File.CreateText(path);
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    FilesManager.WriteOperateLog(Common.LogBug + "保存日志至文件\\", $"写入内容[{text}],出错:{ex.Message}");
                }

                try
                {
                    File.AppendAllText(path, $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {text.Replace(Common.LINE, string.Empty).Replace(Common.ENTER, string.Empty)}\n");
                    rich.ScrollToCaret();
                }
                catch (Exception ex)
                {
                    FilesManager.WriteOperateLog(Common.LogBug + "日志写入至RichText出错\\", $"写入内容[{text}],出错:{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                FilesManager.WriteOperateLog(Common.LogBug + "日志写入至RichText出错外部Try\\", $"写入内容[{text}],最外catch出错:{ex.Message}");
            }
        }
        private static readonly object lockSaveLog = new object();
        /// <summary>
        /// 将RichTextBox内容保存到文件中
        /// </summary>
        /// <param name="rtx">控件名称</param>
        public static void SaveLogFile(RichTextBox rtx)
        {
            try
            {
                lock (lockSaveLog)
                {
                    rtx.Invoke(new EventHandler(delegate
                    {
                        if (rtx.Lines.Length <= 0 || string.IsNullOrEmpty(rtx.Text)) return;

                        var _logFile = Common.LogBug + $"{rtx.Name}\\" + (string.IsNullOrEmpty(Convert.ToString(rtx.Tag)) ? rtx.Name : Convert.ToString(rtx.Tag));
                        if (!Directory.Exists(_logFile))
                        {
                            Directory.CreateDirectory(_logFile);
                        }
                        _logFile += @"\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";

                        rtx.SaveFile(_logFile, RichTextBoxStreamType.TextTextOleObjs);

                        rtx.Clear();
                    }));
                }
            }
            catch (Exception ex)
            {
                FilesManager.WriteOperateLog(Common.LogBug + "日志写入至文件出错\\", $"出错:{ex.Message}");
            }

        }
    }
}
