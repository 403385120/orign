using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Vision.Varex;

namespace ZY.Vision.Interfaces
{
    /// <summary>
    /// 通用相机接口，让系统可以兼容不同的相机
    /// ZhangKF 2021-1-13
    /// </summary>
    public interface ICameraDevice
    {
        ///<summary>相机参数</summary>
        CameraParameter CameraParam
        {
            get;
            set;
        }

        ///<summary>多帧拍照,frame拍照次数</summary>
        Bitmap CaptureOneImage(int frame);

        ///<summary>拍照并返回相片对象,frame - 拍照次数, 可以返回TIF图 2021-3-9 ZhangKF</summary>
        Bitmap CaptureOneImageAndTIF(int frame, ref Bitmap tif);

        ///<summary>单帧或根据相机参数进行拍照</summary>
        Bitmap CaptureOneImage();

        ///<summary>关闭相机</summary>
        void CloseCamera();
        //end interface
    }
}
