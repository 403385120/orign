using Shuyz.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core.Options
{
    public class ReModelRead : ObservableObject
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                   string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);

        private static ReModelRead _instance = new ReModelRead();

        public static ReModelRead Instance
        {
            get { return _instance; }
        }

        private List<string> _listNames = new List<string>();
        private string _path = @"\\atlbattery.com\nd-fqi\MISpec-FQI正常规格";
        private string _source = string.Empty;
        private float _minBatLength = 0;
        private float _maxBatLength = 0;
        private float _minBatWidth = 0;
        private float _maxBatWidth = 0;
        private float _minBatThickness = 0;
        private float _maxBatThickness = 0;
        private float _minLeftLugMargin = 0;
        private float _maxLeftLugMargin = 0;
        private float _minRightLugMargin = 0;
        private float _maxRightLugMargin = 0;
        private float _minLeftLugLength = 0;
        private float _maxLeftLugLength = 0;
        private float _minRightLugLength = 0;
        private float _maxRightLugLength = 0;
        private float _minLeft1WhiteGlue = 0;
        private float _maxLeft1WhiteGlue = 0;
        private float _minLeft2WhiteGlue = 0;
        private float _maxLeft2WhiteGlue = 0;
        private float _minRight1WhiteGlue = 0;
        private float _maxRight1WhiteGlue = 0;
        private float _minRight2WhiteGlue = 0;
        private float _maxRight2WhiteGlue = 0;

        private float _minVoltage = 0;
        private float _maxVoltage = 0;
        private float _minResistance = 0;
        private float _maxResistance = 0;
        private float _minTemperature = 0;
        private float _maxTemperature = 0;
        private float _minIV = 0;
        private float _maxIV = 0;
        private float _ivSource = 0;
        private float _ivRange = 0;
        private float _ivExceptionData1 = 0;
        private float _ivExceptionData2 = 0;
        private int _layerAC = 0;
        private int _layerBD = 0;
        private int _maxAngleThresh = 0;


        public List<string> ListNames
        {
            get { return _listNames; }
        }

        /// <summary>
        /// 电池最小主体长度
        /// </summary>
        public float MinBatLength
        {
            get { return _minBatLength; }

            set
            {
                _minBatLength = value;
                RaisePropertyChanged("MinBatLength");
            }
        }

        /// <summary>
        /// 电池最大主体长度
        /// </summary>
        public float MaxBatLength
        {
            get { return _maxBatLength; }

            set
            {
                _maxBatLength = value;
                RaisePropertyChanged("MaxBatLength");
            }
        }

        /// <summary>
        /// 电池最小主体宽度
        /// </summary>
        public float MinBatWidth
        {
            get { return _minBatWidth; }

            set
            {
                _minBatWidth = value;
                RaisePropertyChanged("MinBatWidth");
            }
        }

        /// <summary>
        /// 电池最大主体宽度
        /// </summary>
        public float MaxBatWidth
        {
            get { return _maxBatWidth; }

            set
            {
                _maxBatWidth = value;
                RaisePropertyChanged("MaxBatWidth");
            }
        }

        /// <summary>
        /// 电池最小厚度
        /// </summary>
        public float MinBatThickness
        {
            get { return _minBatThickness; }

            set
            {
                _minBatThickness = value;
                RaisePropertyChanged("MinBatThickness");
            }
        }

        /// <summary>
        /// 电池最大厚度
        /// </summary>
        public float MaxBatThickness
        {
            get { return _maxBatThickness; }

            set
            {
                _maxBatThickness = value;
                RaisePropertyChanged("MaxBatThickness");
            }
        }

        /// <summary>
        /// 左极耳最小边距
        /// </summary>
        public float MinLeftLugMargin
        {
            get { return _minLeftLugMargin; }

            set
            {
                _minLeftLugMargin = value;
                RaisePropertyChanged("MinLeftLugMargin");
            }
        }

        /// <summary>
        /// 左极耳最大边距
        /// </summary>
        public float MaxLeftLugMargin
        {
            get { return _maxLeftLugMargin; }

            set
            {
                _maxLeftLugMargin = value;
                RaisePropertyChanged("MaxLeftLugMargin");
            }
        }

        /// <summary>
        /// 右极耳最小边距
        /// </summary>
        public float MinRightLugMargin
        {
            get { return _minRightLugMargin; }

            set
            {
                _minRightLugMargin = value;
                RaisePropertyChanged("MinRightLugMargin");
            }
        }

        /// <summary>
        /// 右极耳最大边距
        /// </summary>
        public float MaxRightLugMargin
        {
            get { return _maxRightLugMargin; }

            set
            {
                _maxRightLugMargin = value;
                RaisePropertyChanged("MaxRightLugMargin");
            }
        }

        /// <summary>
        /// 左极耳最小长度
        /// </summary>
        public float MinLeftLugLength
        {
            get { return _minLeftLugLength; }

            set
            {
                _minLeftLugLength = value;
                RaisePropertyChanged("MinLeftLugLength");
            }
        }

        /// <summary>
        /// 左极耳最大长度
        /// </summary>
        public float MaxLeftLugLength
        {
            get { return _maxLeftLugLength; }

            set
            {
                _maxLeftLugLength = value;
                RaisePropertyChanged("MaxLeftLugLength");
            }
        }

        /// <summary>
        /// 右极耳最小长度
        /// </summary>
        public float MinRightLugLength
        {
            get { return _minRightLugLength; }

            set
            {
                _minRightLugLength = value;
                RaisePropertyChanged("MinRightLugLength");
            }
        }

        /// <summary>
        /// 右极耳最大长度
        /// </summary>
        public float MaxRightLugLength
        {
            get { return _maxRightLugLength; }

            set
            {
                _maxRightLugLength = value;
                RaisePropertyChanged("MaxRightLugLength");
            }
        }

        /// <summary>
        /// 左1小白胶最小值
        /// </summary>
        public float MinLeft1WhiteGlue
        {
            get { return _minLeft1WhiteGlue; }

            set
            {
                _minLeft1WhiteGlue = value;
                RaisePropertyChanged("MinLeft1WhiteGlue");
            }
        }

        /// <summary>
        /// 左2小白胶最大值
        /// </summary>
        public float MaxLeft1WhiteGlue
        {
            get { return _maxLeft1WhiteGlue; }

            set
            {
                _maxLeft1WhiteGlue = value;
                RaisePropertyChanged("MaxLeft1WhiteGlue");
            }
        }

        /// <summary>
        /// 左2小白胶最小值
        /// </summary>
        public float MinLeft2WhiteGlue
        {
            get { return _minLeft2WhiteGlue; }

            set
            {
                _minLeft2WhiteGlue = value;
                RaisePropertyChanged("MinLeft2WhiteGlue");
            }
        }

        /// <summary>
        /// 左2小白胶最大值
        /// </summary>
        public float MaxLeft2WhiteGlue
        {
            get { return _maxLeft2WhiteGlue; }

            set
            {
                _maxLeft2WhiteGlue = value;
                RaisePropertyChanged("MaxLeft2WhiteGlue");
            }
        }

        /// <summary>
        /// 右1小白胶最小值
        /// </summary>
        public float MinRight1WhiteGlue
        {
            get { return _minRight1WhiteGlue; }

            set
            {
                _minRight1WhiteGlue = value;
                RaisePropertyChanged("MinRight1WhiteGlue");
            }
        }

        /// <summary>
        /// 右2小白胶最大值
        /// </summary>
        public float MaxRight1WhiteGlue
        {
            get { return _maxRight1WhiteGlue; }

            set
            {
                _maxRight1WhiteGlue = value;
                RaisePropertyChanged("MaxRight1WhiteGlue");
            }
        }

        /// <summary>
        /// 右2小白胶最小值
        /// </summary>
        public float MinRight2WhiteGlue
        {
            get { return _minRight2WhiteGlue; }

            set
            {
                _minRight2WhiteGlue = value;
                RaisePropertyChanged("MinRight2WhiteGlue");
            }
        }

        /// <summary>
        /// 右2小白胶最大值
        /// </summary>
        public float MaxRight2WhiteGlue
        {
            get { return _maxRight2WhiteGlue; }

            set
            {
                _maxRight2WhiteGlue = value;
                RaisePropertyChanged("MaxRight2WhiteGlue");
            }
        }

        /// <summary>
        /// 电压最小值
        /// </summary>
        public float MinVoltage
        {
            get { return _minVoltage; }

            set
            {
                _minVoltage = value;
                RaisePropertyChanged("MinVoltage");
            }
        }

        /// <summary>
        /// 电池电压最大值
        /// </summary>
        public float MaxVoltage
        {
            get { return _maxVoltage; }

            set
            {
                _maxVoltage = value;
                RaisePropertyChanged("MaxVoltage");
            }
        }

        /// <summary>
        /// 电池内阻最小值
        /// </summary>
        public float MinResistance
        {
            get { return _minResistance; }

            set
            {
                _minResistance = value;
                RaisePropertyChanged("MinResistance");
            }
        }

        /// <summary>
        /// 电池内阻最大值
        /// </summary>
        public float MaxResistance
        {
            get { return _maxResistance; }

            set
            {
                _maxResistance = value;
                RaisePropertyChanged("MaxResistance");
            }
        }

        /// <summary>
        /// 电池温度最小值
        /// </summary>
        public float MinTemperature
        {
            get { return _minTemperature; }

            set
            {
                _minTemperature = value;
                RaisePropertyChanged("MinTemperature");
            }
        }

        /// <summary>
        /// 电池温度最大值
        /// </summary>
        public float MaxTemperature
        {
            get { return _maxTemperature; }

            set
            {
                _maxTemperature = value;
                RaisePropertyChanged("MaxTemperature");
            }
        }

        /// <summary>
        /// IV最小值
        /// </summary>
        public float MinIV
        {
            get { return _minIV; }

            set
            {
                _minIV = value;
                RaisePropertyChanged("MinIV");
            }
        }

        /// <summary>
        /// IV最大值
        /// </summary>
        public float MaxIV
        {
            get { return _maxIV; }

            set
            {
                _maxIV = value;
                RaisePropertyChanged("MaxIV");
            }
        }

        /// <summary>
        /// IV初始值
        /// </summary>
        public float IvSource
        {
            get { return _ivSource; }

            set
            {
                _ivSource = value;
                RaisePropertyChanged("IvSource");
            }
        }

        /// <summary>
        /// IV跳变值
        /// </summary>
        public float IvRange
        {
            get { return _ivRange; }

            set
            {
                _ivRange = value;
                RaisePropertyChanged("IvRange");
            }
        }

        /// <summary>
        /// IV异常值1
        /// </summary>
        public float IvExceptionData1
        {
            get { return _ivExceptionData1; }

            set
            {
                _ivExceptionData1 = value;
                RaisePropertyChanged("IvExceptionData1");
            }
        }

        /// <summary>
        /// IV异常值2
        /// </summary>
        public float IvExceptionData2
        {
            get { return _ivExceptionData2; }

            set
            {
                _ivExceptionData2 = value;
                RaisePropertyChanged("IvExceptionData2");
            }
        }

        /// <summary>
        /// 电池AC角层数
        /// </summary>
        public int LayerAC
        {
            get { return _layerAC; }

            set
            {
                _layerAC = value;
                RaisePropertyChanged("LayerAC");
            }
        }

        /// <summary>
        /// 电池BD角层数
        /// </summary>
        public int LayerBD
        {
            get { return _layerBD; }

            set
            {
                _layerBD = value;
                RaisePropertyChanged("LayerBD");
            }
        }

        public int MaxAngleThresh
        {
            get { return _maxAngleThresh; }

            set
            {
                _maxAngleThresh = value;
                RaisePropertyChanged("MaxAngleThresh");
            }
        }

        /// <summary>
        /// 原数据
        /// </summary>
        public string Source
        {
            get { return _source; }

            set
            {
                _source = value;
                RaisePropertyChanged("Source");
            }
        }

        public List<string> GetFilesName(string path)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                FileInfo[] files = dirInfo.GetFiles();
                _listNames.Clear();
                foreach (FileInfo info in files)
                {
                    _listNames.Add(info.Name);
                }
                _path = path;
            }
            catch (Exception ex)
            {

            }
            return _listNames;
        }

        public List<string> GetFilesNameWithoutExtension(string path)
        {
            List<string> names = new List<string>();
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo info in files)
                {
                    names.Add(Path.GetFileNameWithoutExtension(info.FullName));
                }
            }
            catch (Exception ex)
            {

            }
            return names;
        }

        public void ReadConfigParams(string filename)
        {
            string filePath = Path.Combine(_path, filename);
            string extension = Path.GetExtension(filePath);
            if (!File.Exists(filePath) || extension.ToUpper() != ".INI")
            {
                Source = "";
                return;
            }

            UpdateSource(filePath);

            StringBuilder builder = new StringBuilder(1024);
            float min, max;

            GetPrivateProfileString("Config", "厚度", "4.49|4.79", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinBatThickness = min;
            this.MaxBatThickness = max;

            GetPrivateProfileString("Config", "宽", "40.92|41.92", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinBatWidth = min;
            this.MaxBatWidth = max;

            GetPrivateProfileString("Config", "长", "89.81|90.81", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinBatLength = min;
            this.MaxBatLength = max;

            if (CheckParamsConfig.Instance.IsAlOnLeft)
            {
                GetPrivateProfileString("Config", "Al_tab边距_min", "11.94|13.94", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinLeftLugMargin = min;

                GetPrivateProfileString("Config", "Al_tab边距_max", "11.94|13.94", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxLeftLugMargin = max;

                GetPrivateProfileString("Config", "Ni_tab边距_min", "26.14|28.14", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinRightLugMargin = min;

                GetPrivateProfileString("Config", "Ni_tab边距_max", "26.14|28.14", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxRightLugMargin = max;

                GetPrivateProfileString("Config", "Al_tab长度", "7|9", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinLeftLugLength = min;
                this.MaxLeftLugLength = max;

                GetPrivateProfileString("Config", "Ni_tab长度", "7|9", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinRightLugLength = min;
                this.MaxRightLugLength = max;

                GetPrivateProfileString("Config", "Al_tab小白胶_min", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinLeft1WhiteGlue = min;
                this.MinLeft2WhiteGlue = min;

                GetPrivateProfileString("Config", "Al_tab小白胶_max", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxLeft1WhiteGlue = max;
                this.MaxLeft2WhiteGlue = max;

                GetPrivateProfileString("Config", "Ni_tab小白胶_min", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinRight1WhiteGlue = min;
                this.MinRight2WhiteGlue = min;

                GetPrivateProfileString("Config", "Ni_tab小白胶_max", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxRight1WhiteGlue = max;
                this.MaxRight2WhiteGlue = max;
            }
            else
            {
                GetPrivateProfileString("Config", "Al_tab边距_min", "11.94|13.94", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinRightLugMargin = min;

                GetPrivateProfileString("Config", "Al_tab边距_max", "11.94|13.94", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxRightLugMargin = max;

                GetPrivateProfileString("Config", "Ni_tab边距_min", "26.14|28.14", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinLeftLugMargin = min;

                GetPrivateProfileString("Config", "Ni_tab边距_max", "26.14|28.14", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxLeftLugMargin = max;

                GetPrivateProfileString("Config", "Al_tab长度", "7|9", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinRightLugLength = min;
                this.MaxRightLugLength = max;

                GetPrivateProfileString("Config", "Ni_tab长度", "7|9", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinLeftLugLength = min;
                this.MaxLeftLugLength = max;

                GetPrivateProfileString("Config", "Al_tab小白胶_min", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinRight1WhiteGlue = min;
                this.MinRight2WhiteGlue = min;

                GetPrivateProfileString("Config", "Al_tab小白胶_max", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxRight1WhiteGlue = max;
                this.MaxRight2WhiteGlue = max;

                GetPrivateProfileString("Config", "Ni_tab小白胶_min", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MinLeft1WhiteGlue = min;
                this.MinLeft2WhiteGlue = min;

                GetPrivateProfileString("Config", "Ni_tab小白胶_max", "0.2|2.0", builder, 1024, filePath);
                GetConfigData(builder.ToString(), out min, out max);
                this.MaxLeft1WhiteGlue = max;
                this.MaxLeft2WhiteGlue = max;
            }

            GetPrivateProfileString("Config", "电池电压", "3.98|4.03", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinVoltage = min;
            this.MaxVoltage = min;

            GetPrivateProfileString("Config", "电池内阻", "13|21", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinResistance = min;
            this.MaxResistance = min;

            GetPrivateProfileString("Config", "电池温度", "20|30", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinTemperature = min;
            this.MaxTemperature = min;

            GetPrivateProfileString("Config", "IV初始跳变值", "0.3|0.45", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.IvSource = min;
            this.IvRange = min;

            GetPrivateProfileString("Config", "IV上下限", "-0.3|0.95", builder, 1024, filePath);
            GetConfigData(builder.ToString(), out min, out max);
            this.MinIV = min;
            this.MaxIV = min;

            GetPrivateProfileString("Config", "IV异常值1", "-0.1", builder, 1024, filePath);
            this.IvExceptionData1 = float.Parse(builder.ToString());

            GetPrivateProfileString("Config", "IV异常值2", "0.95", builder, 1024, filePath);
            this.IvExceptionData2 = float.Parse(builder.ToString());

            GetPrivateProfileString("Config", "电池层数(AC)", "12", builder, 1024, filePath);
            this.LayerAC = int.Parse(builder.ToString());

            GetPrivateProfileString("Config", "电池层数(BD)", "12", builder, 1024, filePath);
            this.LayerBD = int.Parse(builder.ToString());

            GetPrivateProfileString("Config", "最大角度阈值", "12", builder, 1024, filePath);
            this.LayerAC = int.Parse(builder.ToString());

        }

        private void UpdateSource(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader streamReader = new StreamReader(fs, Encoding.Default);
                    string line = "", value = "";
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        value += line + "\r\n";
                    }
                    Source = value;
                }
            }
            catch (Exception)
            {
                Source = "";
            }
        }

        private bool GetConfigData(string value, out float min, out float max)
        {
            min = 0; max = 0;
            string[] arr = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 2 || !float.TryParse(arr[0], out min) || !float.TryParse(arr[1], out max))
            {
                return false;
            }
            return true;
        }
    }
}
