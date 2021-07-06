using System;
using System.Collections.Generic;
using Shuyz.Framework.Mvvm;
using System.Threading;
using ZY.Logging;
using System.Windows.Input;
using System.Windows.Forms;
using XRayClient.Core;

namespace ZYXray.ViewModels
{
    public class InspectTestPageVm : ObservableObject
    {
        private bool _isBusy = false;

        private bool _autoMode = false;
        private string _autoPath = string.Empty;
        private string _leftUpImg = string.Empty;
        private string _leftDownImg = string.Empty;
        private string _rightUpImg = string.Empty;
        private string _rightDownImg = string.Empty;

        private float _leftUpTime = 0f;
        private float _leftDownTime = 0f;
        private float _rightUpTime = 0f;
        private float _rightDownTime = 0f;

        private int _leftUpRet = 0;
        private int _leftDownRet = 0;
        private int _rightUpRet = 0;
        private int _rightDownRet = 0;

        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {

                if (value == this._isBusy) return;

                this._isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        public bool AutoMode
        {
            get { return this._autoMode; }
            set
            {
                this._autoMode = value;
                RaisePropertyChanged("AutoMode");
            }
        }

        public string AutoPath
        {
            get { return this._autoPath; }
            set
            {
                this._autoPath = value;
                RaisePropertyChanged("AutoPath");
            }
        }

        public string LeftUpImg
        {
            get { return this._leftUpImg; }
            set
            {
                this._leftUpImg = value;
                RaisePropertyChanged("LeftUpImg");
            }
        }


        public string LeftDownImg
        {
            get { return this._leftDownImg; }
            set
            {
                this._leftDownImg = value;
                RaisePropertyChanged("LeftDownImg");
            }
        }

        public string RightDownImg
        {
            get { return this._rightDownImg; }
            set
            {
                this._rightDownImg = value;
                RaisePropertyChanged("RightDownImg");
            }
        }

        public string RightUpImg
        {
            get { return this._rightUpImg; }
            set
            {
                this._rightUpImg = value;
                RaisePropertyChanged("RightUpImg");
            }
        }

        public float LeftUpTime
        {
            get { return this._leftUpTime; }
            set
            {
                this._leftUpTime = value;
                RaisePropertyChanged("LeftUpTime");
            }
        }

        public float LeftDownTime
        {
            get { return this._leftDownTime; }
            set
            {
                this._leftDownTime = value;
                RaisePropertyChanged("LeftDownTime");
            }
        }

        public float RightUpTime
        {
            get { return this._rightUpTime; }
            set
            {
                this._rightUpTime = value;
                RaisePropertyChanged("RightUpTime");
            }
        }

        public float RightDownTime
        {
            get { return this._rightDownTime; }
            set
            {
                this._rightDownTime = value;
                RaisePropertyChanged("RightDownTime");
            }
        }


        public int LeftUpRet
        {
            get { return this._leftUpRet; }
            set
            {
                this._leftUpRet = value;
                RaisePropertyChanged("LeftUpRet");
            }
        }

        public int LeftDownRet
        {
            get { return this._leftDownRet; }
            set
            {
                this._leftDownRet = value;
                RaisePropertyChanged("LeftDownRet");
            }
        }

        public int RightUpRet
        {
            get { return this._rightUpRet; }
            set
            {
                this._rightUpRet = value;
                RaisePropertyChanged("RightUpRet");
            }
        }

        public int RightDownRet
        {
            get { return this._rightDownRet; }
            set
            {
                this._rightDownRet = value;
                RaisePropertyChanged("RightDownRet");
            }
        }

        public ICommand DoTest
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        this.IsBusy = true;

                        try
                        {
                            string mLeftUp = string.Empty;
                            string mLeftDown = string.Empty;
                            string mRightDown = string.Empty;
                            string mRightUp = string.Empty;
                            List<float> timeList = new List<float>();

                            if (!AutoMode)
                            {
                                mLeftUp = this.LeftUpImg;
                                mLeftDown = this.LeftDownImg;
                                mRightDown = this.RightDownImg;
                                mRightUp = this.RightUpImg;
                            }

                            BotIF.InspectTest(this.AutoPath, ref mLeftUp, ref mLeftDown, ref mRightUp, ref mRightDown, this.AutoMode, ref timeList);

                            if (this.AutoMode)
                            {
                                this.LeftUpImg = mLeftUp;
                                this.LeftDownImg = mLeftDown;
                                this.RightDownImg = mRightDown;
                                this.RightUpImg = mRightUp;
                            }

                            switch (CheckParamsConfig.Instance.CheckMode)    //recompose by fjy
                            {
                                case ECheckModes.FourSides:
                                    if (timeList.Count == 8)
                                    {
                                        this.LeftUpTime = timeList[0];
                                        this.LeftDownTime = timeList[1];
                                        this.RightDownTime = timeList[2];
                                        this.RightUpTime = timeList[3];

                                        this.LeftUpRet = (int)timeList[4];
                                        this.LeftDownRet = (int)timeList[5];
                                        this.RightDownRet = (int)timeList[6];
                                        this.RightUpRet = (int)timeList[7];
                                    }
                                    break;
                                case ECheckModes.Diagonal_1_2:
                                    if (timeList.Count == 4)
                                    {
                                        this.LeftUpTime = timeList[0];
                                        this.RightDownTime = timeList[1];

                                        this.LeftUpRet = (int)timeList[2];
                                        this.RightDownRet = (int)timeList[3];
                                    }
                                    break;
                                default:break;
                            }
                            
                        }
                        catch (System.Exception ex)
                        {
                            LoggingIF.Log(string.Format("Failed to test algo {0}", ex.Message.ToString()), LogLevels.Error, "ProductionDataPage");
                        }

                        this.IsBusy = false;
                    }).Start();
                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand SelectFile
        {
            get
            {
                return new RelayCommand<object>(new Action<object>(delegate (object index)
                {
                    int idx = int.Parse(index.ToString());

                    using (var dialog = new OpenFileDialog())
                    {
                        dialog.Filter = "Image Files |*.bmp;*.jpg";
                        dialog.FilterIndex = 1;
                        dialog.DefaultExt = "bmp";
                        DialogResult result = dialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            if (idx == 1) this.LeftUpImg = dialog.FileName;
                            else if (idx == 2) this.LeftDownImg = dialog.FileName;
                            else if (idx == 3) this.RightDownImg = dialog.FileName;
                            else this.RightUpImg = dialog.FileName;
                        }
                    }

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

        public ICommand SelectFolder
        {
            get
            {
                return new RelayCommand(new Action(delegate
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            this.AutoPath = dialog.SelectedPath;
                        }
                    }

                }), delegate
                {
                    return !this.IsBusy;
                });
            }
        }

    }
}
