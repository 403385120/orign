using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Vision.Utils
{
    /// <summary>
    /// 相机拍照结果，只是对Bitmap包装，方便与其它程序对接
    /// ZhangKF 2020-12-30
    /// </summary>
    public class CaptureResult
    {
        ///<summary>拍照结果图像</summary>
        public Bitmap Bitmap
        {
            get;
            set;
        }

        ///<summary>针对八位图同时获取对应的TIF原图 ZhangKF 2021-3-9</summary>
        public Bitmap TIFImage
        {
            get;
            set;
        }

        ///<summary>拍照结果：true-成功/false-失败</summary>
        public bool Result
        {
            get;
            set;
        }

        ///<summary>拍照异常消息</summary>
        public string ErrorMessage
        {
            get;
            set;
        }
        //end class
    }
}
