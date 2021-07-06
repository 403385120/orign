using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Vision
{
    /// <summary>
    /// 常量
    /// </summary>
    public class Consts
    {
        //相机校准文件配置目录
        static readonly string cameraConfigPath = @"C:\CameraSetting";

        ///<summary>相机自动暗校准输入文件</summary>
        public static readonly string DARK_CORRECTION_FILE_INPUT = cameraConfigPath + "\\camera{0}\\darksAverage{0}.smv";
        ///<summary>相机自动暗校准输出文件</summary>
        public static readonly string DARK_CORRECTION_FILE_OUTPUT = cameraConfigPath + "\\camera{0}\\darks.smv";
        ///<summary>相机亮度校准文件</summary>
        public static readonly string FLOOD_CORRECTION_FILE_INPUT = cameraConfigPath + "\\camera{0}\\floodsAverage{0}.smv";
        ///<summary>相机亮度校准输出文件</summary>
        public static readonly string FLOOD_CORRECTION_FILE_OUTPUT = cameraConfigPath + "\\camera{0}\\floods.smv";
        ///<summary>坏线校准文件</summary>
        public static readonly string DEFECT_MAP_FILE_INPUT = cameraConfigPath + "\\camera{0}\\DefectMap.smv";

        ///<summary>图像类型</summary>
        public enum ImageTypes
        {
            ///<summary>8位图</summary>
            Eight,
            ///<summary>16位图</summary>
            Sixteen
        }

        ///<summary>相机1配置节点名称</summary>
        public const string HARDWARE_CONFIG_CAMERA = "Camera";
        ///<summary>相机旋转配置</summary>
        public const string HARDWARE_CONFIG_ROTATE = "rotate";
        ///<summary>相机最大灰度配置</summary>
        public const string HARDWARE_CONFIG_MAX_GRAY = "gray";
        ///<summary>日志记录</summary>
        public const string HARDWARE_CONFIG_LOG_LEVEL = "log_level";
        ///<summary>相机模式</summary>
        public const string HARDWARE_CONFIG_MODE = "mode";
    }
}
