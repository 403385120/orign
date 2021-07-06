using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;
using System.Threading;

namespace ZY.XRayTube
{
    public class HamamatsuXrayTube : IXRayTube
    {
        private XRayTubeConfig _xrayConfig;
        private SerialDevice.SerialDevice _serialDevice = new ZY.SerialDevice.SerialDevice();

        private object _cmdSyncObj = new object();
        private XRayTubeStatus _XRayTubeStatus = new XRayTubeStatus();

        private bool _isThreadOn = true;
        private Thread _pollingThread = null;

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
        public HamamatsuXrayTube(XRayTubeConfig xrayConfig)
        {
            this._xrayConfig = xrayConfig;
            this.Status.XRayTubeType = XRayTubeTypes.HamamatsuXrayTube;
        }

        /// <summary>
        /// 带锁读写
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string ExecuteCmd(string cmd)
        {
            //cmd = cmd.Trim();

            if (cmd == string.Empty) return string.Empty;
            //if (!this._XRayTubeStatus.IsConnectSuccess) return string.Empty;

            lock (this._cmdSyncObj)
            {
                // 该函数在线程循环调用
                // 吃掉异常防止奔溃
                try
                {
                    if (!this._serialDevice.WriteString(cmd)) return string.Empty;
                    Thread.Sleep(100);
                    return this._serialDevice.ReadToString("\r");
                }
                catch
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
            string s = this.ExecuteCmd("\r");

            if (s != "ERR 0 NOC") return false;
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
            this.ExecuteCmd("XON\r");
            return true;
        }

        /// <summary>
        /// 关闭光管
        /// </summary>
        /// <returns></returns>
        public bool XRayClose()
        {
            //if (!_XRayTubeStatus.IsConnectSuccess)
            //{
            //    return true;
            //}

            // Send the flag anyway
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
            this.ExecuteCmd("XOF\r");
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
            this.ExecuteCmd(string.Format("HIV {0}\r", kv));

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
            this.ExecuteCmd(string.Format("CUR {0}\r", ua));

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

            return this.SetCurrentToTube(t);
        }

        /// <summary>
        /// 设置在一定时间内没有从外部控制设备收到指令时，设定停止X 线的时间。以秒为单位
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool SetStopTimeToTube(int time)
        {
            this.ExecuteCmd(string.Format("AST {0}\r", time));

            return true;
        }

        public bool SetAlarmTime(int time)
        {
            this._xrayConfig.AlarmTime = time;
            return true;
        }

        /// <summary>
        /// 查询当前电压，属于状态获取
        /// </summary>
        /// <returns></returns>
        private bool GetActualVoltage()
        {
            string chTmp = this.ExecuteCmd("SHV\r");

            string num = System.Text.RegularExpressions.Regex.Replace(chTmp, @"[^0-9]+", "");
            int nlen = num.Length;

            if (nlen > 0 && nlen < 19)
            {
                int temp;

                int.TryParse(num, out temp);

                _XRayTubeStatus.ActualVoltage = temp;
            }

            return true;
        }

        /// <summary>
        /// 查询当前电流，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetActualCurrent()
        {
            string chTmp = this.ExecuteCmd("SCU\r");

            string num = System.Text.RegularExpressions.Regex.Replace(chTmp, @"[^0-9]+", "");
            int nlen = num.Length;
            if (nlen > 0 && nlen < 19)
            {
                int temp;

                int.TryParse(num, out temp);

                this._XRayTubeStatus.ActualCurrent = temp;
            }

            return true;
        }

        /// <summary>
        /// 光管预热
        /// </summary>
        /// <returns></returns>
        private bool Warmup()
        {
            this.ExecuteCmd("WUP\r");

            return true;
        }

        /// <summary>
        /// 获取预热状态，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetWarmStatus()
        {
            string chTmp = this.ExecuteCmd("SPH\r");

            if (string.Compare(chTmp, "SPH 0", true) == 0)
            {
                _XRayTubeStatus.IsWarmupComplete = true;  //预热完成
            }
            else if (string.Compare(chTmp, "SWE 1", true) == 0)
            {
                _XRayTubeStatus.IsWarmupComplete = false;  //预热中
            }
            return true;
        }

        /// <summary>
        /// 获取联锁状态类型，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetInterLockStatus()
        {
            string chTmp = this.ExecuteCmd("SIN\r");

            //chTmp = _serialDevice.ReadToString("\r");
            if (string.Compare(chTmp, "SIN 0", true) == 0)
            {
                _XRayTubeStatus.IsInterLockOn = true;  // 联锁闭合
            }
            else if (string.Compare(chTmp, "SIN 1", true) == 0)
            {
                _XRayTubeStatus.IsInterLockOn = false; // 联锁断开
            }
            return true;
        }

        /*  public string GetXrayType()
          {
              string str = "TYP\r";
              lock (obj)
              {
                  if (_serialDevice.WriteString(str))
                  {
                      string chTmp;
                      chTmp = _serialDevice.ReadToString("\r");
                      return chTmp;
                  }
                  return null;
              }
          }*/

        /// <summary>
        /// 获取硬件的错误类型，属于状态获取
        /// </summary>
        /// <returns></returns>
        public bool GetCheckErrorStatus()
        {
            string chTmp = this.ExecuteCmd("SER\r");

            if (string.Compare(chTmp, "SER 0", true) == 0)
            {
                _XRayTubeStatus.IsHardwareError = true;   //未出错 
            }
            else if (string.Compare(chTmp, "SER 1", true) == 0)
            {
                _XRayTubeStatus.IsHardwareError = false;  //出错 
            }
            return true;
        }

        /// <summary>
        /// 获取光管的打开总的时间，包括预热和打开的时间，属于状态获取
        /// </summary>
        /// <returns></returns>
        public int GetUseHoursInTotal()
        {
            string chTmp = this.ExecuteCmd("SXT\r");

            string num = System.Text.RegularExpressions.Regex.Replace(chTmp, @"[^0-9]+", "");
            int nlen = num.Length;
            if (nlen > 0 && nlen < 19)
            {
                int temp = 0;
                int.TryParse(num, out temp);
                this._XRayTubeStatus.UseHoursInTotal = temp;
            }

            return this._XRayTubeStatus.UseHoursInTotal;
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
            string chTmp = this.ExecuteCmd("STS\r");

            if (string.Compare(chTmp, "STS 0", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.WarmStandby;
            }
            else if (string.Compare(chTmp, "STS 1", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.WarmingUp;
            }
            else if (string.Compare(chTmp, "STS 2", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.XRayStandBy;
            }
            else if (string.Compare(chTmp, "STS 3", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.XRayRadiation;
            }
            else if (string.Compare(chTmp, "STS 4", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.Overload;
            }
            else if (string.Compare(chTmp, "STS 5", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.XRayDisabled;
            }
            else if (string.Compare(chTmp, "STS 6", true) == 0)
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.SelfChecking;
            }
            else
            {
                this._XRayTubeStatus.TubeStatus = ETubeStatus.OtherStatus;
            }
        
            return this._XRayTubeStatus.TubeStatus;
        }

        /// <summary>
        /// 循环更新和检测光管的状态，并循环向光管发送指令，更新光管状态。此函数在上层调用时，可通过一个线程来调用。
        /// </summary>
        /// <returns></returns>
        public void XRay_Control()
        {
            while (true)
            {
                if (!this._isThreadOn)
                {
                    break;
                }

                if (!this._XRayTubeStatus.IsConnectSuccess)
                {
                    Thread.Sleep(500);
                    continue;
                }

                //开光前条件确认
                // 1. 待机照射中
                // 2. 联锁确认
                // 3. 硬件错误
                GetWarmStatus();
                GetInterLockStatus();
                GetCheckErrorStatus();
                GetUseHoursInTotal();
                GetStatus();

                // 打开X光后持续发送电压电流
                GetActualVoltage();
                GetActualCurrent();

                if (_XRayTubeStatus.TubeStatus == ETubeStatus.WarmStandby && this._XRayTubeStatus.ShouldXrayOn)
                {
                    Warmup();
                }

                if (this._XRayTubeStatus.ShouldXrayOn && this._XRayTubeStatus.IsInterLockOn && this._XRayTubeStatus.IsHardwareError &&
                    (this._XRayTubeStatus.TubeStatus == ETubeStatus.XRayStandBy || this._XRayTubeStatus.TubeStatus == ETubeStatus.XRayRadiation))
                {
                    this.XOn();


                    if (_XRayTubeStatus.ActualVoltage < this._xrayConfig.PresetVoltage)
                    {
                        SetVoltageToTube(this._xrayConfig.PresetVoltage);
                    }
                    if (_XRayTubeStatus.ActualCurrent < this._xrayConfig.PresetCurrent)
                    {
                        SetCurrentToTube(this._xrayConfig.PresetCurrent);
                    }
                }
                else
                {
                    if (this._XRayTubeStatus.ShouldXrayOn && (_XRayTubeStatus.TubeStatus == ETubeStatus.WarmStandby
                        || _XRayTubeStatus.TubeStatus == ETubeStatus.WarmingUp))
                    {
                        // 等待预热完成
                        continue;
                    }
                    else
                    {
                        this.XOff();
                    }
                }
            }
        }

    }
}

