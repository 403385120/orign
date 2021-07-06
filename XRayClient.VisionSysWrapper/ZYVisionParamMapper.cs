using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace XRayClient.VisionSysWrapper
{
    /**
    * @brief	距离对结构体
    */
    public struct PairDis
    {
        public float length;   //阳极与上侧阴极距离
        public float length2;  //阳极与下侧阴极距离

        public PairDis(int forceConstutor)
        {
            length = 0;
            length2 = 0;
        }
    }

    /**
     * @brief	检测参数定义
     */
    public struct InspectParams
    {
        public int iMethode;
        public int iLine;
        public int iCorner;
        public int total_layer;
        public int thread_num;

        public float pixel_to_mm;
        public float min_length;
        public float max_length;
        public float max_angle_thresh;

        [MarshalAs(UnmanagedType.U1)]
        public bool isDrawLine;
        [MarshalAs(UnmanagedType.U1)]
        public bool isShowData;
        [MarshalAs(UnmanagedType.U1)]
        public bool isShowAngle;
        [MarshalAs(UnmanagedType.U1)]
        public bool isDetectAngle;
        [MarshalAs(UnmanagedType.U1)]
        public bool bEnhanceImage;
        [MarshalAs(UnmanagedType.U1)]
        public bool bInitial;

        public RectStruct detected_rect;
        public string strBarcode;

        public InspectParams(int forceConstutor)
        {
            iMethode = 3;
            iLine = 1;
            iCorner = 1;
            total_layer = 9;
            thread_num = 1;

            pixel_to_mm = 0.0082f;
            min_length = 0.1f;
            max_length = 1.4f;
            max_angle_thresh = 90.0f;

            isDrawLine = true;
            isShowData = true;
            isDetectAngle = true;
            isShowAngle = true;
            bEnhanceImage = false;
            bInitial = false;

            strBarcode = string.Empty;
            detected_rect = new RectStruct { x = 0, y = 0, width = 90, height = 200 };
        }

        public InspectParams DeepCopy()
        {
            InspectParams p = new InspectParams(1);
            p.iMethode = this.iMethode;
            p.iLine = this.iLine;
            p.iCorner = this.iCorner;
            p.total_layer = this.total_layer;
            p.thread_num = this.thread_num;
            p.pixel_to_mm = this.pixel_to_mm;
            p.min_length = this.min_length;
            p.max_length = this.max_length;
            p.max_angle_thresh = this.max_angle_thresh;

            p.isDrawLine = this.isDrawLine;
            p.isShowData = this.isShowData;
            p.isDetectAngle = this.isDetectAngle;
            p.isShowAngle = this.isShowAngle;
            p.bEnhanceImage = this.bEnhanceImage;
            p.bInitial = this.bInitial;

            p.detected_rect = new RectStruct { x = this.detected_rect.x, y = this.detected_rect.y,
                width = this.detected_rect.width, height = this.detected_rect.height }; 
            p.strBarcode = this.strBarcode;
            return p;
	    }
        public string ToString()
        {
            string resutl = string.Empty;
            resutl = "iMethode:" + this.iMethode.ToString() + Environment.NewLine +
                "iLine:" + this.iLine.ToString() + Environment.NewLine +
                "iCorner:" + this.iCorner.ToString() + Environment.NewLine +
                "total_layer:" + this.total_layer.ToString() + Environment.NewLine +
                "thread_num:" + this.thread_num.ToString() + Environment.NewLine +
                "pixel_to_mm：" + this.pixel_to_mm.ToString() + Environment.NewLine +
                "min_length" + this.min_length.ToString() + Environment.NewLine +
                "max_length：" + this.max_length.ToString() + Environment.NewLine +
                "max_angle_thresh：" + this.max_angle_thresh.ToString() + Environment.NewLine +
                "isDrawLine：" + this.isDrawLine.ToString() + Environment.NewLine +
                "isShowData：" + this.isShowData.ToString() + Environment.NewLine +
                "isDetectAngle：" + this.isDetectAngle.ToString() + Environment.NewLine +
                "isShowAngle：" + this.isShowAngle.ToString() + Environment.NewLine +
                "bEnhanceImage：" + this.bEnhanceImage.ToString() + Environment.NewLine +
                "bInitial：" + this.bInitial.ToString() + Environment.NewLine +
                "strBarcode：" + this.strBarcode.ToString() + Environment.NewLine+
                "detected_rect:X=" + this.detected_rect.x.ToString()+",Y="+this.detected_rect.y.ToString()+",WIDTH="+this.detected_rect.width.ToString()+",HEIGHT="+this.detected_rect.height.ToString();
            return resutl;
        }

        public void CopyTo(ref InspectParams p)
        {
            p.iMethode = this.iMethode;
            p.iLine = this.iLine;
           // p.iCorner = this.iCorner;
            p.total_layer = this.total_layer;
            p.pixel_to_mm = this.pixel_to_mm;
            p.min_length = this.min_length;
            p.max_length = this.max_length;
            p.max_angle_thresh = this.max_angle_thresh;

            p.isDrawLine = this.isDrawLine;
            p.isShowData = this.isShowData;
            p.isShowAngle = this.isShowAngle;
            p.isDetectAngle = this.isDetectAngle;

            p.detected_rect = new RectStruct
            {
                x = this.detected_rect.x,
                y = this.detected_rect.y,
                width = this.detected_rect.width,
                height = this.detected_rect.height
            };
        }
    }

    /**
    * @brief	返回结果结构体
    * 注意： 此结构体与C++不兼容！禁止使用
    */
    public struct ZYResultData
    {
        public float[] vecDis;
        public PairDis[] vecPairDis;
        public float[] vecAngles;
        public ZYResultDataMin resultDataMin;

        public ZYResultData(int forceConstutor)
        {
            vecDis = new float[50];
            vecPairDis = new PairDis[50];
            vecAngles = new float[50];
            resultDataMin = new ZYResultDataMin(1);
        }

        public void ResetArray()
        {
            
            int layers = 50;

            //if (layers < 1) throw new System.Exception("Illegel parameter!");

            vecDis = new float[layers];
            vecPairDis = new PairDis[layers];
            vecAngles = new float[layers];
        }

        public void CopyTo(ref ZYResultData r)
        {
            //r.vecDis = new float[this.vecDis.Count()];
            this.vecDis.CopyTo(r.vecDis, 0);

            //r.vecPairDis = new PairDis[this.vecPairDis.Count()];
            this.vecPairDis.CopyTo(r.vecPairDis, 0);

            //r.vecAngles = new float[this.vecAngles.Count()];
            this.vecAngles.CopyTo(r.vecAngles, 0);

            this.resultDataMin.CopyTo(ref r.resultDataMin);
        }

        public void Destroy()
        {
            this.resultDataMin.Destroy();
        }
    }

    /**
    * @brief	返回结果结构体
    */
    public struct ZYResultDataMin
    {
        //public float[] vecDis;
        //public PairDis[] vecPairDis;
        //public float[] vecAngles;
        public float fMinDis;
        public float fMaxDis;
        public float fMeanDis;
        public float fContrast; // 对比度
        public int iResult; // 结果标志 1->OK, 0->NG
        public ZYImageStruct colorMat;  // 返回结果图
        public string strBarcode;

        public ZYResultDataMin(int forceConstutor)
        {
            //vecDis = new float[100];
            //vecPairDis = new PairDis[100];
            //vecAngles = new float[100];
            fMinDis = 0;
            fMaxDis = 0;
            fMeanDis = 0;
            fContrast = 0;
            iResult = 0;
            colorMat = new ZYImageStruct { data = (IntPtr)null, width = 0, height = 0, channel = 0 };
            strBarcode = string.Empty;
        }

        public void CopyTo(ref ZYResultDataMin r)
        {
            //r.vecDis = new float[this.vecDis.Count()];
            //this.vecDis.CopyTo(r.vecDis, 0);
            //r.vecPairDis = new PairDis[this.vecPairDis.Count()];
            //this.vecPairDis.CopyTo(r.vecPairDis, 0);
            //r.vecAngles = new float[this.vecAngles.Count()];
            //this.vecAngles.CopyTo(r.vecAngles, 0);
            r.fMinDis = this.fMinDis;
            r.fMaxDis = this.fMaxDis;
            r.fMeanDis = this.fMeanDis;
            r.fContrast = this.fContrast;
            r.iResult = this.iResult;
            this.colorMat.CopyTo(ref r.colorMat);
            r.strBarcode = this.strBarcode;
        }

        public void Destroy()
        {
            this.colorMat.Destory();
        }
    }

    /**
     * @brief	相机参数定义
     */
    public struct CameraParams
    {
        public int camType;         // 相机类型(厂家)(暂定0为堡盟相机)

        public int nWidth;          //相机图像的宽度
        public int nHeight;         //相机图像的高度

        public int PinValue;        //相机的平均滤波系数
        public string SzBufSeriNum; //相机的序列号

        public float pixelRatio;    // 像素比
		public float pixelRatio2;    // 像素比2
        public int exposure;        // 曝光时间
        public int gain;            // 增益
		
		public int min_graylevel;//最小灰度值
        public int max_graylevel;//最大灰度值

        [MarshalAs(UnmanagedType.U1)]
        public bool xFlip;         //X方向偏转
        [MarshalAs(UnmanagedType.U1)]
        public bool yFlip;			//Y方向偏转

        public CameraParams(int forceConstutor)
        {
            camType = 0;
            nWidth = 1600;
            nHeight= 1200;
            PinValue = 8;
            exposure = 40000;
            gain = 1;
            pixelRatio = 1;
			pixelRatio2 = 1;
            SzBufSeriNum = string.Empty;
            xFlip = false;
            yFlip = false;
			min_graylevel = 10;
            max_graylevel = 80;
        }
        public string ToString()
        {
            string result = string.Empty;
            result = "camType:" + this.camType.ToString() + Environment.NewLine +
                "nWidth:" + this.nWidth.ToString() + Environment.NewLine +
                "nHeight:" + this.nHeight.ToString() + Environment.NewLine +
                "PinValue:" + this.PinValue.ToString() + Environment.NewLine +
                "exposure:" + this.exposure.ToString() + Environment.NewLine +
                "gain:" + this.gain.ToString() + Environment.NewLine +
                "pixelRatio:" + this.pixelRatio.ToString() + Environment.NewLine +
                "pixelRatio2:" + this.pixelRatio2.ToString() + Environment.NewLine +
                "SzBufSeriNum:" + this.SzBufSeriNum + Environment.NewLine +
                "xFlip:" + this.xFlip.ToString() + Environment.NewLine +
                "yFlip:" + this.yFlip.ToString() + Environment.NewLine +
                "min_graylevel:" + this.min_graylevel.ToString() + Environment.NewLine +
                "max_graylevel:" + this.max_graylevel.ToString() + Environment.NewLine;
            return result;
        }
        public CameraParams DeepCopy()
        {
            CameraParams c = new CameraParams();

            c.camType = this.camType;
            c.nWidth = this.nWidth;
            c.nHeight = this.nHeight;
            c.PinValue = this.PinValue;
            c.SzBufSeriNum = this.SzBufSeriNum;
            c.pixelRatio = this.pixelRatio;
			c.pixelRatio2 = this.pixelRatio2;
            c.exposure = this.exposure;
            c.gain = this.gain;
            c.xFlip = this.xFlip;
            c.yFlip = this.yFlip;
			c.min_graylevel = this.min_graylevel;
            c.max_graylevel = this.max_graylevel;

            return c;
        }
    }

}
