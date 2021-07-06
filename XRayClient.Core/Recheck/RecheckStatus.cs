using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.Core
{
    public class RecheckStatus : ObservableObject
    {
        private DateTime _startTime = DateTime.Now;
        private bool _isRecheckMode = true;     // 状态模式(true: 复检 false: FQA)
        private int _totalNum = 0;              // 总数
        private int _checkedNum = 0;            // 已判定数
        private int _waitCheckNum = 0;          // 再判定数
        private int _curIndex = 0;              // 图片索引
        private int _subIndex = 0;              // 图片子索引
        private string _curBarCode = string.Empty;  // 当前条码

        public bool IsRecheckMode
        {
            get { return this._isRecheckMode; }
            set
            {
                this._isRecheckMode = value;
                RaisePropertyChanged("IsRecheckMode");
            }
        }

        public string CurBarCode
        {
            get { return this._curBarCode; }
            set
            {
                this._curBarCode = value;
                RaisePropertyChanged("CurBarCode");
            }
        }

        public DateTime StartTime
        {
            get { return this._startTime; }
            set
            {
                this._startTime = value;
                RaisePropertyChanged("StartTime");
            }
        }

        public int TotalNum
        {
            get { return this._totalNum; }
            set
            {
                this._totalNum = value;
                RaisePropertyChanged("TotalNum");
            }
        }

        public int CheckedNum
        {
            get { return this._checkedNum; }
            set
            { 
                this._checkedNum = value;
                RaisePropertyChanged("CheckedNum");
            }
        }

        public int WaitCheckNum
        {
            get { return this._waitCheckNum; }
            set
            {
                this._waitCheckNum = value;
                RaisePropertyChanged("WaitCheckNum");
            }
        }

        public int CurIndex
        {
            get { return this._curIndex; }
            set
            {
                this._curIndex = value;
                RaisePropertyChanged("CurIndex");
            }
        }

        public int SubIndex
        {
            get { return this._subIndex; }
            set
            {
                this._subIndex = value;
                RaisePropertyChanged("SubIndex");
            }
        }

        public void NavPrev()
        {
            if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
            {
                if (this.CurIndex == 0)
                {
                    return;
                }
                this.CurIndex--;
            }
            else   //默认4角模式
            {
                if (this.SubIndex > 0)
                {
                    this.SubIndex = 0;
                    return;
                }

                if (this.SubIndex == 0)
                {
                    if (this.CurIndex == 0)
                    {
                        return;
                    }

                    this.CurIndex--;
                    this.SubIndex = 1;
                }
            }
        }

        public void NavNext()
        {
            if (CheckParamsConfig.Instance.CheckMode == ECheckModes.Diagonal_1_2)
            {
                if (this.CurIndex >= this.TotalNum - 1)
                {
                    return;
                }
                this.CurIndex++;
            }
            else  //默认4角模式
            {
                if (this.SubIndex == 0)
                {
                    this.SubIndex = 1;
                    return;
                }

                if (this.SubIndex > 0)
                {
                    if (this.CurIndex >= this.TotalNum - 1)
                    {
                        return;
                    }
                    
                    this.CurIndex++;
                    this.SubIndex = 0;
                }
            }
        }

        public void WaitCheck()
        {
            this.WaitCheckNum++;
        }

       
    }
}
