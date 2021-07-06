using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.BatteryCheckManager
{
    public class RecheckRecord : ObservableObject
    {
        private int _recordID = -1;
        private string _batteryBarCode = string.Empty;
        private int _finalResult = 0;
        private string _resultPath = string.Empty;
        private string _stfPath = string.Empty; // 路径only，用于复判重传
        private string _mesImagePath = string.Empty;

        private string _a1_OriginalImagePath = string.Empty;
        private string _a2_OriginalImagePath = string.Empty;
        private string _a3_OriginalImagePath = string.Empty;
        private string _a4_OriginalImagePath = string.Empty;

        private string _a1_ResultImagePath = string.Empty;
        private string _a2_ResultImagePath = string.Empty;
        private string _a3_ResultImagePath = string.Empty;
        private string _a4_ResultImagePath = string.Empty;

        private int _recheckState = 0;
        private DateTime _recheckTime = DateTime.Now;
        private string _recheckUserID = string.Empty;
        private DateTime _fqaTime = DateTime.Now;
        private string _fqaUser = string.Empty;

        private bool _frontResult = false;  // 第一组检测结果
        private bool _backResult = false;   // 第二组检测结果
        private bool _isChecked = false;    // 是否检测标志
        private bool _isfrontChecked = false;    // 第一组是否检测标志       add by fjy
        private bool _isbackChecked = false;     // 第二组是否检测标志       add by fjy
        private bool _isWaitCheck = false;       // 

        private int _checkMode = 0;

        /// <summary>
        /// 数据库记录ID
        /// </summary>
        public int RecordID
        {
            get { return this._recordID; }
            set
            {
                this._recordID = value;
            }
        }

        /// <summary>
        /// 扫码电池二维码
        /// </summary>
        public string BatteryBarCode
        {
            get { return this._batteryBarCode; }
            set
            {
                this._batteryBarCode = value;
                //RaisePropertyChanged("BatteryBarCode");
            }
        }

        public string StfPath
        {
            get { return this._stfPath; }
            set
            {
                this._stfPath = value;
            }
        }

        public string MesImagePath
        {
            get { return this._mesImagePath; }
            set
            {
                this._mesImagePath = value;
            }
        }
        /// <summary>
        /// 最终检测结果
        /// 这里存的是bool值，每个corner的result是int值
        /// </summary>
        public int FinalResult
        {
            get { return this._finalResult; }
            set
            {
                this._finalResult = value;
            }
        }

        public string ResultPath
        {
            get { return this._resultPath; }
            set
            {
                this._resultPath = value;
            }
        }

        /// <summary>
        /// 左上原始图Path
        /// </summary>
        public string A1_OriginalImagePath
        {
            get { return this._a1_OriginalImagePath; }
            set
            {
                this._a1_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// 右下原始图Path
        /// </summary>
        public string A2_OriginalImagePath
        {
            get { return this._a2_OriginalImagePath; }
            set
            {
                this._a2_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// 右下原始图Path
        /// </summary>
        public string A3_OriginalImagePath
        {
            get { return this._a3_OriginalImagePath; }
            set
            {
                this._a3_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// 右下原始图Path
        /// </summary>
        public string A4_OriginalImagePath
        {
            get { return this._a4_OriginalImagePath; }
            set
            {
                this._a4_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// 左上结果图Path
        /// </summary>
        public string A1_ResultImagePath
        {
            get { return this._a1_ResultImagePath; }
            set
            {
                this._a1_ResultImagePath = value;
            }
        }

        /// <summary>
        /// 右下结果图Path
        /// </summary>
        public string A2_ResultImagePath
        {
            get { return this._a2_ResultImagePath; }
            set
            {
                this._a2_ResultImagePath = value;
            }
        }

        /// <summary>
        /// 右下结果图Path
        /// </summary>
        public string A3_ResultImagePath
        {
            get { return this._a3_ResultImagePath; }
            set
            {
                this._a3_ResultImagePath = value;
            }
        }

        /// <summary>
        /// 右下结果图Path
        /// </summary>
        public string A4_ResultImagePath
        {
            get { return this._a4_ResultImagePath; }
            set
            {
                this._a4_ResultImagePath = value;
            }
        }

        /// <summary>
        /// 复检结果
        /// </summary>
        public int RecheckState
        {
            get { return this._recheckState; }
            set
            {
                this._recheckState = value;
            }
        }

        /// <summary>
        /// 复检时间
        /// </summary>
        public DateTime RecheckTime
        {
            get { return this._recheckTime; }
            set
            {
                this._recheckTime = value;
            }
        }

        /// <summary>
        /// 复检人员
        /// </summary>
        public string RecheckUserID
        {
            get { return this._recheckUserID; }
            set
            {
                this._recheckUserID = value;
            }
        }

        /// <summary>
        /// 复检时间
        /// </summary>
        public DateTime FQATime
        {
            get { return this._fqaTime; }
            set
            {
                this._fqaTime = value;
            }
        }

        /// <summary>
        /// FQA人员
        /// </summary>
        public string FQAUser
        {
            get { return this._fqaUser; }
            set
            {
                this._fqaUser = value;
            }
        }

        public bool FrontResult
        {
            get { return this._frontResult; }
            set
            {
                this._isChecked = true;
                this.IsfrontChecked = true;    //add by fjy

                this._frontResult = value;
                RaisePropertyChanged("FrontResult");
            }
        }

        public bool BackResult
        {
            get { return this._backResult; }
            set
            {
                this._isChecked = true;
                this.IsbackChecked = true;    //add by fjy

                this._backResult = value;
                RaisePropertyChanged("BackResult");
            }
        }

        public bool IsChecked
        {
            get { return this._isChecked; }
            set
            {
                this._isChecked = value;
            }
        }

        public bool IsWaitCheck
        {
            get { return this._isWaitCheck; }
            set
            {
                this._isWaitCheck = value;
            }
        }

        public bool IsfrontChecked
        {
            get { return this._isfrontChecked; }
            set
            {
                this._isfrontChecked = value;
            }
        }

        public bool IsbackChecked
        {
            get { return this._isbackChecked; }
            set
            {
                this._isbackChecked = value;
            }
        }

        public int CheckMode
        {
            get { return this._checkMode; }
            set
            {
                this._checkMode = value;
            }
        }
    }
}
