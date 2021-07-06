using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayClient.VisionSysWrapper;
using System.Drawing;
using ZY.Vision.Algorithm;

namespace XRayClient.Core
{
    /// <summary>
    /// 单实例算法类
    /// </summary>
    public class AlgoWrapper
    {
        private static AlgoWrapper _instance = new AlgoWrapper();
        public static AlgoWrapper Instance
        {
            get { return _instance; }
        }

        private AlgoWrapper()
        {
            Bitmap bat32 = Resources.Images.BatterySample;
            Bitmap bat24 = this.ConvertTo24Bit(bat32);
            
            this._imageBattery = new ZYImageStruct(bat24);
        }

        private Bitmap ConvertTo24Bit(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }

        ~AlgoWrapper()
        {
            this._imageBattery.Destory();
        }

        private ZYImageStruct _imageBattery = new ZYImageStruct();

        public int InspectPreCorner(BatteryCorner corner)
        {
            int iRet = VisionSysWrapperIF.InspectPre(ref corner.InspectParams, ref corner.ShotImage);
            if(iRet < 0)
            {
                corner.InspectResults.resultDataMin.iResult = iRet;
            }

            return iRet;
        }

        //TODO:替换算法传递类型 ZhangKF 2021-3-16
        public int ImageProcessInspect(ref InspectParams Params, ref ImageTransfor image, ref ZYResultData preResData, int iMethode,int imaNo)
        {
            bool iRet;
            //if (iMethode == 3)//TIFF算法
            //{
            iRet = VisionSysWrapperIF.ImageProcessInspect_DL(Params.strBarcode, ref Params, ref image, preResData.vecDis, preResData.vecPairDis, preResData.vecAngles, ref preResData.resultDataMin, iMethode, imaNo);
            //}
            //else
            //{
            //    iRet = VisionSysWrapperIF.ImageProcessInspect_DL(Params.strBarcode, ref Params, ref image, preResData.vecDis, preResData.vecPairDis, preResData.vecAngles, ref preResData.resultDataMin);
            //}
            if (iRet)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int InspectBackCorner(BatteryCorner corner)
        {
            int iRet = VisionSysWrapperIF.InspectBack(ref corner.InspectParams, ref corner.ShotImage);
            if (iRet < 0)
            {
                corner.InspectResults.resultDataMin.iResult = iRet;
            }

            return iRet;
        }

        public int GetInspectResult(ref ZYResultData preResData, ref ZYResultData backResData)
        {
            int iRet = VisionSysWrapperIF.GetInspectResult(ref preResData, ref backResData);

            return iRet;
        }

        public int GetResultImage(ref BatterySeat seat)
        {
            int iRet =  VisionSysWrapperIF.GetResultImage(ref seat.Corner1.InspectResults.resultDataMin.colorMat,
                ref seat.Corner2.InspectResults.resultDataMin.colorMat, 
                ref seat.Corner3.InspectResults.resultDataMin.colorMat, 
                ref seat.Corner4.InspectResults.resultDataMin.colorMat, 
                ref this._imageBattery,
                seat.FinalResult, 
                (seat.Sn == string.Empty ? "Empty" : seat.Sn), ref seat.ResultImage);

            return iRet;
        }
    }
}
