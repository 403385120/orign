#if VAREX

using Dexela_NET;
using DexelaDefs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Vision.Varex
{
    /// <summary>
    /// 相机校准帮助类
    /// ZhangKF 2022-12-22
    /// </summary>
    public class CameraCorrectHelper
    {
        /// <summary>
        /// 相机校准
        /// </summary>
        /// <param name="cameraIndex">相机的序列号，不同的相机从不同的文件夹读取校准文件（1 - 一号相机 2 - 二号相机）</param>
        public static void Correct(int cameraIndex)
        {
            if (cameraIndex != 1 && cameraIndex != 2)
            {
                throw new ArgumentException("相机索引只能为1或2");
            }

            //自动暗校准
            CheckFileExist(string.Format(Consts.DARK_CORRECTION_FILE_INPUT,cameraIndex));
            DexImage_NET inputDark = new DexImage_NET(string.Format(Consts.DARK_CORRECTION_FILE_INPUT,cameraIndex));
            inputDark.FindMedianofPlanes();
            inputDark.SetAveragedFlag(true);
            inputDark.SetImageType(DexImageTypes_NET.Offset);
            inputDark.WriteImage(string.Format(Consts.DARK_CORRECTION_FILE_OUTPUT, cameraIndex));

            //自动亮校准
            CheckFileExist(string.Format(Consts.FLOOD_CORRECTION_FILE_INPUT,cameraIndex));
            DexImage_NET inputflood = new DexImage_NET(string.Format(Consts.FLOOD_CORRECTION_FILE_INPUT,cameraIndex));
            CheckFileExist(string.Format(Consts.DARK_CORRECTION_FILE_OUTPUT,cameraIndex));
            inputflood.LoadDarkImage(string.Format(Consts.DARK_CORRECTION_FILE_OUTPUT,cameraIndex));
            inputflood.FindMedianofPlanes();
            inputflood.SetAveragedFlag(true);
            inputflood.SubtractDark();
            inputflood.SetDarkCorrectedFlag(true);
            inputflood.FixFlood();
            inputflood.SetFixedFlag(true);
            inputflood.SetImageType(DexImageTypes_NET.Gain);
            inputflood.WriteImage(string.Format(Consts.FLOOD_CORRECTION_FILE_OUTPUT, cameraIndex));
        }

        ///<summary>检查校准文件存在</summary>
        private static void CheckFileExist(string file)
        {
            if (!File.Exists(file))
            {
                throw new Exception(string.Format("相机校准文件不存在:{0}", Consts.DARK_CORRECTION_FILE_INPUT));
            }
        }

        //end class
    }
}

#endif