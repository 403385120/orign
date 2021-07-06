using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using XRayClient.VisionSysWrapper;
using Shuyz.Framework.Mvvm;
using ZY.XRayTube;
using ATL.Common;
using ZY.Vision.Algorithm;

namespace XRayClient.Core
{
    public class BatteryCorner : ObservableObject
    {
        private DateTime _shotTime = DateTime.MinValue;               // 拍照时间

        //TODO: 更改传算法图片对象 ZhangKF 2021-3-16
        //public ZYImageStruct ShotImage; // = new ZYImageStruct();     // 图片
        public ImageTransfor ShotImage;

        // 必需public直接开放
        private bool _isShotOK = false;                                 // 拍照成功？

        private XRayTubeStatus _xRayTubeStatus = new XRayTubeStatus();  // 拍照时光管状态

        public InspectParams InspectParams = new InspectParams(1);     // 检测参数
                                                                       // public 开放减少构造次数
        public ZYResultData InspectResults = new ZYResultData(1);      // 检测结果

        private string _origImageFileName = string.Empty;   // 原始图片名称
        private string _resultImgFileName = string.Empty;   // 结果图片名称

        private int _imgNO = 0;
        /// <summary>
        /// 构造时自动分配内存
        /// </summary>
        public BatteryCorner()
        {
            //TODO: 新版相机，不需要设置图片属性  ZhangKF 2021-3-16
            //this.ShotImage.Create(ImageDefinations.MaxImgWidth, ImageDefinations.MaxImgHeight, 1);
            //this.ShotImage.width = ImageDefinations.ActualWidth;
            //this.ShotImage.height = ImageDefinations.ActualHeight;

            this.InspectResults.resultDataMin.colorMat.Create(ImageDefinations.MaxImgWidth, ImageDefinations.MaxImgHeight, 3);
            this.InspectResults.resultDataMin.colorMat.width = ImageDefinations.ActualWidth;
            this.InspectResults.resultDataMin.colorMat.height = ImageDefinations.ActualHeight;

            this.Reset();
        }

        ~BatteryCorner()
        {
            this.Destroy();
        }

        public void Destroy()
        {
            this.ShotImage.Destory();
            this.InspectResults.Destroy();
        }

        /// <summary>
        /// 清除状态(不清内存)
        /// </summary>
        public void Reset()
        {
            this.ShotTime = DateTime.MinValue;
            //this.ShotImage    // 禁止更改，此处不需要重新分配内存避免浪费
            this.IsShotOK = false;

            this.MyXRayTubeStatus = new XRayTubeStatus();

            //this.InspectParams //禁止更改
            this.InspectResults.resultDataMin.iResult = 0;

            this.OrigImageFileName = string.Empty;
            this.ResultImgFileName = string.Empty;
        }

        public void CopyTo(ref BatteryCorner b)
        {
            // TODO: 采用新版相机转换对象的克隆方法 ZhangKF 2020-3-16
            //this.ShotImage.CopyTo(ref b.ShotImage);
            if (this.ShotImage.data != IntPtr.Zero)
            {
                b.ShotImage = this.ShotImage.Clone();
            }

            b.ShotTime = this.ShotTime; // ShotImage will change ShotTime
            b.IsShotOK = this.IsShotOK;

            b.MyXRayTubeStatus = this.MyXRayTubeStatus.DeepCopy();

            b.InspectParams = this.InspectParams.DeepCopy();
            this.InspectResults.CopyTo(ref b.InspectResults);

            b.OrigImageFileName = this.OrigImageFileName;
            b.ResultImgFileName = this.ResultImgFileName;
        }

        public DateTime ShotTime
        {
            get { return this._shotTime; }
            set
            {
                if (value == this._shotTime) return;

                this._shotTime = value;
                RaisePropertyChanged("ShotTime");
            }
        }

        public bool IsShotOK
        {
            get { return this._isShotOK; }
            set
            {
                if (value == this._isShotOK) return;

                this._isShotOK = value;
                RaisePropertyChanged("IsShotOK");
            }
        }

        public int ImgNO
        {
            get { return this._imgNO; }
            set
            {
                if (value == this._imgNO) return;

                this._imgNO = value;
                RaisePropertyChanged("ImgNO");
            }
        }

        public XRayTubeStatus MyXRayTubeStatus
        {
            get { return this._xRayTubeStatus; }
            set
            {
                if (value == this._xRayTubeStatus) return;

                this._xRayTubeStatus = value;
                RaisePropertyChanged("MyXRayTubeStatus");
            }
        }

        public string OrigImageFileName
        {
            get
            {
                if (string.Empty == this._origImageFileName) return "Empty";
                return this._origImageFileName;
            }
            set
            {
                this._origImageFileName = value;
                RaisePropertyChanged("OrigImageFileName");
            }
        }

        public string ResultImgFileName
        {
            get
            {
                if (string.Empty == this._origImageFileName) return "Empty";
                return this._resultImgFileName;
            }
            set
            {
                this._resultImgFileName = value;
                RaisePropertyChanged("ResultImgFileName");
            }
        }

        public override string ToString()
        {
            return "ShotTime: " + this.ShotTime.ToString() + Environment.NewLine
                 + "IsShotOK: " + this.IsShotOK.ToString() + Environment.NewLine
                 + "Width: " + this.ShotImage.width.ToString() + Environment.NewLine
                 + "Height: " + this.ShotImage.height.ToString() + Environment.NewLine
                 + "XTubeStatus: " + this.MyXRayTubeStatus.ToString() + Environment.NewLine
                 + "OrigImage: " + this.OrigImageFileName.ToString() + Environment.NewLine
                 + "AlgoResult: " + this.InspectResults.resultDataMin.iResult.ToString() + Environment.NewLine
                 + "ImageNO: " + this.ImgNO.ToString() + Environment.NewLine
                 + "ResultImage: " + this.ResultImgFileName.ToString() + Environment.NewLine
                 + "InspectParams: " + this.InspectParams.ToString();
        }

    }
}
