using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace ZYXray.ViewModels
{
    class TestStationInfoVm : ObservableObject
    {
        private static TestStationInfoVm _instance = new TestStationInfoVm();
        public static TestStationInfoVm Instance
        {
            get { return _instance; }
        }

        string scan_barcode = string.Empty;
        string iv_barcode = string.Empty;
        string xray_barcode = string.Empty;
        string vol_barcode = string.Empty;
        string resis_barcode = string.Empty;
        string temp_barcode = string.Empty;
        string thick_barcode = string.Empty;
        string dime_barcode = string.Empty;
        string sort_barcode = string.Empty;

        string scan_value = string.Empty;
        string iv_value = string.Empty;
        string xray_value = string.Empty;
        string vol_value = string.Empty;
        string resis_value = string.Empty;
        string temp_value = string.Empty;
        string thick_value = string.Empty;
        string dime_value = string.Empty;
        string sort_value = string.Empty;

        string scan_result = string.Empty;
        string iv_result = string.Empty;
        string xray_result = string.Empty;
        string vol_result = string.Empty;
        string resis_result = string.Empty;
        string temp_result = string.Empty;
        string thick_result = string.Empty;
        string dime_result = string.Empty;
        string sort_result = string.Empty;
        public string ScanBarCode
        {
            get { return this.scan_barcode; }
            set
            {
                this.scan_barcode = value;
                RaisePropertyChanged("ScanBarCode");
            }
        }
        public string XRayBarCode
        {
            get { return this.xray_barcode; }
            set
            {
                this.xray_barcode = value;
                RaisePropertyChanged("XRayBarCode");
            }
        }
        public string IVBarCode
        {
            get { return this.iv_barcode; }
            set
            {
                this.iv_barcode = value;
                RaisePropertyChanged("IVBarCode");
            }
        }
        public string VolBarCode
        {
            get { return this.vol_barcode; }
            set
            {
                this.vol_barcode = value;
                RaisePropertyChanged("VolBarCode");
            }
        }
        public string ResisBarCode
        {
            get { return this.resis_barcode; }
            set
            {
                this.resis_barcode = value;
                RaisePropertyChanged("ResisBarCode");
            }
        }
        public string TempBarCode
        {
            get { return this.temp_barcode; }
            set
            {
                this.temp_barcode = value;
                RaisePropertyChanged("TempBarCode");
            }
        }
        public string ThickBarCode
        {
            get { return this.thick_barcode; }
            set
            {
                this.thick_barcode = value;
                RaisePropertyChanged("ThickBarCode");
            }
        }
        public string DimeBarCode
        {
            get { return this.dime_barcode; }
            set
            {
                this.dime_barcode = value;
                RaisePropertyChanged("DimeBarCode");
            }
        }
        public string SortBarCode
        {
            get { return this.sort_barcode; }
            set
            {
                this.sort_barcode = value;
                RaisePropertyChanged("SortBarCode");
            }
        }

        public string ScanValue
        {
            get { return this.scan_value; }
            set
            {
                this.scan_value = value;
                RaisePropertyChanged("ScanValue");
            }
        }
        public string XRayValue
        {
            get { return this.xray_value; }
            set
            {
                this.xray_value = value;
                RaisePropertyChanged("XRayValue");
            }
        }
        public string IVValue
        {
            get { return this.iv_value; }
            set
            {
                this.iv_value = value;
                RaisePropertyChanged("IVValue");
            }
        }
        public string VolValue
        {
            get { return this.vol_value; }
            set
            {
                this.vol_value = value;
                RaisePropertyChanged("VolValue");
            }
        }
        public string ResisValue
        {
            get { return this.resis_value; }
            set
            {
                this.resis_value = value;
                RaisePropertyChanged("ResisValue");
            }
        }
        public string TempValue
        {
            get { return this.temp_value; }
            set
            {
                this.temp_value = value;
                RaisePropertyChanged("TempValue");
            }
        }
        public string ThickValue
        {
            get { return this.thick_value; }
            set
            {
                this.thick_value = value;
                RaisePropertyChanged("ThickValue");
            }
        }
        public string DimeValue
        {
            get { return this.dime_value; }
            set
            {
                this.dime_value = value;
                RaisePropertyChanged("DimeValue");
            }
        }
        public string SortValue
        {
            get { return this.sort_value; }
            set
            {
                this.sort_value = value;
                RaisePropertyChanged("SortValue");
            }
        }

        public string ScanResult
        {
            get { return this.scan_result; }
            set
            {
                this.scan_result = value;
                RaisePropertyChanged("ScanResult");
            }
        }
        public string XRayResult
        {
            get { return this.xray_result; }
            set
            {
                this.xray_result = value;
                RaisePropertyChanged("XRayResult");
            }
        }
        public string IVResult
        {
            get { return this.iv_result; }
            set
            {
                this.iv_result = value;
                RaisePropertyChanged("IVResult");
            }
        }
        public string VolResult
        {
            get { return this.vol_result; }
            set
            {
                this.vol_result = value;
                RaisePropertyChanged("VolResult");
            }
        }
        public string ResisResult
        {
            get { return this.resis_result; }
            set
            {
                this.resis_result = value;
                RaisePropertyChanged("ResisResult");
            }
        }
        public string TempResult
        {
            get { return this.temp_result; }
            set
            {
                this.temp_result = value;
                RaisePropertyChanged("TempResult");
            }
        }
        public string ThickResult
        {
            get { return this.thick_result; }
            set
            {
                this.thick_result = value;
                RaisePropertyChanged("ThickResult");
            }
        }
        public string DimeResult
        {
            get { return this.dime_result; }
            set
            {
                this.dime_result = value;
                RaisePropertyChanged("DimeResult");
            }
        }
        public string SortResult
        {
            get { return this.sort_result; }
            set
            {
                this.sort_result = value;
                RaisePropertyChanged("SortResult");
            }
        }
    }
}
