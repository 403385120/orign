using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using System.Runtime.InteropServices;
using System.IO;

namespace XRayClient.Core
{
    /// <summary>
    /// 数量统计
    /// 每次班次开始的时候用户手动清空数据
    /// </summary>
    public class Statistics : ObservableObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                         string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);

        private readonly string _configFile = Path.Combine(Environment.CurrentDirectory, "CheckStatistics.ini");

        private int _totalNum = 0;
        private int _okNum = 0;
        private int _ngNum = 0;         // 算法NG
        private int _scanNG = 0;        // 扫码失败
        private int _otherNG = 0;       // 黑图、STF错误
        private int _mesNG = 0;
        private int _thincknessNG = 0;
        private int _dimensionNG = 0;
        private int _oCVNG = 0;
        private int _iVNG = 0;
        private int _wrinkleNG = 0;
        private int _distanceNG = 0;
        private int _angleNG = 0;
        private int _xRayNG=0;
        private float _ppm = 0;
        private DateTime _startTime = DateTime.Now; // 统计开始时间
        private int _scanTotal = 0;        // 扫码失败

        public int TotalNum
        {
            get { return this._totalNum; }
            set
            {
                this._totalNum = value;
                RaisePropertyChanged("TotalNum");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
                RaisePropertyChanged("PPM");
            }
        }

        public int OKNum
        {
            get { return this._okNum; }
            set
            {
                this._okNum = value;
                RaisePropertyChanged("OKNum");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int NGNum
        {
            get { return this._ngNum; }
            set
            {
                this._ngNum = value;
                RaisePropertyChanged("NGNum");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int ScanNG
        {
            get { return this._scanNG; }
            set
            {
                this._scanNG = value;
                RaisePropertyChanged("ScanNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int ScanTotal
        {
            get { return this._scanTotal; }
            set
            {
                this._scanTotal = value;
                RaisePropertyChanged("ScanTotal");
                RaisePropertyChanged("ScanPassRate");
            }
        }

        public int MesNG
        {
            get { return this._mesNG; }
            set
            {
                this._mesNG = value;
                RaisePropertyChanged("MesNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int ThincknessNG
        {
            get { return this._thincknessNG; }
            set
            {
                this._thincknessNG = value;
                RaisePropertyChanged("ThincknessNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int DimensionNG
        {
            get { return this._dimensionNG; }
            set
            {
                this._dimensionNG = value;
                RaisePropertyChanged("DimensionNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int OCVNG
        {
            get { return this._oCVNG; }
            set
            {
                this._oCVNG = value;
                RaisePropertyChanged("OCVNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int IVNG
        {
            get { return this._iVNG; }
            set
            {
                this._iVNG = value;
                RaisePropertyChanged("IVNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        /// <summary>
        /// 褶皱NG
        /// </summary>
        public int WrinkleNG
        {
            get { return this._wrinkleNG; }
            set
            {
                this._wrinkleNG = value;
                RaisePropertyChanged("WrinkleNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int DistanceNG
        {
            get { return this._distanceNG; }
            set
            {
                this._distanceNG = value;
                RaisePropertyChanged("DistanceNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int XRayNG
        {
            get { return this._xRayNG; }
            set
            {
                this._xRayNG = value;
                RaisePropertyChanged("XRayNG");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int AngleNG
        {
            get { return this._angleNG; }
            set
            {
                this._angleNG = value;
                RaisePropertyChanged("AngleNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
            }
        }

        public int OtherNG
        {
            get { return this._otherNG; }
            set
            {
                this._otherNG = value;
                RaisePropertyChanged("OtherNG");
                RaisePropertyChanged("PassRate");
                RaisePropertyChanged("XRayPassRate");
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

        public float PassRate
        {
            get { return this._totalNum > 0 ? 100 * (float)this._okNum / (float)this._totalNum : 0; }
        }

        public float XRayPassRate
        {
            //get { return this._totalNum > 0 ? 100 * (1 - ((float)this._ngNum / ((float)this._totalNum - (float)this._scanNG - (float)this._thincknessNG - (float)this._otherNG - (float)this._mesNG - (float)this._dimensionNG - this._oCVNG) - this.IVNG)) : 0; }
            get { return this._totalNum > 0 ? 100 * (1 - ((float)this.XRayNG / ((float)this._totalNum))) : 0; }
        }

        public float ScanPassRate
        {
            get { return this._scanTotal > 0 ? 100 * (1 - (float)this._scanNG / (float)this._scanTotal) : 0; }
        }

        public float PPM
        {
            get { return this._ppm; }
            set
            {
                this._ppm = value;
                RaisePropertyChanged("PPM");
            }
        }

        public Statistics()
        {
            if (!File.Exists(this._configFile))
            {
                this.Save();
            }

            this.Read();
        }

        ~Statistics()
        {
            this.Save();
        }

        public void Save()
        {
            WritePrivateProfileString("Statistics", "TotalNum", this.TotalNum.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "OKNum", this.OKNum.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "NGNum", this.NGNum.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "ScanNG", this.ScanNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "ScanTotal", this.ScanTotal.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "ThincknessNG", this.ThincknessNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "DimensionNG", this.DimensionNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "OCVNG", this.OCVNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "IVNG", this.IVNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "MesNG", this.MesNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "WrinkleNG", this.WrinkleNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "DistanceNG", this.DistanceNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "AngleNG", this.AngleNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "XRayNG", this.XRayNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "OtherNG", this.OtherNG.ToString(), this._configFile);
            WritePrivateProfileString("Statistics", "StartTime", this.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), this._configFile);
            WritePrivateProfileString("Statistics", "PPM", this.PPM.ToString(), this._configFile);
        }

        public void UpdateView(BatterySeat bt)
        {
            this.TotalNum++;
            if ((!bt.Sn.Contains("ERROR") && bt.Sn != String.Empty && bt.ThicknessResult && bt.DimensionResult && bt.OCVResult && bt.IVResult && bt.MesResult && bt.ResultCode == EResultCodes.FoundOK))
            {
                this.OKNum++;
            }
            if (bt.Sn.Contains("ERROR") || bt.Sn == String.Empty)
            {
                //this.ScanNG++;
            }
            if (bt.ThicknessResult == false)
            {
                this.ThincknessNG++;
            }
            if (bt.DimensionResult == false)
            {
                this.DimensionNG++;
            }
            if (bt.OCVResult == false)
            {
                this.OCVNG++;
            }
            if (bt.IVResult == false)
            {
                this.IVNG++;
            }
            if (bt.MesResult == false)
            {
                this.MesNG++;
            }
            if (bt.ResultCode == EResultCodes.AlgoFail)
            {
                this.NGNum++;
                if (bt.Corner1.InspectResults.resultDataMin.iResult == -5 ||
                    bt.Corner2.InspectResults.resultDataMin.iResult == -5 ||
                    bt.Corner3.InspectResults.resultDataMin.iResult == -5 ||
                    bt.Corner4.InspectResults.resultDataMin.iResult == -5)
                {
                    this.WrinkleNG++;
                }
                if (bt.Corner1.InspectResults.resultDataMin.iResult == -7 ||
                    bt.Corner2.InspectResults.resultDataMin.iResult == -7 ||
                    bt.Corner3.InspectResults.resultDataMin.iResult == -7 ||
                    bt.Corner4.InspectResults.resultDataMin.iResult == -7)
                {
                    this.DistanceNG++;
                }
                if (bt.Corner1.InspectResults.resultDataMin.iResult == -8 ||
                    bt.Corner2.InspectResults.resultDataMin.iResult == -8 ||
                    bt.Corner3.InspectResults.resultDataMin.iResult == -8 ||
                    bt.Corner4.InspectResults.resultDataMin.iResult == -8)
                {
                    this.AngleNG++;
                }

            }
            else if (bt.ResultCode == EResultCodes.ShotFail)
            {
                this.OtherNG++;
            }
        }

        public void Reset()
        {
            this.StartTime = DateTime.Now;
            this.TotalNum = 0;
            this.OKNum = 0;
            this.NGNum = 0;
            this.ScanNG = 0;
            this.MesNG = 0;
            this.ThincknessNG = 0;
            this.DimensionNG = 0;
            this.OCVNG = 0;
            this.IVNG = 0;
            this.WrinkleNG = 0;
            this.DistanceNG = 0;
            this.AngleNG = 0;
            this.OtherNG = 0;
            this.XRayNG = 0;
            this.PPM = 0;
            this.ScanTotal = 0;
            this.Save();
        }

        private void Read()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("Statistics", "TotalNum", @"0", builder, 1024, this._configFile);
            this.TotalNum = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "OKNum", @"0", builder, 1024, this._configFile);
            this.OKNum = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "NGNum", @"0", builder, 1024, this._configFile);
            this.NGNum = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "ScanNG", @"0", builder, 1024, this._configFile);
            this.ScanNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "ScanTotal", @"0", builder, 1024, this._configFile);
            this.ScanTotal = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "ThincknessNG", @"0", builder, 1024, this._configFile);
            this.ThincknessNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "DimensionNG", @"0", builder, 1024, this._configFile);
            this.DimensionNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "OCVNG", @"0", builder, 1024, this._configFile);
            this.OCVNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "IVNG", @"0", builder, 1024, this._configFile);
            this.IVNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "MesNG", @"0", builder, 1024, this._configFile);
            this.MesNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "WrinkleNG", @"0", builder, 1024, this._configFile);
            this.WrinkleNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "DistanceNG", @"0", builder, 1024, this._configFile);
            this.DistanceNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "AngleNG", @"0", builder, 1024, this._configFile);
            this.AngleNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "XRayNG", @"0", builder, 1024, this._configFile);
            this.XRayNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "OtherNG", @"0", builder, 1024, this._configFile);
            this.OtherNG = int.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "StartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), builder, 1024, this._configFile);
            this.StartTime = DateTime.Parse(builder.ToString());

            GetPrivateProfileString("Statistics", "PPM", @"0", builder, 1024, this._configFile);
            this.PPM = float.Parse(builder.ToString());

        }

    }
}
