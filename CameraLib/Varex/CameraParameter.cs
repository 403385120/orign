using DexelaDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ZY.Vision.Exceptions;

namespace ZY.Vision.Varex
{
    /// <summary>
    /// 初始化相机的参数，如图片尺寸、相机类型， 用于与校准文件一起对相要进行校准
    /// 此类从VisionSysWrapper移置过来
    /// ZhangKF 2020-12-22
    /// </summary>
    public class CameraParameter
    {
        #region API
        ///<summary>配置文件</summary>
        private static readonly string _configFile = System.Environment.CurrentDirectory + "\\HardwareConfig.ini";
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region 相机属性
        ///<summary>相机类型(厂家)(暂定0为堡盟相机)</summary>
        public int camType;
        ///<summary>相机的平均滤波系数</summary>
        public int PinValue;
        ///<summary>相机的序列号</summary>
        public string SzBufSeriNum;
        ///<summary>像素比</summary>
        public float pixelRatio;
        ///<summary>像素比2</summary>
        public float pixelRatio2;
        ///<summary>曝光时间</summary>
        public int gain;
        ///<summary>最小灰度值</summary>
        public int min_graylevel;
        ///<summary>最大灰度值</summary>
        public int max_graylevel;
        ///<summary>X方向偏转</summary>
        public bool xFlip;
        ///<summary>Y方向偏转</summary>
        public bool yFlip;
        #endregion

        ///<summary>拍照相片宽度</summary>
        public int ImageWidth
        {
            get;
            set;
        }

        ///<summary>拍照相片度度</summary>
        public int ImageHeight
        {
            get;
            set;
        }

        ///<summary>曝光时间</summary>
        public int ExposeTime
        {
            get;
            set;
        }

        ///<summary>构造函数：赋初值</summary>
        public CameraParameter(int forceConstutor)
        {
            camType = 0;
            this.ImageWidth = 1600;
            this.ImageHeight = 1200;
            PinValue = 8;
            this.ExposeTime = 100;
            gain = 1;
            pixelRatio = 1;
            pixelRatio2 = 1;
            SzBufSeriNum = string.Empty;
            xFlip = false;
            yFlip = false;
            min_graylevel = 10;
            max_graylevel = 80;

            //相机高低模式
            string mode = ReadConfigItem(Consts.HARDWARE_CONFIG_CAMERA, Consts.HARDWARE_CONFIG_MODE);
            if (!string.IsNullOrEmpty(mode))
            {
                this.CaptureMode = (FullWellModes_NET)Enum.Parse(typeof(FullWellModes_NET), mode);
            }
        }

        ///<summary>相机拍照模式（High/Low）</summary>
        public FullWellModes_NET CaptureMode
        {
            get;
            set;
        } = FullWellModes_NET.Low;

        ///<summary>相机拍照层数</summary>
        public int CaptrueFrame
        {
            get;
            set;
        } = 3;

        ///<summary>从配置文件hardwareConfig.ini获取相机配置，section取值（Camera1Config/Camera2Config）</summary>
        public static CameraParameter ReadConfig(string section)
        {
            CameraParameter camParam = new CameraParameter(1);

            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString(section, "camType", @"0", builder, 1024, _configFile);
            camParam.camType = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "nWidth", @"1600", builder, 1024, _configFile);
            camParam.ImageWidth = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "nHeight", @"1200", builder, 1024, _configFile);
            camParam.ImageHeight = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "PinValue", @"8", builder, 1024, _configFile);
            //camParam.PinValue = int.Parse(builder.ToString());
            //TODO:替换参数来源 ZhangKF 2021-4-10
            camParam.CaptrueFrame = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "exposure", @"100", builder, 1024, _configFile);
            camParam.ExposeTime = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "gain", @"1", builder, 1024, _configFile);
            camParam.gain = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "min_graylevel", @"0", builder, 1024, _configFile);
            camParam.min_graylevel = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "max_graylevel", @"4000", builder, 1024, _configFile);
            camParam.max_graylevel = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "pixelRatio", @"1", builder, 1024, _configFile);
            camParam.pixelRatio = float.Parse(builder.ToString());

            GetPrivateProfileString(section, "pixelRatio2", @"1", builder, 1024, _configFile);
            camParam.pixelRatio2 = float.Parse(builder.ToString());

            GetPrivateProfileString(section, "SzBufSeriNum", @"8777591518", builder, 1024, _configFile);
            camParam.SzBufSeriNum = builder.ToString();

            GetPrivateProfileString(section, "xFlip", @"false", builder, 1024, _configFile);
            camParam.xFlip = bool.Parse(builder.ToString());

            GetPrivateProfileString(section, "yFlip", @"false", builder, 1024, _configFile);
            camParam.yFlip = bool.Parse(builder.ToString());

            return camParam;
        }

        ///<summary>读取相机配置参数,sectionName 父级节点名称 itemName 子节点名称</summary>
        public static string ReadConfigItem(string sectionName, string itemName)
        {
            string val = string.Empty;
            try
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString(sectionName, itemName, @"0", builder, 1024, _configFile);
                string str = builder.ToString();
                var arr = str.Split('#');
                val = arr[0].Trim();
            }
            catch(Exception ex)
            {
                Share.Error("配置获取失败：" + ex.Message);
                Share.Error(ex.StackTrace);
            }

            return val;
        }

        //end class
    }
}
