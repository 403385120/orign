using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.XRayTube;
using ZY.SerialDevice;
using ZY.BarCodeReader;
using ZY.MitutoyoReader;
using XRayClient.VisionSysWrapper;
using System.Runtime.InteropServices;
using System.IO;
using Instrument;

namespace ZYXray.Models
{
    public class HardwareConfig
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                         string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                                                          StringBuilder retVal, int size, string filePath);

        private readonly string _configFile = System.Environment.CurrentDirectory + "\\HardwareConfig.ini";

        private static HardwareConfig _instance = new HardwareConfig();

        public static HardwareConfig Instance
        {
            get { return _instance; }
        }

        private HardwareConfig()
        {
            this.ReadConfig();
        }

        #region - Privates -

        private float _caliDist = 1;   // 标定片物理距离
        private float _pixelDist = 1;   //像素距离
        private float _pixelRatio = 0;  //像素比

        private int _cameraShotDelay1 = 0;  //工位拍照延时 by fjy
        private int _cameraShotDelay2 = 0;
        private int _cameraShotDelay3 = 0;
        private int _cameraShotDelay4 = 0;
        private int _scanbarcodeDelay = 0;

        private string _ipAdress = string.Empty;
        private int _port = 0;
        private string _ipAdress2 = string.Empty;
        private int _port2 = 0;

        private int _XRayTubeIdleTime = 0;

        /// <summary>
        /// 光管1的配置
        /// </summary>
        private XRayTubeConfig _xRayConfig1 = new XRayTubeConfig(new SerialDeviceConfig("COM1", 38400, 8, 0, 0), 75, 152, 11000, 2000);
        private XRayTubeConfig _xRayConfig2 = new XRayTubeConfig(new SerialDeviceConfig("COM2", 38400, 8, 0, 0), 75, 152, 11000, 2000);

        /// <summary>
        /// 扫码枪的配置
        /// </summary>
        private CodeReaderConfig _barcodeConfig = new CodeReaderConfig(new SerialDeviceConfig("COM3", 115200, 8, 0, 2));

        private MitutoyoReaderConfig _mitutoyoConfig = new MitutoyoReaderConfig(new SerialDeviceConfig("COM7", 2400, 8, 0, 0));
        private MitutoyoReaderConfig _mitutoyoConfig2 = new MitutoyoReaderConfig(new SerialDeviceConfig("COM8", 2400, 8, 0, 0));
        private MitutoyoReaderConfig _mitutoyoConfig3 = new MitutoyoReaderConfig(new SerialDeviceConfig("COM7", 2400, 8, 0, 0));
        private MitutoyoReaderConfig _mitutoyoConfig4 = new MitutoyoReaderConfig(new SerialDeviceConfig("COM8", 2400, 8, 0, 0));

        private SerialPortConfig _bT3562SerialPortConfig = new SerialPortConfig();
        private SerialPortConfig _e5CCSerialPortConfig = new SerialPortConfig();
        private SerialPortConfig _mI3SerialPortConfig1 = new SerialPortConfig();
        private SerialPortConfig _mI3SerialPortConfig2 = new SerialPortConfig();
        private string _ip34461A;
        private string _ipLR8401;
        private string _ipToprie;
        private string _ipToprie2;
        private int _portLR8401;
        private int _portToprie;
        private int _portToprie2;

        private CameraParams _cameraConfig1 = new CameraParams()
        {
            camType = 0,
            nWidth = 1600,
            nHeight = 1200,
            PinValue = 7,
            exposure = 40000,
            gain = 1,
            min_graylevel = 0,
            max_graylevel = 4000,
            pixelRatio = 1,
            pixelRatio2 = 1,
            SzBufSeriNum = "8779071518",
            xFlip = false,
            yFlip = false

        };

        private CameraParams _cameraConfig2 = new CameraParams()
        {
            camType = 0,
            nWidth = 1600,
            nHeight = 1200,
            PinValue = 7,
            exposure = 40000,
            gain = 1,
            min_graylevel = 0,
            max_graylevel = 4000,
            pixelRatio = 1,
            SzBufSeriNum = "8777591518",
            xFlip = false,
            yFlip = false
        };

        #endregion

        public float CaliDist
        {
            get { return this._caliDist; }
            set
            {
                this._caliDist = value;
            }
        }


        public float PixelDist
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("Camera1Config", "PixelDist", @"0", builder, 1024, this._configFile);
                _pixelDist = float.Parse(builder.ToString());
                return this._pixelDist;
            }
            set
            {
                this._pixelDist = value;
                WritePrivateProfileString("Camera1Config", "PixelDist", _pixelDist.ToString(), this._configFile);
            }
        }

        public float PixelRatio
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("Camera1Config", "PixelRatio", @"0", builder, 1024, this._configFile);
                _pixelRatio = float.Parse(builder.ToString());
                return this._pixelRatio;
            }
            set
            {
                this._pixelRatio = value;
                WritePrivateProfileString("Camera1Config", "PixelRatio", _pixelRatio.ToString(), this._configFile);
            }
        }

        public XRayTubeConfig XRayConfig1
        {
            get { return this._xRayConfig1; }
        }

        public XRayTubeConfig XRayConfig2
        {
            get { return this._xRayConfig2; }
        }

        public CodeReaderConfig BarcodeConfig
        {
            get { return this._barcodeConfig; }
        }

        public MitutoyoReaderConfig MitutoyoConfig
        {
            get { return this._mitutoyoConfig; }
        }

        public MitutoyoReaderConfig MitutoyoConfig2
        {
            get { return this._mitutoyoConfig2; }
        }
        public MitutoyoReaderConfig MitutoyoConfig3
        {
            get { return this._mitutoyoConfig3; }
        }
        public MitutoyoReaderConfig MitutoyoConfig4
        {
            get { return this._mitutoyoConfig4; }
        }

        public CameraParams CameraParams1
        {
            get { return this._cameraConfig1; }
            set { this._cameraConfig1 = value; }
        }

        public CameraParams CameraParams2
        {
            get { return this._cameraConfig2; }
            set { this._cameraConfig2 = value; }
        }

        public SerialPortConfig BT3562SerialPortConfig
        {
            get { return this._bT3562SerialPortConfig; }
        }

        public SerialPortConfig E5CCSerialPortConfig
        {
            get { return this._e5CCSerialPortConfig; }
        }
        public SerialPortConfig MI3SerialPortConfig1
        {
            get { return this._mI3SerialPortConfig1; }
        }
        public SerialPortConfig MI3SerialPortConfig2
        {
            get { return this._mI3SerialPortConfig2; }
        }
        public string Ip34461A
        {
            get
            {
                return this._ip34461A;
            }
        }
        public string IpLR8401
        {
            get
            {
                return this._ipLR8401;
            }
        }
        public int PortLR8401
        {
            get
            {
                return this._portLR8401;
            }
        }

        public string IpToprie
        {
            get
            {
                return this._ipToprie;
            }
        }
        public int PortToprie
        {
            get
            {
                return this._portToprie;
            }
        }
        public string IpToprie2
        {
            get
            {
                return this._ipToprie2;
            }
        }
        public int PortToprie2
        {
            get
            {
                return this._portToprie2;
            }
        }

        public int CameraShotDelay1
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("Camera1Config", "ShotDelayTime1", @"0", builder, 1024, this._configFile);
                _cameraShotDelay1 = int.Parse(builder.ToString());
                return this._cameraShotDelay1;
            }
            set
            {
                this._cameraShotDelay1 = value;
                WritePrivateProfileString("Camera1Config", "ShotDelayTime1", _cameraShotDelay1.ToString(), this._configFile);
            }
        }
        public int CameraShotDelay2
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("Camera1Config", "ShotDelayTime2", @"0", builder, 1024, this._configFile);
                _cameraShotDelay2 = int.Parse(builder.ToString());
                return this._cameraShotDelay2;
            }
            set
            {
                this._cameraShotDelay2 = value;
                WritePrivateProfileString("Camera1Config", "ShotDelayTime2", _cameraShotDelay2.ToString(), this._configFile);
            }
        }
        public int CameraShotDelay3
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("Camera1Config", "ShotDelayTime3", @"0", builder, 1024, this._configFile);
                _cameraShotDelay3 = int.Parse(builder.ToString());
                return this._cameraShotDelay3;
            }
            set
            {
                this._cameraShotDelay3 = value;
                WritePrivateProfileString("Camera1Config", "ShotDelayTime3", _cameraShotDelay3.ToString(), this._configFile);
            }
        }
        public int CameraShotDelay4
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("Camera1Config", "ShotDelayTime4", @"0", builder, 1024, this._configFile);
                _cameraShotDelay4 = int.Parse(builder.ToString());
                return this._cameraShotDelay4;
            }
            set
            {
                this._cameraShotDelay4 = value;
                WritePrivateProfileString("Camera1Config", "ShotDelayTime4", _cameraShotDelay4.ToString(), this._configFile);
            }
        }
        public int ScanBarcodeDelay
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "ScanBarcodeDelayTime", @"0", builder, 1024, this._configFile);
                _scanbarcodeDelay = int.Parse(builder.ToString());
                return this._scanbarcodeDelay;
            }
            set
            {
                this._scanbarcodeDelay = value;
                WritePrivateProfileString("CodeReaderConfig", "ScanBarcodeDelayTime", _scanbarcodeDelay.ToString(), this._configFile);
            }
        }

        public int XRayTubeIdleTime
        {
            get
            {
                return this._XRayTubeIdleTime;
            }
            set
            {
                this._XRayTubeIdleTime = value;
            }
        }
        private void ReadConfig()
        {
            if (!System.IO.File.Exists(this._configFile))
            {
                this.SaveConfig();
                return;
            }

            this.ReadCodeReaderConfig();

            this.ReadMitutoyoReaderConfig();

            this.ReadMitutoyoReaderConfig2();
            this.ReadMitutoyoReaderConfig3();
            this.ReadMitutoyoReaderConfig4();

            this.ReadXRayTubeConfig("XRayTube1Config", ref this._xRayConfig1);
            this.ReadXRayTubeConfig("XRayTube2Config", ref this._xRayConfig2);

            this.ReadCameraConfig("Camera1Config", ref this._cameraConfig1);
            this.ReadCameraConfig("Camera2Config", ref this._cameraConfig2);

            this.ReadBT3562ReaderConfig();
            this.ReadE5CCReaderConfig();
            this.ReadMI3ReaderConfig1();
            this.ReadMI3ReaderConfig2();
            this.ReadIp34461AConfig();
            this.ReadLR8401Config();
            this.ReadToprieConfig();

        }

        public string ScanBarcodeIPAdress
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "ScanBarcodeIPAdress", @"192.168.250.20", builder, 1024, this._configFile);
                _ipAdress = builder.ToString();
                return this._ipAdress;
            }
        }

        public int ScanBarcodePort
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "ScanBarcodePort", @"9004", builder, 1024, this._configFile);
                _port = int.Parse(builder.ToString());
                return this._port;
            }
        }
        public string ScanBarcodeIPAdress2
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "ScanBarcodeIPAdress2", @"192.168.101.2", builder, 1024, this._configFile);
                _ipAdress2 = builder.ToString();
                return this._ipAdress2;
            }
        }

        public int ScanBarcodePort2
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "ScanBarcodePort2", @"9004", builder, 1024, this._configFile);
                _port2 = int.Parse(builder.ToString());
                return this._port2;
            }
        }

        public string DimensionIPAdress
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "DimensionIPAdress", @"127.0.0.1", builder, 1024, this._configFile);
                _ipAdress = builder.ToString();
                return this._ipAdress;
            }
        }

        public int DimensionPort
        {
            get
            {
                StringBuilder builder = new StringBuilder(1024);
                GetPrivateProfileString("CodeReaderConfig", "DimensionPort", @"6600", builder, 1024, this._configFile);
                _port = int.Parse(builder.ToString());
                return this._port;
            }
        }
        public void SaveXTubeConfigOnly()
        {
            this.SaveXRayTubeConfig("XRayTube1Config", this.XRayConfig1);
            this.SaveXRayTubeConfig("XRayTube2Config", this.XRayConfig2);
        }

        public void SaveConfig(int num = 0)
        {
            this.SaveCodeReaderConfig();

            this.SaveMitutoyoReaderConfig();

            this.SaveMitutoyoReaderConfig2();
            this.SaveMitutoyoReaderConfig3();
            this.SaveMitutoyoReaderConfig4();

            this.SaveXRayTubeConfig("XRayTube1Config", this.XRayConfig1);
            this.SaveXRayTubeConfig("XRayTube2Config", this.XRayConfig2);

            if (num == 1)
            { this.SaveCameraConfig("Camera1Config", this._cameraConfig1); }
            else if (num == 2)
            { this.SaveCameraConfig("Camera2Config", this._cameraConfig2); }
            else
            {
                this.SaveCameraConfig("Camera1Config", this._cameraConfig1);
                this.SaveCameraConfig("Camera2Config", this._cameraConfig2);
            }

            this.SaveBT3562Config();
            this.SaveE5CCConfig();
            this.SaveMI3Config1();
            this.SaveMI3Config2();
            this.SaveIp34461AConfig();
            this.SaveLR8401Config();
            this.SaveToprieConfig();
        }

        #region - XRay -

        //光管2的配置文件
        private void ReadXRayTubeConfig(string section, ref XRayTubeConfig xrayParams)
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString(section, "PortName", @"COM2", builder, 1024, this._configFile);
            xrayParams.SerialConfig.PortName = builder.ToString();

            GetPrivateProfileString(section, "BaudRate", @"38400", builder, 1024, this._configFile);
            xrayParams.SerialConfig.BaudRate = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "DataBits", @"8", builder, 1024, this._configFile);
            xrayParams.SerialConfig.DataBits = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "StopBits", @"0", builder, 1024, this._configFile);
            xrayParams.SerialConfig.StopBits = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "Parity", @"0", builder, 1024, this._configFile);
            xrayParams.SerialConfig.Parity = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "XRayTubeTypes", @"HamamatsuXrayTube", builder, 1024, this._configFile);
            xrayParams.XRayTubeType = (XRayTubeTypes)Enum.Parse(typeof(XRayTubeTypes), builder.ToString());

            GetPrivateProfileString(section, "PresetVoltage", @"75", builder, 1024, this._configFile);
            xrayParams.PresetVoltage = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "PresetCurrent", @"152", builder, 1024, this._configFile);
            xrayParams.PresetCurrent = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "StopTime", @"2000", builder, 1024, this._configFile);
            xrayParams.StopTime = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "AutoOpen", @"True", builder, 1024, this._configFile);
            xrayParams.AutoOpen = bool.Parse(builder.ToString());

            GetPrivateProfileString(section, "AlarmTime", @"11000", builder, 1024, this._configFile);
            xrayParams.AlarmTime = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "XRayTubeIdleTime", @"0", builder, 1024, this._configFile);
            _XRayTubeIdleTime = int.Parse(builder.ToString());
        }

        private void SaveXRayTubeConfig(string section, XRayTubeConfig xrayParams)
        {
            WritePrivateProfileString(section, "PortName", xrayParams.SerialConfig.PortName, this._configFile);
            WritePrivateProfileString(section, "BaudRate", xrayParams.SerialConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString(section, "DataBits", xrayParams.SerialConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString(section, "StopBits", xrayParams.SerialConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString(section, "Parity", xrayParams.SerialConfig.Parity.ToString(), this._configFile);
            WritePrivateProfileString(section, "XRayTubeTypes", xrayParams.XRayTubeType.ToString(), this._configFile);
            WritePrivateProfileString(section, "PresetVoltage", xrayParams.PresetVoltage.ToString(), this._configFile);
            WritePrivateProfileString(section, "PresetCurrent", xrayParams.PresetCurrent.ToString(), this._configFile);
            WritePrivateProfileString(section, "StopTime", xrayParams.StopTime.ToString(), this._configFile);
            WritePrivateProfileString(section, "AutoOpen", xrayParams.AutoOpen.ToString(), this._configFile);
            WritePrivateProfileString(section, "AlarmTime", xrayParams.AlarmTime.ToString(), this._configFile);
        }
        #endregion

        #region - CodeReader -

        //扫码枪的配置文件
        private void ReadCodeReaderConfig()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("CodeReaderConfig", "PortName", @"COM3", builder, 1024, this._configFile);
            this._barcodeConfig.SerialConfig.PortName = builder.ToString();

            GetPrivateProfileString("CodeReaderConfig", "BaudRate", @"115200", builder, 1024, this._configFile);
            this._barcodeConfig.SerialConfig.BaudRate = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("CodeReaderConfig", "DataBits", @"8", builder, 1024, this._configFile);
            this._barcodeConfig.SerialConfig.DataBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("CodeReaderConfig", "StopBits", @"0", builder, 1024, this._configFile);
            this._barcodeConfig.SerialConfig.StopBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("CodeReaderConfig", "Parity", @"2", builder, 1024, this._configFile);
            this._barcodeConfig.SerialConfig.Parity = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("CodeReaderConfig", "CodeReaderTypes", @"KeyenceSerial", builder, 1024, this._configFile);
            this._barcodeConfig.CodeReaderType = (CodeReaderTypes)Enum.Parse(typeof(CodeReaderTypes), builder.ToString());

        }
        private void SaveCodeReaderConfig()
        {
            WritePrivateProfileString("CodeReaderConfig", "PortName", this.BarcodeConfig.SerialConfig.PortName, this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "BaudRate", this.BarcodeConfig.SerialConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "DataBits", this.BarcodeConfig.SerialConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "StopBits", this.BarcodeConfig.SerialConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "Parity", this.BarcodeConfig.SerialConfig.Parity.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "CodeReaderTypes", this.BarcodeConfig.CodeReaderType.ToString(), this._configFile);

            WritePrivateProfileString("CodeReaderConfig", "ScanBarcodeIPAdress", this.ScanBarcodeIPAdress.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "ScanBarcodePort", this.ScanBarcodePort.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "ScanBarcodeIPAdress2", this.ScanBarcodeIPAdress2.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "ScanBarcodePort2", this.ScanBarcodePort2.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "DimensionIPAdress", this.DimensionIPAdress.ToString(), this._configFile);
            WritePrivateProfileString("CodeReaderConfig", "DimensionPort", this.DimensionPort.ToString(), this._configFile);
        }
        #endregion

        private void SaveBT3562Config()
        {
            WritePrivateProfileString("BT3562Config", "PortName", this.BT3562SerialPortConfig.PortName, this._configFile);
            WritePrivateProfileString("BT3562Config", "BaudRate", this.BT3562SerialPortConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("BT3562Config", "DataBits", this.BT3562SerialPortConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("BT3562Config", "StopBits", this.BT3562SerialPortConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("BT3562Config", "Parity", this.BT3562SerialPortConfig.Parity.ToString(), this._configFile);
        }
        private void SaveE5CCConfig()
        {
            WritePrivateProfileString("E5CCConfig", "PortName", this.E5CCSerialPortConfig.PortName, this._configFile);
            WritePrivateProfileString("E5CCConfig", "BaudRate", this.E5CCSerialPortConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("E5CCConfig", "DataBits", this.E5CCSerialPortConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("E5CCConfig", "StopBits", this.E5CCSerialPortConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("E5CCConfig", "Parity", this.E5CCSerialPortConfig.Parity.ToString(), this._configFile);
        }

        private void SaveMI3Config1()
        {
            WritePrivateProfileString("MI3Config1", "PortName", this.MI3SerialPortConfig1.PortName, this._configFile);
            WritePrivateProfileString("MI3Config1", "BaudRate", this.MI3SerialPortConfig1.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("MI3Config1", "DataBits", this.MI3SerialPortConfig1.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("MI3Config1", "StopBits", this.MI3SerialPortConfig1.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("MI3Config1", "Parity", this.MI3SerialPortConfig1.Parity.ToString(), this._configFile);
        }
        private void SaveMI3Config2()
        {
            WritePrivateProfileString("MI3Config2", "PortName", this.MI3SerialPortConfig2.PortName, this._configFile);
            WritePrivateProfileString("MI3Config2", "BaudRate", this.MI3SerialPortConfig2.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("MI3Config2", "DataBits", this.MI3SerialPortConfig2.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("MI3Config2", "StopBits", this.MI3SerialPortConfig2.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("MI3Config2", "Parity", this.MI3SerialPortConfig2.Parity.ToString(), this._configFile);
        }
        private void SaveIp34461AConfig()
        {
            WritePrivateProfileString("Ip34461AConfig", "Ip34461A", this._ip34461A, this._configFile);
        }
        private void SaveLR8401Config()
        {
            WritePrivateProfileString("LR8401Config", "IpLR8401", this._ipLR8401, this._configFile);
            WritePrivateProfileString("LR8401Config", "PortLR8401", this._portLR8401.ToString(), this._configFile);
        }
        private void SaveToprieConfig()
        {
            WritePrivateProfileString("IpToprieConfig", "IpToprie", this._ipToprie, this._configFile);
            WritePrivateProfileString("IpToprieConfig", "PortToprie", this._portToprie.ToString(), this._configFile);
            WritePrivateProfileString("IpToprieConfig", "IpToprie2", this._ipToprie2, this._configFile);
            WritePrivateProfileString("IpToprieConfig", "PortToprie2", this._portToprie2.ToString(), this._configFile);

        }

        #region - MitutoyoReader -
        private void ReadMitutoyoReaderConfig()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("MitutoyoConfig", "PortName", @"COM2", builder, 1024, this._configFile);
            this._mitutoyoConfig.SerialConfig.PortName = builder.ToString();

            GetPrivateProfileString("MitutoyoConfig", "BaudRate", @"2400", builder, 1024, this._configFile);
            this._mitutoyoConfig.SerialConfig.BaudRate = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig", "DataBits", @"8", builder, 1024, this._configFile);
            this._mitutoyoConfig.SerialConfig.DataBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig", "StopBits", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig.SerialConfig.StopBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig", "Parity", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig.SerialConfig.Parity = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig", "MitutoyoReaderTypes", @"Mitutoyo", builder, 1024, this._configFile);
            this._mitutoyoConfig.MitutoyoReaderType = (MitutoyoReaderTypes)Enum.Parse(typeof(MitutoyoReaderTypes), builder.ToString());

        }
        private void SaveMitutoyoReaderConfig()
        {
            WritePrivateProfileString("MitutoyoConfig", "PortName", this.MitutoyoConfig.SerialConfig.PortName, this._configFile);
            WritePrivateProfileString("MitutoyoConfig", "BaudRate", this.MitutoyoConfig.SerialConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig", "DataBits", this.MitutoyoConfig.SerialConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig", "StopBits", this.MitutoyoConfig.SerialConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig", "Parity", this.MitutoyoConfig.SerialConfig.Parity.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig", "MitutoyoReaderTypes", this.MitutoyoConfig.MitutoyoReaderType.ToString(), this._configFile);
        }

        private void ReadBT3562ReaderConfig()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("BT3562Config", "PortName", @"COM1", builder, 1024, this._configFile);
            this.BT3562SerialPortConfig.PortName = builder.ToString();
            GetPrivateProfileString("BT3562Config", "BaudRate", @"9600", builder, 1024, this._configFile);
            this.BT3562SerialPortConfig.BaudRate = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("BT3562Config", "DataBits", @"8", builder, 1024, this._configFile);
            this.BT3562SerialPortConfig.DataBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("BT3562Config", "StopBits", @"1", builder, 1024, this._configFile);
            this.BT3562SerialPortConfig.StopBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("BT3562Config", "Parity", @"0", builder, 1024, this._configFile);
            this.BT3562SerialPortConfig.Parity = Convert.ToInt32(builder.ToString());
        }

        private void ReadE5CCReaderConfig()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("E5CCConfig", "PortName", @"COM1", builder, 1024, this._configFile);
            this.E5CCSerialPortConfig.PortName = builder.ToString();
            GetPrivateProfileString("E5CCConfig", "BaudRate", @"9600", builder, 1024, this._configFile);
            this.E5CCSerialPortConfig.BaudRate = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("E5CCConfig", "DataBits", @"8", builder, 1024, this._configFile);
            this.E5CCSerialPortConfig.DataBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("E5CCConfig", "StopBits", @"1", builder, 1024, this._configFile);
            this.E5CCSerialPortConfig.StopBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("E5CCConfig", "Parity", @"0", builder, 1024, this._configFile);
            this.E5CCSerialPortConfig.Parity = Convert.ToInt32(builder.ToString());
        }

        private void ReadMI3ReaderConfig1()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("MI3Config1", "PortName", @"COM1", builder, 1024, this._configFile);
            this.MI3SerialPortConfig1.PortName = builder.ToString();
            GetPrivateProfileString("MI3Config1", "BaudRate", @"9600", builder, 1024, this._configFile);
            this.MI3SerialPortConfig1.BaudRate = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("MI3Config1", "DataBits", @"8", builder, 1024, this._configFile);
            this.MI3SerialPortConfig1.DataBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("MI3Config1", "StopBits", @"1", builder, 1024, this._configFile);
            this.MI3SerialPortConfig1.StopBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("MI3Config1", "Parity", @"0", builder, 1024, this._configFile);
            this.MI3SerialPortConfig1.Parity = Convert.ToInt32(builder.ToString());
        }
        private void ReadMI3ReaderConfig2()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("MI3Config2", "PortName", @"COM1", builder, 1024, this._configFile);
            this.MI3SerialPortConfig2.PortName = builder.ToString();
            GetPrivateProfileString("MI3Config2", "BaudRate", @"9600", builder, 1024, this._configFile);
            this.MI3SerialPortConfig2.BaudRate = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("MI3Config2", "DataBits", @"8", builder, 1024, this._configFile);
            this.MI3SerialPortConfig2.DataBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("MI3Config2", "StopBits", @"1", builder, 1024, this._configFile);
            this.MI3SerialPortConfig2.StopBits = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("MI3Config2", "Parity", @"0", builder, 1024, this._configFile);
            this.MI3SerialPortConfig2.Parity = Convert.ToInt32(builder.ToString());
        }
        private void ReadIp34461AConfig()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("Ip34461AConfig", "Ip34461A", @"127.0.0.1", builder, 1024, this._configFile);
            _ip34461A = builder.ToString();
        }
        private void ReadLR8401Config()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("LR8401Config", "IpLR8401", @"127.0.0.1", builder, 1024, this._configFile);
            _ipLR8401 = builder.ToString();
            GetPrivateProfileString("LR8401Config", "PortLR8401", @"3000", builder, 1024, this._configFile);
            _portLR8401 = Convert.ToInt32(builder.ToString());
        }
        private void ReadToprieConfig()
        {
            StringBuilder builder = new StringBuilder(1024);
            GetPrivateProfileString("IpToprieConfig", "IpToprie", @"127.0.0.1", builder, 1024, this._configFile);
            _ipToprie = builder.ToString();
            GetPrivateProfileString("IpToprieConfig", "PortToprie", @"3000", builder, 1024, this._configFile);
            _portToprie = Convert.ToInt32(builder.ToString());
            GetPrivateProfileString("IpToprieConfig", "IpToprie2", @"127.0.0.1", builder, 1024, this._configFile);
            _ipToprie2 = builder.ToString();
            GetPrivateProfileString("IpToprieConfig", "PortToprie2", @"3000", builder, 1024, this._configFile);
            _portToprie2 = Convert.ToInt32(builder.ToString());

        }

        private void ReadMitutoyoReaderConfig2()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("MitutoyoConfig2", "PortName", @"COM1", builder, 1024, this._configFile);
            this._mitutoyoConfig2.SerialConfig.PortName = builder.ToString();

            GetPrivateProfileString("MitutoyoConfig2", "BaudRate", @"2400", builder, 1024, this._configFile);
            this._mitutoyoConfig2.SerialConfig.BaudRate = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig2", "DataBits", @"8", builder, 1024, this._configFile);
            this._mitutoyoConfig2.SerialConfig.DataBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig2", "StopBits", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig2.SerialConfig.StopBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig2", "Parity", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig2.SerialConfig.Parity = int.Parse(builder.ToString()); ;
        }
        private void ReadMitutoyoReaderConfig3()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("MitutoyoConfig3", "PortName", @"COM1", builder, 1024, this._configFile);
            this._mitutoyoConfig3.SerialConfig.PortName = builder.ToString();

            GetPrivateProfileString("MitutoyoConfig3", "BaudRate", @"2400", builder, 1024, this._configFile);
            this._mitutoyoConfig3.SerialConfig.BaudRate = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig3", "DataBits", @"8", builder, 1024, this._configFile);
            this._mitutoyoConfig3.SerialConfig.DataBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig3", "StopBits", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig3.SerialConfig.StopBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig3", "Parity", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig3.SerialConfig.Parity = int.Parse(builder.ToString()); ;
        }
        private void ReadMitutoyoReaderConfig4()
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString("MitutoyoConfig4", "PortName", @"COM1", builder, 1024, this._configFile);
            this._mitutoyoConfig4.SerialConfig.PortName = builder.ToString();

            GetPrivateProfileString("MitutoyoConfig4", "BaudRate", @"2400", builder, 1024, this._configFile);
            this._mitutoyoConfig4.SerialConfig.BaudRate = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig4", "DataBits", @"8", builder, 1024, this._configFile);
            this._mitutoyoConfig4.SerialConfig.DataBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig4", "StopBits", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig4.SerialConfig.StopBits = int.Parse(builder.ToString()); ;

            GetPrivateProfileString("MitutoyoConfig4", "Parity", @"0", builder, 1024, this._configFile);
            this._mitutoyoConfig4.SerialConfig.Parity = int.Parse(builder.ToString()); ;
        }

        private void SaveMitutoyoReaderConfig2()
        {
            WritePrivateProfileString("MitutoyoConfig2", "PortName", this.MitutoyoConfig2.SerialConfig.PortName, this._configFile);
            WritePrivateProfileString("MitutoyoConfig2", "BaudRate", this.MitutoyoConfig2.SerialConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig2", "DataBits", this.MitutoyoConfig2.SerialConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig2", "StopBits", this.MitutoyoConfig2.SerialConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig2", "Parity", this.MitutoyoConfig2.SerialConfig.Parity.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig2", "MitutoyoReaderTypes", this.MitutoyoConfig2.MitutoyoReaderType.ToString(), this._configFile);
        }
        private void SaveMitutoyoReaderConfig3()
        {
            WritePrivateProfileString("MitutoyoConfig3", "PortName", this.MitutoyoConfig3.SerialConfig.PortName, this._configFile);
            WritePrivateProfileString("MitutoyoConfig3", "BaudRate", this.MitutoyoConfig3.SerialConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig3", "DataBits", this.MitutoyoConfig3.SerialConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig3", "StopBits", this.MitutoyoConfig3.SerialConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig3", "Parity", this.MitutoyoConfig3.SerialConfig.Parity.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig3", "MitutoyoReaderTypes", this.MitutoyoConfig3.MitutoyoReaderType.ToString(), this._configFile);
        }
        private void SaveMitutoyoReaderConfig4()
        {
            WritePrivateProfileString("MitutoyoConfig4", "PortName", this.MitutoyoConfig4.SerialConfig.PortName, this._configFile);
            WritePrivateProfileString("MitutoyoConfig4", "BaudRate", this.MitutoyoConfig4.SerialConfig.BaudRate.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig4", "DataBits", this.MitutoyoConfig4.SerialConfig.DataBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig4", "StopBits", this.MitutoyoConfig4.SerialConfig.StopBits.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig4", "Parity", this.MitutoyoConfig4.SerialConfig.Parity.ToString(), this._configFile);
            WritePrivateProfileString("MitutoyoConfig4", "MitutoyoReaderTypes", this.MitutoyoConfig4.MitutoyoReaderType.ToString(), this._configFile);
        }

        #endregion
        #region - Camera -

        //相机的参数配置文件
        private void ReadCameraConfig(string section, ref CameraParams camParam)
        {
            StringBuilder builder = new StringBuilder(1024);

            GetPrivateProfileString(section, "camType", @"0", builder, 1024, this._configFile);
            camParam.camType = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "nWidth", @"1600", builder, 1024, this._configFile);
            camParam.nWidth = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "nHeight", @"1200", builder, 1024, this._configFile);
            camParam.nHeight = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "PinValue", @"8", builder, 1024, this._configFile);
            camParam.PinValue = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "exposure", @"40000", builder, 1024, this._configFile);
            camParam.exposure = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "gain", @"1", builder, 1024, this._configFile);
            camParam.gain = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "min_graylevel", @"1", builder, 1024, this._configFile);
            camParam.min_graylevel = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "max_graylevel", @"1", builder, 1024, this._configFile);
            camParam.max_graylevel = int.Parse(builder.ToString());

            GetPrivateProfileString(section, "pixelRatio", @"1", builder, 1024, this._configFile);
            camParam.pixelRatio = float.Parse(builder.ToString());

            GetPrivateProfileString(section, "pixelRatio2", @"1", builder, 1024, this._configFile);
            camParam.pixelRatio2 = float.Parse(builder.ToString());

            GetPrivateProfileString(section, "SzBufSeriNum", @"8777591518", builder, 1024, this._configFile);
            camParam.SzBufSeriNum = builder.ToString();

            GetPrivateProfileString(section, "xFlip", @"false", builder, 1024, this._configFile);
            camParam.xFlip = bool.Parse(builder.ToString());

            GetPrivateProfileString(section, "yFlip", @"false", builder, 1024, this._configFile);
            camParam.yFlip = bool.Parse(builder.ToString());
        }

        private void SaveCameraConfig(string section, CameraParams camParam)
        {
            WritePrivateProfileString(section, "camType", camParam.camType.ToString(), this._configFile);
            WritePrivateProfileString(section, "nWidth", camParam.nWidth.ToString(), this._configFile);
            WritePrivateProfileString(section, "nHeight", camParam.nHeight.ToString(), this._configFile);
            WritePrivateProfileString(section, "PinValue", camParam.PinValue.ToString(), this._configFile);
            WritePrivateProfileString(section, "exposure", camParam.exposure.ToString(), this._configFile);
            WritePrivateProfileString(section, "gain", camParam.gain.ToString(), this._configFile);
            WritePrivateProfileString(section, "min_graylevel", camParam.min_graylevel.ToString(), this._configFile);
            WritePrivateProfileString(section, "max_graylevel", camParam.max_graylevel.ToString(), this._configFile);
            WritePrivateProfileString(section, "pixelRatio", camParam.pixelRatio.ToString(), this._configFile);
            WritePrivateProfileString(section, "pixelRatio2", camParam.pixelRatio2.ToString(), this._configFile);
            WritePrivateProfileString(section, "SzBufSeriNum", camParam.SzBufSeriNum, this._configFile);
            WritePrivateProfileString(section, "xFlip", camParam.xFlip.ToString(), this._configFile);
            WritePrivateProfileString(section, "yFlip", camParam.yFlip.ToString(), this._configFile);
            WritePrivateProfileString(section, "PixelDist", PixelDist.ToString(), this._configFile);
            WritePrivateProfileString(section, "PixelRatio", PixelRatio.ToString(), this._configFile);
        }
        #endregion

        #region SoftWareVersion
        public void SaveSoftWareVersionConfig(string plc, string hmi, string soft)
        {
            if (!Directory.Exists("D:\\Andon\\")) Directory.CreateDirectory("D:\\Andon\\");

            string configFile = "D:\\Andon\\SoftWareVersion.ini";
            WritePrivateProfileString("SoftWareVersion", "PLC", plc, configFile);
            WritePrivateProfileString("SoftWareVersion", "HMI", hmi, configFile);
            WritePrivateProfileString("SoftWareVersion", "CCD视觉定位软件", "v1.0.0.0", configFile);
            WritePrivateProfileString("SoftWareVersion", "上位机软件", soft, configFile);

        }
        #endregion
    }
}
