using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.BatteryCheckManager
{
    public class ProductionDataXray
    {
        private int _recordID = -1; 
        private DateTime _logDateTime = DateTime.Now;
        private string _productSN = string.Empty;
        private string _jsonData = string.Empty;
        private string _keyValue = string.Empty;


        public int RecordID
        {
            get { return this._recordID; }
            set
            {
                this._recordID = value;
            }
        }

        public DateTime LogDateTime
        {
            get { return this._logDateTime; }
            set
            {
                this._logDateTime = value;
            }
        }

        public string ProductSN
        {
            get { return this._productSN; }
            set
            {
                this._productSN = value;
            }
        }

        public string JsonData
        {
            get { return this._jsonData; }
            set
            {
                this._jsonData = value;
            }
        }

        public string KeyValue
        {
            get { return this._keyValue; }
            set
            {
                this._keyValue = value;
            }
        }
    }
}
