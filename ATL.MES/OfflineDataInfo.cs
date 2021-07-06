using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATL.MES
{
    /// <summary>
    /// 当3次未回复指令为A015、A013、A029、A031、A033、A035、A045、A051、A055时需要向设备发出停机指令，并启动设备的有声报警信号提示MES发生异常需要介入处理，MES异常处理完成后，按下设备的启动按钮，上位机软件收到启动信号后首先将未收到回复数据区域的数据逐条上传mes，接收到回复后按照正常的逻辑进行处理，实现一键恢复运行的功能
    /// </summary>
    public class OfflineDataInfo
    {
        public int ID { get; set; }
        public DateTime LogDateTime = DateTime.Now;
        public string FunctionID { get; set; }
        public string Guid { get; set; }
        /// <summary>
        /// 缓存中待发送的JASON数据
        /// </summary>
        public string Data { get; set; }
        public string EquipmentID { get; set; }
        public string Cellbarcode { get; set; }
    }

    public class OfflineBarcode
    {
        public string EquipmentID { get; set; }
        public string Barcode { get; set; }
    }

    public class A049RequestInfo
    {
        public static List<A049RequestInfo> lstA049RequestInfo = new List<A049RequestInfo>();
        public List<string> ProductSN { get; set; }
        public List<A049.MaterialInfoItem> MaterialInfo { get; set; }
    }
    
    public class MESmsg
    {
        public static object lockObj = new object();
        public MESmsg()
        {
            LogDateTime = DateTime.Now;
            try
            {
                lock(lockObj)
                {
                    if (lstMESmsg.Count > 500)
                        lstMESmsg.RemoveAt(0);
                }
            }
            catch { }
        }
        public static List<MESmsg> lstMESmsg = new List<MESmsg>();
        public DateTime LogDateTime { get; set; }
        public string FunctionID { get; set; }
        /// <summary>
        /// 将JASON翻译成普通人可以看懂的
        /// </summary>
        public string translation { get; set; }
        public static void Add(MESmsg msg)
        {
            lock(lockObj)
            {
                lstMESmsg.Add(msg);
            }
        }
    }

    /// <summary>
    /// MES登录的账户
    /// </summary>
    public struct UserInfo
    {
        private static string userID;
        private static string userLevel;
        private static string userName;

        public static string UserID
        {
            get
            {
                return userID == null ? string.Empty : userID;
            }
            set
            {
                userID = value;
            }
        }

        public static string UserLevel
        {
            get
            {
                return userLevel == null ? string.Empty : userLevel;
            }
            set
            {
                userLevel = value;
            }
        }
        /// <summary>
        /// A007 MES下发和A40返回帐号
        /// </summary>
        public static string UserName
        {
            get
            {
                return userName == null ? string.Empty : userName;
            }
            set
            {
                userName = value;
                ATL.Core.UserInfo.CurrentUserName = value;
            }
        }
    }
    [Serializable]
    public class A047RequestInfo
    {
        public string EquipmentID { get; set; }
        private string modelInfo;
        public string ModelInfo
        {
            get
            {
                return modelInfo == null ? string.Empty : modelInfo;
            }
            set
            {
                modelInfo = value;
            }
        }
        public A047UserInfo UserInfo { get; set; }
        public List<A047.MaterialInfoItem> MaterialInfo { get; set; }
    }
    [Serializable]
    public class A047UserInfo
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserLevel { get; set; }

    }
}
