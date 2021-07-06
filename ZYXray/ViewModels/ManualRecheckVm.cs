using System;
using System.Linq;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using XRayClient.Core;
using XRayClient.VisionSysWrapper;
using System.Drawing;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ZY.Logging;
using System.IO;
using ZYXray.Utils;
using XRayClient.BatteryCheckManager;
using ATL.Core;


namespace ZYXray.ViewModels
{
    public class ManualRecheckVm : ObservableObject
    {
        public FullScreenToggleHandler FullScreenToggleHandlers = null;
        private ManualRecheckPage page = null;

        private ImageControlVm _imageControlVmLeft = new ImageControlVm();
        private ImageControlVm _imageControlVmRight = new ImageControlVm();

        private ZYImageStruct _leftImageStruct = new ZYImageStruct();
        private ZYImageStruct _rightImageStruct = new ZYImageStruct();

        private ManualRecheck _manualRecheckLogic = new ManualRecheck();
        private AuthBoxVm _authBoxVm = new AuthBoxVm();

        private string _fqauserid = string.Empty;
        private string _prduserid = string.Empty;
        private string _currentuserid = string.Empty;

        private bool _isFullScreen = false;
        private bool _isBusy = false;
        private bool _isAuthVisible = true;

        private bool _isPaused = false;         // 是否暂停
        private bool _isPausedRecheck = false;  // 是否在复检模式下暂停

        public ManualRecheckVm()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                throw new Exception("Not allowed");
            }
        }

        public ManualRecheckVm(ManualRecheckPage page)
        {
            this._imageControlVmLeft.IsCrossVisible = false;
            this._imageControlVmRight.IsCrossVisible = false;

            this.page = page;

            int mode =  int.Parse(UserDefineVariableInfo.DicVariables["IsRecheckMode"].ToString());
            this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode = mode == 1 ? true : false;

            // 登陆关闭
            this._authBoxVm.MyExitEventHandler += delegate (bool isNotWaitCheck)
            {
                this.ToggleFullScreen.Execute(null);
            };

            // 登陆成功(线程调用！)
            // 获取列表，初始化
            // 显示第一份图片
            this._authBoxVm.MyLoginEventHandler += delegate (bool isNotWaitCheck)
            {
                try
                {
                    if(mode == 1)
                    {
                        _prduserid = this._authBoxVm.UserName;
                        _currentuserid = _prduserid;
                    }
                    else if(mode == 0)
                    {
                        _fqauserid = this._authBoxVm.UserName;
                        _currentuserid = _fqauserid;
                    }

                    this.MyManualRecheck.GetReady(this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode, false);

                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        if (null != page)
                        {
                            page.Focusable = true;
                            Keyboard.Focus(page);
                        }

                        this.UpdateImage();
                    }));
                }
                catch (System.Exception ex)
                {
                    LoggingIF.Log("Failed to update recheck list" + ex.Message.ToString(), LogLevels.Error, "Recheck");

                    System.Windows.MessageBox.Show("Failed to update recheck list" + ex.Message.ToString());

                    return;
                }
                finally
                {
                    this.IsAuthVisible = false;
                }
            };
        }

        public int ImageWidth
        {
            get { return ImageDefinations.ActualWidth; }
        }

        public int ImageHeight
        {
            get { return ImageDefinations.ActualHeight; }
        }

        public bool IsFullScreen
        {
            get { return this._isFullScreen; }
            set
            {
                this._isFullScreen = value;
                RaisePropertyChanged("IsFullScreen");
            }
        }

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                this._isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        public bool IsAuthVisible
        {
            get { return this._isAuthVisible; }
            set
            {
                this._isAuthVisible = value;
                RaisePropertyChanged("IsAuthVisible");
            }
        }

        public ImageControlVm ImageLeft
        {
            get { return this._imageControlVmLeft; }
        }

        public ImageControlVm ImageRight
        {
            get { return this._imageControlVmRight; }
        }

        public ManualRecheck MyManualRecheck
        {
            get { return this._manualRecheckLogic; }
        }

        public AuthBoxVm MyAuthBoxVm
        {
            get { return this._authBoxVm; }
            set
            {
                this._authBoxVm = value;
                RaisePropertyChanged("MyAuthBoxVm");
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public ICommand Pause
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    this._isPaused = true;
                    this._isPausedRecheck = this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode;

                    this.IsFullScreen = false;

                    // Call 外部逻辑以便本页面显示最前端
                    if (null != FullScreenToggleHandlers)
                    {
                        try
                        {
                            this.FullScreenToggleHandlers(this._isFullScreen);
                        }
                        catch { }
                    }
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand ToggleFullScreen
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    // 退出提示保存
                    if (this.IsFullScreen && !this.IsAuthVisible)
                    {
                        System.Windows.MessageBox.Show("关闭此窗口将丢失所有修改，请确认你已经提交正在进行的工作！\r\n临时退出可点击最小化按钮，以便处理机台后继续恢复您的工作！");
                    }

                    // 反转
                    this.IsFullScreen = !this.IsFullScreen;

                    bool _needReload = true;

                    // 如果只是暂停恢复只要重新
                    if (this.IsFullScreen && this._isPaused)
                    {
                        this._isPaused = false;

                        if (this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode && this._isPausedRecheck)
                        {
                            this.IsAuthVisible = false;
                            _needReload = false;
                        }

                        else if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode && !this._isPausedRecheck)
                        {
                            this.IsAuthVisible = false;
                            _needReload = false;
                        }

                        // 暂停模式切换 Recheck-FQA或FQA-Recheck强制重载
                        else
                        {
                            _needReload = true;
                        }
                    }


                    if (this.IsFullScreen && _needReload)
                    {
                        this.IsAuthVisible = true;

                        // 登陆之后清空用户名和密码
                        this._authBoxVm.UserName = string.Empty;
                        this._authBoxVm.UserPass = string.Empty;

                        page.autoBox.passWord.Clear();

                        if (this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode)
                        {
                            this._authBoxVm.NeedLocalLogin = false;
                        }
                        else
                        {
                            this._authBoxVm.NeedLocalLogin = true;
                        }
                    }

                    // Call 外部逻辑以便本页面显示最前端
                    if (null != FullScreenToggleHandlers)
                    {
                        try
                        {
                            this.FullScreenToggleHandlers(this._isFullScreen);
                        }
                        catch { }
                    }

                    if (this.IsFullScreen && null != page)
                    {
                        page.Focusable = true;
                        Keyboard.Focus(page);
                    }

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        /// <summary>
        /// 更新显示图片
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
                        this.MarkNG.Execute(false);     //图片不存在时，该电池判为NG
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

        public ICommand NavPrev
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    this._manualRecheckLogic.NavPrev();
                    this.UpdateImage();
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand NavNext
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    this._manualRecheckLogic.NavNext();
                    this.UpdateImage();
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand MarkOK
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object isShortCut)
                {
                    if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode && isShortCut.ToString() == "true")
                    {
                        System.Windows.MessageBox.Show("FQA状态下标记快捷键无效，请使用鼠标点击！");

                        return;
                    }

                    this._manualRecheckLogic.MarkOK();

                    this._manualRecheckLogic.NavNext();
                    this.UpdateImage();

                }), delegate
                {
                    return /*this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode &&*/ !this.IsBusy;
                });
            }
        }

        public ICommand MarkNG
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object isShortCut)
                {
                    if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode && isShortCut.ToString() == "true")
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
                }), delegate
                {
                    return /*this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode &&*/ !this.IsBusy;
                });
            }
        }


        /// <summary>
        /// 人工复判为再判
        /// </summary>
        public ICommand WaitCheck
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object isShortCut)
                {
                    if (!this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode && isShortCut.ToString() == "true")
                    {
                        System.Windows.MessageBox.Show("FQA状态下标记快捷键无效，请使用鼠标点击！");

                        return;
                    }

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
                }), delegate
                {
                    return /*this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode &&*/ !this.IsBusy;
                });
            }
        }
        /// <summary>
        /// 人工复判提交结果按钮
        /// </summary>
        public ICommand Submit
        {
            get
            {
                return new RelayCommand(new Action(() =>
                {
                    this.IsBusy = true;

                    this.SubmitAsync();

                    this.IsBusy = false;

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        private async void SubmitAsync()
        {
            int subCnt = 0;
            int unSubCnt = 0;
            bool result = false;
            string failReason = string.Empty;

            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
                    this._manualRecheckLogic.SubmitRecheckResult(ref subCnt, ref unSubCnt, this._authBoxVm.UserName);
                    this.MyManualRecheck.GetReady(this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode, this._authBoxVm.IsNotWaitCheck);
                    result = true;
                }
                catch (System.Exception ex)
                {
                    LoggingIF.Log("Failed to submit recheck result: " + ex.Message.ToString(), LogLevels.Error, "Recheck");
                    result = false;
                    failReason = ex.Message.ToString();
                }
            });

            if (result)
            {
                System.Windows.MessageBox.Show("已完成\r\r已处理数目:" + subCnt.ToString() + Environment.NewLine + "未处理数目:" + unSubCnt.ToString());
            }
            else
            {
                System.Windows.MessageBox.Show("错误\r\n" + failReason);
            }
        }

        /// <summary>
        /// FQA确认（FQA提交按钮）
        /// </summary>
        public ICommand FQASubmit
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    this.IsBusy = true;

                    this.SubmitFQAAsync();

                    this.IsBusy = false;
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        private async void SubmitFQAAsync()
        {
            bool result = false;
            string failReason = string.Empty;

            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
                    //Thread.Sleep(5000);
                    this._manualRecheckLogic.SubmitFQAResult(this._authBoxVm.UserName);
                    this.MyManualRecheck.GetReady(this._manualRecheckLogic.MyRecheckStatus.IsRecheckMode, this._authBoxVm.IsNotWaitCheck);

                    result = true;
                }
                catch (System.Exception ex)
                {
                    LoggingIF.Log("Failed to submit FQA result: " + ex.Message.ToString(), LogLevels.Error, "Recheck");
                    result = false;
                    failReason = ex.Message.ToString();
                }
            });

            if (result)
            {
                System.Windows.MessageBox.Show("已完成\r\n文件拷贝完毕，请记得上传数据！");
            }
            else
            {
                System.Windows.MessageBox.Show("错误\r\n" + failReason);
            }
        }

        /// <summary>
        /// FQA 提交结果(FQA上传按钮)
        /// </summary>
        public ICommand FQAUpload
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    System.Windows.MessageBox.Show("点击确定后，开始上传，请耐心等待上传完成的提示\r\n" );

                    LoggingIF.Log("FQA Upload, User:  " + _currentuserid, LogLevels.Info, "Recheck");

                    this.IsBusy = true;

                    this.UploadResult();

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        private async void UploadResult()
        {

            await Task.Run(() => {
                try
                {
                    BatteryCheckIF.RefreshFQAUploadList();    //获取FQA判定OK的列表（RecheckState=4）
                    BatteryCheckIF.RefreshFQAUploadNGList();  //获取人工判定NG的列表（RecheckState=2）
                    BatteryCheckIF.RefreshRecheckProductDataList();
                }
                catch { }
            });

            await Task.Delay(1000);

            if (BatteryCheckIF.MyRecheckRecordManager.FQANotUpload.Count() < 1 && BatteryCheckIF.MyRecheckRecordManager.FQANotUploadNG.Count() < 1)
            {
                this.IsBusy = false;
                return;
            }

            int okCnt = 0;
            int failCnt = 0;
            bool result = false;
            string failReason = string.Empty;
            await Task.Run(() => {
                try
                {
                    this._manualRecheckLogic.UploadFQAResult("FQA", "","0000123456", ref okCnt, ref failCnt, ref failReason);
                    result = true;
                }
                catch (System.Exception ex)
                {
                    failReason = ex.Message.ToString();
                    result = false;
                }
            });


            if (result)
            {
                System.Windows.MessageBox.Show("已完成\r\n上传成功数:" + okCnt.ToString() + Environment.NewLine + "上传失败数:" + failCnt.ToString() + Environment.NewLine + failReason);
            }
            else
            {
                System.Windows.MessageBox.Show("错误\r\n" + failReason);
            }

            this.IsBusy = false;
        }


    }
}
