using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using ZY.Vision;
using ZY.Vision.Varex;

namespace ZY.Vision.Algorithm
{
    /// <summary>
    /// 16位图转8位图
    /// ZhangKF 2020-12-28
    /// </summary>
    public class Short2Byte
    {
        ///<summary>16位颜色与8位颜色的对照表</summary>
        static ushort[] _color_tables = new ushort[65536];
        static ushort[] _color_tables_8 = new ushort[65536];

        ///<summary>16位颜色阀值,超过最大阀值将赋最大值</summary>
        static ushort _COLOR_MIN = 0;
        static ushort _COLOR_MAX = 4000;

        ///<summary>当前待处理的位图</summary>
        Bitmap _bitmap = null;

        static Short2Byte()
        {
            //TODO:取相机1的值作为灰度标准值，避免每次拍照传递此参数 ZhangKF 2021-4-10
            #region 取灰度值
            string val = CameraParameter.ReadConfigItem(Consts.HARDWARE_CONFIG_CAMERA, Consts.HARDWARE_CONFIG_MAX_GRAY);
            if (!string.IsNullOrEmpty(val))
                _COLOR_MAX = ushort.Parse(val);
            #endregion
            InitColorTables();
        }

        ///<summary>从位图对象构造对象</summary>
        public Short2Byte(Bitmap bitmap)
        {
            this._bitmap = bitmap;
        }

        ///<summary>初始化颜色对照表</summary>
        public static void InitColorTables()
        {
            ushort new_color = 0;
            ushort gay = (ushort)(_COLOR_MAX - _COLOR_MIN);
            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                _color_tables[i] = (ushort)i;
                if (_color_tables[i] <= _COLOR_MIN)
                {
                    _color_tables[i] = 0;
                }
                else
                {
                    if (_color_tables[i] >= _COLOR_MAX)
                    {
                        _color_tables[i] = ushort.MaxValue;
                    }
                    else
                    {
                        new_color = (ushort)(ushort.MaxValue * (i - _COLOR_MIN) / gay + 0.5f);
                        _color_tables[i] = new_color;
                    }
                }
            }

            for (int i = 0; i <= ushort.MaxValue; i++)
            {
                _color_tables_8[i] = (byte)((i + 256) / 256 - 1);
            }
        }

        ///<summary>从16位图获取8位图</summary>
        public Bitmap ToByteImage()
        {
            ushort[] data = this._bitmap.ToBytes_16();
            byte[] newData = new byte[data.Length];
            var color = 0;
            for (int i = 0; i < data.Length; i++)
            {
                color = data[i];
                var c1 = _color_tables[color];
                var c2 = (byte)_color_tables_8[c1];
                newData[i] = c2;
            }

            Bitmap bitmap = new Bitmap(_bitmap.Width, _bitmap.Height, PixelFormat.Format8bppIndexed);
            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            BitmapData imageData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int size = newData.Length * sizeof(byte);
            Share.CopyFromArray(newData, 0, imageData.Scan0, size);

            bitmap.UnlockBits(imageData);

            return bitmap;
        }

        ///<summary>获取8位图像的字节数组</summary>
        public byte[] ToBytes()
        {
            ushort[] data = this._bitmap.ToBytes_16();
            byte[] newData = new byte[data.Length];
            var color = 0;
            for (int i = 0; i < data.Length; i++)
            {
                color = data[i];
                var c1 = _color_tables[color];
                var c2 = (byte)_color_tables_8[c1];
                newData[i] = c2;
            }

            return newData;
        }
        //end class
    }
}
