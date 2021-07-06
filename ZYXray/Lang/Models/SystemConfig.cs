using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using System.Runtime.InteropServices;
using System.IO;

namespace ZYXray.Models
{
    public class SystemConfig : ObservableObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                       string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);

        private readonly string _configFile = System.Environment.CurrentDirectory + "\\SystemConfig.ini";

        private static SystemConfig _instance = new SystemConfig();
        public static SystemConfig Instance
        {
            get { return _instance; }
        }

        private SystemConfig()
        {
            this.Read();
        }

        private string _lang = string.Empty;
        private string _updateDir = "D:\\XRayClientUpdate";
        private float _resultScale = 1.0f;
        private int _mistCleanDays = 20;
        private string _scanTestCode = string.Empty;    //扫码枪测试条码


        public string Lang
        {
            get { return this._lang; }
            set
            {
                if (this._lang == value) return;

                this._lang = value;
                RaisePropertyChanged("Lang");
            }
        }

        public string UpdateDir
        {
            get { return this._updateDir; }
            set
            {
                this._updateDir = value;
                RaisePropertyChanged("UpdateDir");
            }
        }
        
        public string ScanTestCode
        {
            get { return this._scanTestCode; }
            set
            {
                this._scanTestCode = value;
                RaisePropertyChanged("ScanTestCode");
            }
        }

        public float ResultScale
        {
            get { return this._resultScale; }
            set
            {
                if (value < 1 || value > 2.5) return;

                this._resultScale = value;
                RaisePropertyChanged("ResultScale");

                //   this.WriteGeneralConfig();
                WritePrivateProfileString("General", "ResultScale", _resultScale.ToString(), this._configFile);

            }
        }

        public int MistCleanDays
        {
            get { return this._mistCleanDays; }
            set
            {
                this._mistCleanDays = value;
                RaisePropertyChanged("MistCleanDays");
            }
        }

        private void Read()
        {
            if (!File.Exists(this._configFile))
            {
                this.Write();
            }

            this.ReadGeneralConfig();
        }

        public void Write()
        {
            this.WriteGeneralConfig();
        }

        #region - General -
        
        private void ReadGeneralConfig()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("General", "Lang", @"简体中文", builder, 1024, this._configFile);
            this.Lang = builder.ToString();

            GetPrivateProfileString("General", "UpdateDir", this.UpdateDir, builder, 1024, this._configFile);
            this.UpdateDir = builder.ToString();

            GetPrivateProfileString("General", "ResultScale", this.ResultScale.ToString(), builder, 1024, this._configFile);
            this.ResultScale = float.Parse(builder.ToString());

            GetPrivateProfileString("General", "MiscCleanDays", this.MistCleanDays.ToString(), builder, 1024, this._configFile);
            this.MistCleanDays = int.Parse(builder.ToString());
        }

        private void WriteGeneralConfig()
        {
            WritePrivateProfileString("General", "Lang", this.Lang, this._configFile);
            WritePrivateProfileString("General", "UpdateDir", this.UpdateDir, this._configFile);
            WritePrivateProfileString("General", "ResultScale", this.ResultScale.ToString(), this._configFile);
            WritePrivateProfileString("General", "MiscCleanDays", this.MistCleanDays.ToString(), this._configFile);
        }

        #endregion
    }
}
