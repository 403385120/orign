using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using System.IO;
using ZY.Logging;

namespace XRayClient.Core
{
    /// <summary>
    /// 图片保存设置
    /// </summary>
    public class ImageSaveConfig : ObservableObject
    {
        private bool _saveOrigOKImage = true;  // 保存原始OK图片
        private bool _saveOrigNGImage = true;

        private bool _saveTestOKImage = true;  // 保存检测OK图片
        private bool _saveTestNGImage = true;

        private EImageFormats _imageFormat = EImageFormats.BMP; // 图片格式

        private int _diskThreshold = 50;                                    // 磁盘剩余多少自动覆盖
        private string _saveOrigPath = "D:\\XRayPic\\Orig";                 // 原始图片保存路径
        private string _saveTestPath = "D:\\XRayPic\\Test";                 // 检测结果
        private string _saveOutLinePath = "-OutLine";                       // 单机路径后缀（附加在常规路径后）
        private string _saveRecheckPath = "D:\\XRayPic\\Recheck";           // 手工校验
        private bool _useDynamicTestPath = true;                            // 使用STF动态目录保存结果图片

        private bool _saveStartupTestImage = true;
        private string _startupTestPath = "D:\\XRayPic\\Startup";

        public ImageSaveConfig()
        {
            try
            {
                if (!Directory.Exists(this._saveOrigPath))
                {
                    Directory.CreateDirectory(this._saveOrigPath);
                }
                if (!Directory.Exists(this._saveTestPath))
                {
                    Directory.CreateDirectory(this._saveTestPath);
                }
                if(!Directory.Exists(this._saveRecheckPath))
                {
                    Directory.CreateDirectory(this._saveRecheckPath);
                }
                if(!Directory.Exists(this._saveOrigPath + this._saveOutLinePath))
                {
                    Directory.CreateDirectory(this._saveOrigPath + this._saveOutLinePath);
                }
                if(!Directory.Exists(this._saveTestPath + this._saveOutLinePath))
                {
                    Directory.Exists(this._saveTestPath + this._saveOutLinePath);
                }
            }
            catch
            {
                LoggingIF.Log("Failed to create image saving directory!", LogLevels.Error, "ImageSaveConfig");
            }
        }

        public bool SaveOrigOKImage
        {
            get { return this._saveOrigOKImage; }
            set
            {
                if (value == this._saveOrigOKImage) return;

                this._saveOrigOKImage = value;
                RaisePropertyChanged("SaveOrigOKImage");
            }
        }

        public bool SaveOrigNGImage
        {
            get { return this._saveOrigNGImage; }
            set
            {
                if (value == this._saveOrigNGImage) return;

                this._saveOrigNGImage = value;
                RaisePropertyChanged("SaveOrigNGImage");
            }
        }

        public bool SaveTestOKImage
        {
            get { return this._saveTestOKImage; }
            set
            {
                if (value == this._saveTestOKImage) return;

                this._saveTestOKImage = value;
                RaisePropertyChanged("SaveTestOKImage");
            }
        }

        public bool SaveTestNGImage
        {
            get { return this._saveTestNGImage; }
            set
            {
                if (value == this._saveTestNGImage) return;

                this._saveTestNGImage = value;
                RaisePropertyChanged("SaveTestNGImage");
            }
        }

        public IEnumerable<EImageFormats> BindableEImageFormats
        {
            get
            {
                return Enum.GetValues(typeof(EImageFormats))
                    .Cast<EImageFormats>();
            }
        }

        public EImageFormats ImageFormat
        {
            get { return this._imageFormat; }
            set
            {
                if (value == this._imageFormat) return;

                this._imageFormat = value;
                RaisePropertyChanged("ImageFormat");
            }
        }

        public int DiskThreshold
        {
            get { return this._diskThreshold; }
            set
            {
                if (value == this._diskThreshold) return;

                this._diskThreshold = value;
                RaisePropertyChanged("DiskThreshold");
            }
        }

        public string SaveOrigPath
        {
            get { return this._saveOrigPath; }
            set
            {
                if (value == this._saveOrigPath) return;

                this._saveOrigPath = value;
                RaisePropertyChanged("SaveOrigPath");
            }
        }

        public string SaveTestPath
        {
            get { return this._saveTestPath; }
            set
            {
                if (value == this._saveTestPath) return;

                this._saveTestPath = value;
                RaisePropertyChanged("SaveTestPath");
            }
        }

        public string SaveRecheckPath
        {
            get { return this._saveRecheckPath; }
            set
            {
                if (value == this._saveRecheckPath) return;

                this._saveRecheckPath = value;
                RaisePropertyChanged("SaveRecheckPath");
            }
        }

        public bool UseDynamicTestPath
        {
            get { return this._useDynamicTestPath; }
            set
            {
                this._useDynamicTestPath = value;
                RaisePropertyChanged("UseDynamicTestPath");
            }
        }

        public bool SaveStartupTestImage
        {
            get { return this._saveStartupTestImage; }
            set
            {
                this._saveStartupTestImage = value;
                RaisePropertyChanged("SaveStartupTestImage");
            }
        }

        public string StartupTestPath
        {
            get { return this._startupTestPath; }
            set
            {
                this._startupTestPath = value;
                RaisePropertyChanged("StartupTestPath");
            }
        }

    }
}
