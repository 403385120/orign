using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
    public abstract class EsqDbBusinessEntity
    {
        private static int cacheAbsoluteExpiration = 1800000;

        private static int cacheExpirationMinute = 30;

        protected string saveTable = string.Empty;

        protected string selectTable = string.Empty;

        protected string backupTable = string.Empty;

        protected string systemCode = string.Empty;

        protected string _tableKeyField = string.Empty;

        private bool _isSucees = false;

        private decimal _totalCount = default(decimal);

        public static int CacheAbsoluteExpiration
        {
            get
            {
                return cacheAbsoluteExpiration;
            }
            set
            {
                cacheAbsoluteExpiration = value;
            }
        }

        public static int CacheExpirationMinute
        {
            get
            {
                return cacheExpirationMinute;
            }
            set
            {
                cacheExpirationMinute = value;
            }
        }

        public string SaveTable
        {
            get
            {
                return saveTable;
            }
            set
            {
                saveTable = value;
            }
        }

        public string SelectTable
        {
            get
            {
                return selectTable;
            }
            set
            {
                selectTable = value;
            }
        }

        public string BackupTable
        {
            get
            {
                return backupTable;
            }
            set
            {
                backupTable = value;
            }
        }

        public string SystemCode
        {
            get
            {
                return systemCode;
            }
            set
            {
                systemCode = value;
            }
        }

        public string TableKeyField
        {
            get
            {
                return _tableKeyField;
            }
            set
            {
                _tableKeyField = value;
            }
        }

        public string ErrorMsg
        {
            get;
            set;
        }

        public bool IsSuccess
        {
            get
            {
                return _isSucees;
            }
            set
            {
                _isSucees = value;
            }
        }

        public decimal TotalCount
        {
            get
            {
                return _totalCount;
            }
            set
            {
                _totalCount = value;
            }
        }

        public EsqDbBusinessEntity()
        {
        }

        public object GetValue(string propertyName)
        {
            return GetType().GetProperty(propertyName).GetValue(this, null);
        }
    }
}
