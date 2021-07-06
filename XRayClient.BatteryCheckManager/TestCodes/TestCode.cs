using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.BatteryCheckManager
{
    /// <summary>
    /// 点检料条码表
    /// </summary>
    public class TestCode
    {
        private int _recordID = -1;                 // 记录ID
        private string _barCode = string.Empty;     // 条码
        private DateTime _createTime = DateTime.Now;// 创建时间
        private string _createdBy = string.Empty;   // 创建人
        private string _remarks = string.Empty;     // 备注

        public int RecordID
        {
            get { return this._recordID; }
            set
            {
                this._recordID = value;
            }
        }

        public string BarCode
        {
            get { return this._barCode; }
            set
            {
                this._barCode = value;
            }
        }

        public DateTime CreateTime
        {
            get { return this._createTime; }
            set
            {
                this._createTime = value;
            }
        }

        public string CreateBy
        {
            get { return this._createdBy; }
            set
            {
                this._createdBy = value;
            }
        }

        public string Remarks
        {
            get { return this._remarks; }
            set
            {
                this._remarks = value;
            }
        }

    }
}
