using ATL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XRayClient.Core;
using XRayClient.VisionSysWrapper;
using ZY.Logging;
using ZYXray.Utils;
using ZYXray.ViewModels;

namespace ZYXray.Winform
{
    public partial class frmRecheckImage : Form
    {
        public frmRecheckImage()
        {
            InitializeComponent();
        }
        public static frmRecheckImage Current;
        public FullScreenToggleHandler FullScreenToggleHandlers = null;


        private ImageControlVm _imageControlVmLeft = new ImageControlVm();
        private ImageControlVm _imageControlVmRight = new ImageControlVm();

        private ZYImageStruct _leftImageStruct = new ZYImageStruct();
        private ZYImageStruct _rightImageStruct = new ZYImageStruct();

        private ManualRecheck _manualRecheckLogic = new ManualRecheck();
        private AuthBoxVm _authBoxVm = new AuthBoxVm();

        private string _fqauserid = string.Empty;
        private string _prduserid = string.Empty;
        private string _currentuserid = string.Empty;
        public ImageControlVm ImageLeft { get; set; }
        public ImageControlVm ImageRight { get; set; }

        private bool _isFullScreen = false;
        private bool _isBusy = false;
        private bool _isAuthVisible = true;
        ManualRecheck manual = new ManualRecheck();

        private bool _isPaused = false;         // 是否暂停
        private bool _isPausedRecheck = false;  // 是否在复检模式下暂停

        private void FrmRecheckImage_Load(object sender, EventArgs e)
        {
            this._imageControlVmLeft.IsCrossVisible = false;
            this._imageControlVmRight.IsCrossVisible = false;

            int mode = int.Parse(UserDefineVariableInfo.DicVariables["IsRecheckMode"].ToString());
            this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode = mode == 1 ? true : false;

            // 登陆关闭
            //this._authBoxVm.MyExitEventHandler += delegate (bool isNotWaitCheck)
            //{
            //    this.ToggleFullScreen.Execute(null);
            //};

            // 登陆成功(线程调用！)
            // 获取列表，初始化
            // 显示第一份图片

            try
            {
                if (mode == 1)
                {
                    _prduserid = this._authBoxVm.UserName;
                    _currentuserid = _prduserid;
                }
                else if (mode == 0)
                {
                    _fqauserid = this._authBoxVm.UserName;
                    _currentuserid = _fqauserid;
                }

                manual.GetReady(this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode, false);
                this.UpdateImage();
                //    Application.Current.Dispatcher.Invoke((Action)(() =>
                //    {
                //        if (null != page)
                //        {
                //            page.Focusable = true;
                //            Keyboard.Focus(page);
                //        }

                //       
                //    }));
            }
            catch (System.Exception ex)
            {
                LoggingIF.Log("Failed to update recheck list" + ex.Message.ToString(), LogLevels.Error, "Recheck");

                System.Windows.MessageBox.Show("Failed to update recheck list" + ex.Message.ToString());

                return;
            }
            finally
            {
                //this.IsAuthVisible = false;
            }
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        private void UpdateImage()
        {
            string img1 = string.Empty;
            string img2 = string.Empty;

            try
            {
                this._manualRecheckLogic.GetImagePath(ref img1, ref img2);
                if (!File.Exists(img1) || !File.Exists(img2))
                {
                    return;

                    if (this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode)
                    {
                        btnMarkNG_Click(null, null);    //图片不存在时，该电池判为NG
                    }

                    throw new Exception(LangHelper.getTranslation("LL_ImgNotFound") + Environment.NewLine + img1 + Environment.NewLine + img2);
                }

                using (Bitmap bmp = new Bitmap(img1))
                {
                    this._leftImageStruct.UpdateWithBitmap(bmp);
                }

                using (Bitmap bmp = new Bitmap(img2))
                {
                    this._rightImageStruct.UpdateWithBitmap(bmp);
                }

                this.ImageLeft.UpdateImage(ref this._leftImageStruct);
                this.ImageRight.UpdateImage(ref this._rightImageStruct);
            }
            catch (System.Exception ex)
            {
                LoggingIF.Log("Failed to update display image:" + ex.Message.ToString(), LogLevels.Error, "Recheck");

                System.Windows.MessageBox.Show("Failed to update display image:" + ex.Message.ToString());
            }
        }

        private void btnMarkNG_Click(object sender, EventArgs e)
        {
            if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode)
            {
                System.Windows.MessageBox.Show("FQA状态下标记快捷键无效，请使用鼠标点击！");

                return;
            }

            try
            {
                this._manualRecheckLogic.MarkNG();

                this._manualRecheckLogic.NavNext();
                this.UpdateImage();
            }
            catch (System.Exception ex)
            {
                LoggingIF.Log("Failed to mark NG:" + ex.Message.ToString(), LogLevels.Error, "Recheck");

                System.Windows.MessageBox.Show("Failed to mark NG:" + ex.Message.ToString());
            }
        }

        private void btnMarkOK_Click(object sender, EventArgs e)
        {
            if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode)
            {
                System.Windows.MessageBox.Show("FQA状态下标记快捷键无效，请使用鼠标点击！");

                return;
            }

            try
            {
                this._manualRecheckLogic.MarkNG();

                this._manualRecheckLogic.NavNext();
                this.UpdateImage();
            }
            catch (System.Exception ex)
            {
                LoggingIF.Log("Failed to mark NG:" + ex.Message.ToString(), LogLevels.Error, "Recheck");

                System.Windows.MessageBox.Show("Failed to mark NG:" + ex.Message.ToString());
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this._manualRecheckLogic.NavNext();
            this.UpdateImage();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            this._manualRecheckLogic.NavPrev();
            this.UpdateImage();
        }

        private void btnWaitCheck_Click(object sender, EventArgs e)
        {
            //if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode  == "true")
            //{
            //    System.Windows.MessageBox.Show("FQA状态下标记快捷键无效，请使用鼠标点击！");

            //    return;
            //}

            try
            {
                this._manualRecheckLogic.WaitCheck();

                this._manualRecheckLogic.NavNext();
                this.UpdateImage();
            }
            catch (System.Exception ex)
            {
                LoggingIF.Log("Failed to mark NG:" + ex.Message.ToString(), LogLevels.Error, "Recheck");

                System.Windows.MessageBox.Show("Failed to mark NG:" + ex.Message.ToString());
            }
        }
    }
}
