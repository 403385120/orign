using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZY.XRayTube
{
    public class ReDianXRayTube : IXRayTube
    {
        private XRayTubeConfig _xrayConfig;
        private SerialDevice.SerialDevice _serialDevice = new ZY.SerialDevice.SerialDevice();

        private object _cmdSyncObj = new object();
        private XRayTubeStatus _XRayTubeStatus = new XRayTubeStatus();

        private bool _isThreadOn = true;
        private Thread _pollingThread = null;

        private bool oldSign = false;
        private string com = "COM1";

        public XRayTubeStatus Status
        {
            get { return this._XRayTubeStatus; }
        }

        /// <summary>
        /// 光管的构造函数
        /// </summary>
        /// <param name="deviceConfig">
        /// 传入的参数 就表示是哪个光管
        /// </param>
        public ReDianXRayTube(XRayTubeConfig xrayConfig)
        {
            this._xrayConfig = xrayConfig;
            this.Status.XRayTubeType = XRayTubeTypes.ReDianXrayTube;
            com = xrayConfig.SerialConfig.PortName;
        }

        /// <summary>
        /// 带锁读写
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string ExecuteCmd(string cmd)
        {
            lock (this._cmdSyncObj)
            {
                if (cmd == string.Empty) return string.Empty;
                //Logging.LoggingIF.Log(com + " cmd:" + cmd.Trim(), Logging.LogLevels.Debug, "ExecuteCmd");

                // 该函数在线程循环调用
                // 吃掉异常防止奔溃
                try
                {
                    if (!this._serialDevice.WriteString(cmd)) return string.Empty;
                    Thread.Sleep(250); //有的指令返回时间有点长               
                    string str = this._serialDevice.ReadAll();
                    //Logging.LoggingIF.Log(com + " res:" + str, Logging.LogLevels.Debug, "ExecuteCmd");
                    return str;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 断开连接，断开串口
        /// </summary>
        public void DisConnect()
        {
            this.XRayClose();

            this._isThreadOn = false;
            if (null != this._pollingThread)
            {
                this._pollingThread.Join(600);
            }

            if (!this._XRayTubeStatus.IsConnectSuccess) return;

            this.XOff();
            this._serialDevice.Close();

            this._XRayTubeStatus.IsConnectSuccess = false;
            //TODO Set other status
        }

        /// <summary>
        /// 连接串口通信并创建光管控制线程
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (this._XRayTubeStatus.IsConnectSuccess)
            {
                return true;
            }

            this._XRayTubeStatus.IsConnectSuccess = false;

            if (!this._serialDevice.OpenPort(this._xrayConfig.SerialConfig))
            {
                return false;
            }

            if (!this.CheckConnect())
            {
                return false;
            }

            this._XRayTubeStatus.IsConnectSuccess = true;
            this._isThreadOn = true;

            this._pollingThread = new Thread(this.XRay_Control);
            this._pollingThread.IsBackground = true;
            this._pollingThread.Start();

            return true;
        }

        public void ReloadConfig(XRayTubeConfig config)
        {
            this._xrayConfig = config;
        }

        /// <summary>
        /// 验证是否连接
        /// </summary>
        /// <returns></returns>
        private bool CheckConnect()
        {
            string s = this.ExecuteCmd("Hello\r");

            if (!s.Contains("Hello")) return false;
            return true;
        }

        /// <summary>
        /// 打开光管
        /// </summary>
        /// <returns></returns>
        public bool XRayOpen()
        {
            if (!this._XRayTubeStatus.IsConnectSuccess)
            {
                return false;
            }

            this._XRayTubeStatus.ShouldXrayOn = true;
            return true;
        }

        private bool XOn()
        {
            string str = this.ExecuteCmd("XRAY ON\r");
            if (str.Contains("OK"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭光管
        /// </summary>
        /// <returns></returns>
        public bool XRayClose()
        {
            _XRayTubeStatus.ShouldXrayOn = false;
            return true;
        }

        /// <summary>
        /// 自动开关
        /// </summary>
        /// <returns></returns>
        public bool XRayAutoOpen()
        {
            if (this._XRayTubeStatus.ShouldXrayOn) return true;

            if (this._xrayConfig.AutoOpen)
            {
                this.XRayOpen();
            }

            return true;
        }

        private bool XOff()
        {
            this.ExecuteCmd("XRAY OFF\r");
            return true;
        }

        /// <summary>
        /// 预设电压
        /// </summary>
        /// <param name="kv"></param>
        public bool SetVoltage(int kv)
        {
            this._xrayConfig.PresetVoltage = kv;

            return this.SetVoltageToTube(kv);
        }

        /// <summary>
        /// 向光管设置电压
        /// </summary>
        /// <param name="kv"></param>
        /// <returns></returns>
        private bool SetVoltageToTube(int kv)
        {
            string str = this.ExecuteCmd(string.Format("HV {0}\r", kv));

            return true;
        }

        /// <summary>
        /// 预设电流
        /// </summary>
        /// <param name="cu"></param>
        public bool SetCurrent(int cu)
        {
            this._xrayConfig.PresetCurrent = cu;

            return this.SetCurrentToTube(cu);
        }

        /// <summary>
        /// 向光管设置电流
        /// </summary>
        /// <param name="ua"></param>
        /// <returns></returns>
        private bool SetCurrentToTube(int ua)
        {
            this.ExecuteCmd(string.Format("BEAM {0}\r", ua));

            return true;
        }

        /// <summary>
        /// 设置自动停止时间
        /// </summary>
        /// <param name="t"></param>
        public bool SetStopTime(int t)
        {
            if (t < 3000) t = 3000; // 保护，不能超过3s

            this._xrayConfig.StopTime = t;

            return this.SetStopTimeToTube(t);
        }

        /// <summary>
        /// 设置在一定时间内没有从外部控制设备收到指令时，设定停止X 线的时间。以秒为单位
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool SetStopTimeToTube(int time)
        {
            // this.ExecuteCmd(string.Format("AST {0}\r", time));

            return true;
        }

        public bool SetAlarmTime(int time)
        {
            //this._xrayConfig.AlarmTime = time;
            return true;
        }

        /// <summary>
        /// 查询当前电压，属于状态获取
        /// </summary>
        /// <returns></returns>
        private bool GetActualVoltage()
        {
            string chTmp = this.ExecuteCmd("HV\r");
            if (chTmp.Contains("HV Measured"))
            {
                Regex r = new Regex(@"\d*\.\d*|0\.\d*[1-9]\d*$");
                string num = r.Match(chTmp).Value;
                //string num = System.Text.RegularExpressions.Regex.Replace(chTmp, @"\d*\.\d*|0\.\d*[1-9]\d*$", "");
                int nlen = num.Length;
                if (nlen > 0 && nlen < 19)
                {
                    int temp;
                    num = num.Split('.').First();
                    temp = Convert.ToInt32(num);
                    //int.TryParse(num, out temp);

                    _XRayTubeStatus.ActualVoltage = temp;
                }
            }

            return true;
        }

        /// <summary>
        /// 查询当前电流，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetActualCurrent()
        {
            string chTmp = this.ExecuteCmd("BEAM\r");

            if (chTmp.Contains("Beam Measured"))
            {
                Regex r = new Regex(@"\d*\.\d*|0\.\d*[1-9]\d*$");
                string num = r.Match(chTmp).Value;

                int nlen = num.Length;
                if (nlen > 0 && nlen < 19)
                {
                    int temp;
                    num = num.Split('.').First();
                    int.TryParse(num, out temp);

                    this._XRayTubeStatus.ActualCurrent = temp;
                }
            }

            return true;
        }

        /// <summary>
        /// 光管预热
        /// </summary>
        /// <returns></returns>
        private bool Warmup()
        {
            string str = this.ExecuteCmd("PROGRAM 001\r");

            if (str.Contains("beginning"))
            {
                str = this.ExecuteCmd("XRAY ON\r");
                if (str.Contains("Warmup"))
                {
                    _XRayTubeStatus.IsWarmupComplete = false;//预热中
                }
                else
                {
                    _XRayTubeStatus.IsWarmupComplete = true;//预热完成
                }
            }
            return true;
        }

        private int GetWarmTime(string str)
        {
            int nTime = 0;
            int nPos0 = str.IndexOf("NonOpSecsRemain");
            int nPos1 = str.IndexOf("TotalHours");

            if (nPos1 - nPos0 - 17 <= 0)
            {
                return 0;
            }
            string strValue = str.Substring(nPos0 + 16, nPos1 - nPos0 - 17);
            if (strValue != "")
            {
                nTime = Convert.ToInt32(strValue);
            }
            return nTime;
        }

        /// <summary>
        /// 获取联锁状态类型，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetInterLockStatus()
        {
            string chTmp = this.ExecuteCmd("INTERLOCK\r");

            //chTmp = _serialDevice.ReadToString("\r");
            if (!chTmp.Contains("Unsafe"))
            {
                _XRayTubeStatus.IsInterLockOn = true;  // 联锁闭合
            }
            else
            {
                _XRayTubeStatus.IsInterLockOn = false; // 联锁断开
            }
            return true;
        }


        /// <summary>
        /// 获取硬件的错误类型，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetCheckErrorStatus()
        {
            string chTmp = this.ExecuteCmd("ST\r");

            if (chTmp.Contains("!") && chTmp.Contains("ST"))
            {
                if (chTmp.Contains("Status"))
                {
                    _XRayTubeStatus.IsHardwareError = true;   //未出错 
                }
                else
                {
                    _XRayTubeStatus.IsHardwareError = false;  //出错 
                }
            }
            return true;
        }

        /// <summary>
        /// 获取光管的打开总的时间，包括预热和打开的时间，属于状态获取
        /// </summary>
        /// <returns></returns>
        public void GetUseHoursInTotal()
        {
            string chTmp = this.ExecuteCmd("TIMESTATS\r");
            //!TIMESTATS NonOpSecsRemain 0  TotalHours 12.1  TotalHoursXRAYon  1.0

            if (chTmp.Length > 20)
            {
                chTmp = chTmp.Replace("  ", " ");
                string num = chTmp.Split(' ').Last();
                //num = System.Text.RegularExpressions.Regex.Replace(chTmp, @"\d*\.\d*|0\.\d*[1-9]\d*$", "");
                if (num == "")
                {
                    return;
                }
                int nlen = num.Length;
                if (nlen > 0 && nlen < 19)
                {
                    int temp = 0;
                    temp = Convert.ToInt32(Convert.ToDouble(num));
                    //int.TryParse(num, out temp);
                    this._XRayTubeStatus.UseHoursInTotal = temp;
                }
            }
        }

        /// <summary>
        /// 获取光管的当前状态
        /// </summary>
        /// <returns>
        /// 0-预热待机中（需要预热）
        /// 1-预热动作中(正在预热)
        /// 2-X 线照射待机中
        /// 3-X 线照射中
        /// 4-过载保护功能动作中
        /// 5-X 线无法照射状态
        /// 6-自检动作中
        /// 其他-其他状态或者错误
        /// </returns>
        public ETubeStatus GetStatus()
        {
            string chTmp = this.ExecuteCmd("ST\r");
            if (chTmp.Contains("Status") && chTmp.Contains("!"))
            {
                if (chTmp.Contains("Warnup"))
                {
                    this._XRayTubeStatus.TubeStatus = ETubeStatus.WarmStandby;
                }
                else if ((chTmp.Contains("Nofocus") || chTmp.Contains("Infocus")) && chTmp.Contains("On"))
                {
                    this._XRayTubeStatus.TubeStatus = ETubeStatus.XRayRadiation;
                }
                else if ((chTmp.Contains("Nofocus") || chTmp.Contains("Infocus")) && chTmp.Contains("Off"))
                {
                    this._XRayTubeStatus.TubeStatus = ETubeStatus.XRayStandBy;
                }
            }

            return this._XRayTubeStatus.TubeStatus;
        }

        /// <summary>
        /// 循环更新和检测光管的状态，并循环向光管发送指令，更新光管状态。此函数在上层调用时，可通过一个线程来调用。
        /// </summary>
        /// <returns></returns>
        public void XRay_Control()
        {
            while (this._XRayTubeStatus.IsConnectSuccess)
            {
                int sts = (int)GetStatus();
                if (oldSign == false && this._XRayTubeStatus.ShouldXrayOn == true && sts == 0)//捕获到开光动作未预热就发一次预热指令
                {
                    Warmup();
                }
                oldSign = this._XRayTubeStatus.ShouldXrayOn;

                //以sts 2为条件              
                if (sts == 2 && _XRayTubeStatus.IsInterLockOn == true && _XRayTubeStatus.IsHardwareError == true)
                {
                    if (this._XRayTubeStatus.ShouldXrayOn)
                    {
                        this.XOn();
                        this._XRayTubeStatus.ShouldXrayOn = true;
                    }

                    //if (GetWarmStatus() == false)
                    //{
                    //    //刚预热完成，自动关闭x光
                    //    XOff();
                    //    _XRayTubeStatus.TubeStatus = ETubeStatus.XRayStandBy;
                    //    //m_bIsPreheat = false;
                    //    //m_bIsNeedWarmup = false;
                    //    //m_bTrunOn = false;
                    //    //m_bIsXrayOn = false;
                    //}
                }

                //if (this._XRayTubeStatus.ShouldXrayOn == true /*&& sts!=0 && sts!=1*/)
                //{
                //    this.XOn();
                //}


                //查询当前电压电流
                if (_XRayTubeStatus.ActualVoltage != this._xrayConfig.PresetVoltage)
                {
                    SetVoltageToTube(this._xrayConfig.PresetVoltage);
                }
                if (_XRayTubeStatus.ActualCurrent != this._xrayConfig.PresetCurrent)
                {
                    SetCurrentToTube(this._xrayConfig.PresetCurrent);
                }
                GetActualVoltage();
                GetActualCurrent();

                //开光前条件确认
                // 1. 待机照射中
                // 2. 联锁确认
                // 3. 硬件错误
                GetInterLockStatus();
                GetCheckErrorStatus();
                GetUseHoursInTotal();

                if ((sts != 0 && sts != 1 && sts != 2 && sts != 3) || this._XRayTubeStatus.ShouldXrayOn == false || _XRayTubeStatus.IsInterLockOn == false || _XRayTubeStatus.IsHardwareError == false /*|| m_bIsWarmupStatus != 0*/)
                {
                    XOff();
                    this._XRayTubeStatus.ShouldXrayOn = false;
                }

                ////	ChekUseHoursInTotal();
                Thread.Sleep(100);
            }
        }

    }
}
