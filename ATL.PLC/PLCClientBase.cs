using ATL.Common;
using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ATL.PLC
{
    public abstract class PLCClientBase
    {
        public PLCClientBase()
        {

        }
        /// <summary>
        /// PLC通信状态修改事件
        /// </summary>
        public event EventHandler<ConnectChangedEventArgs> OnConnectedChanged;
        /// <summary>
        /// 通信错误次数统计
        /// </summary>
        public uint CommunicationErrorCount = 0;
        /// <summary>
        /// 连续读取失败多少次后重新连接PLC
        /// </summary>
        public uint CommunicationErrorCountReconnect = 30;
        private bool isConnect;
        public bool IsConnect
        {
            get
            {
                return isConnect;
            }
            set
            {
                if(isConnect != value)
                {
                    CommunicationErrorCount = 0;
                    
                    if (OnConnectedChanged != null)
                    {
                        ConnectChangedEventArgs arg = new ConnectChangedEventArgs() { isConnected = value };
                        OnConnectedChanged(this, arg);
                    }
                }
                if (!value)
                {
                    CommunicationErrorCount++;
                    if(CommunicationErrorCount == CommunicationErrorCountReconnect)
                    {
                        string msg = string.Empty;
                        if(!Connect(ref msg))
                        {
                            if (ATL.Common.StringResources.IsDefaultLanguage)
                            {
                                LogInDB.Error($"重连 PLC[{ip}]失败: {msg}");
                            }
                            else
                            {
                                LogInDB.Error($"ReCommunicate PLC[{ip}]  Failure: {msg}");
                            }
                        }
                        CommunicationErrorCount = 0;
                    }
                }
                isConnect = value;
            }
        }
        public string ip;
        public int port;
        /// <summary>
        /// PlcConfigurationInfo表里配置的ID号
        /// </summary>
        public int id;
        public object CommunicationObjLock = new object();
        /// <summary>
        /// 上位机读取指令一次最低读取多少个字符。西门子PLC一次最少读取一个字节，为1个字符；三菱PLC一次最少读取一个寄存器，也就是2个字符
        /// </summary>
        public int AddressWordLength;
        public IByteTransform _ByteTransform;

        public abstract bool Connect(ref string msg);

        public abstract bool DisConnect();

        public abstract void Dispose();
        
        public abstract object Read<T>(string address, ref string Msg, ushort length = 1);
       
        public abstract bool Write<T>(string address, T value, ref string Msg);
        
        /// <summary>
        /// NotRW地址段的立即读取，而不是从周期读取的地址段缓存里解析值。不支持倍福PLC
        /// </summary>
        /// <param name="plcID"></param>
        /// <param name="address"></param>
        /// <param name="ValueType"></param>
        /// <param name="value"></param>
        /// <param name="length">对于西门子PLC，长度指Byte的长度；对于OmronPLC，则长度值Int16的长度</param>
        /// <returns></returns>
        public virtual bool ReadByAddress(string address, string ValueType, out string value, ushort length = 1)
        {
            string msg = string.Empty;
            value = string.Empty;
            if(!IsConnect)
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error($"PLC[{ip}]通信失败: 读取 {address} 失败");
                }
                else
                {
                    LogInDB.Error($"PLC[{ip}]Communication Failure: Read {address}Failure");
                }
            }
            try
            {
                switch (ValueType.ToUpper())
                {
                    case "STRING":
                        {
                            object _value = Read<string>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                IsConnect = false;
                                return false;
                            }
                            else
                            {
                                value = _value.ToString();
                                return true;
                            }
                        }
                    case "INT16":
                        {
                            object _value = Read<short[]>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                return false;
                            }
                            else if (length > 1)
                            {
                                value = string.Join(",", (short[])_value);
                                return true;
                            }
                            else
                            {
                                value = ((short[])_value)[0].ToString();
                                return true;
                            }
                        }
                    case "INT32":
                        {
                            object _value = Read<int[]>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                return false;
                            }
                            else if (length > 1)
                            {
                                value = string.Join(",", (int[])_value);
                                return true;
                            }
                            else
                            {
                                value = ((int[])_value)[0].ToString();
                                return true;
                            }
                        }
                    case "INT64":
                        {
                            object _value = Read<long[]>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                return false;
                            }
                            else if (length > 1)
                            {
                                value = string.Join(",", (long[])_value);
                                return true;
                            }
                            else
                            {
                                value = ((long[])_value)[0].ToString();
                                return true;
                            }
                        }
                    case "FLOAT":
                        {
                            object _value = Read<float[]>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                return false;
                            }
                            else if (length > 1)
                            {
                                value = string.Join(",", (float[])_value);
                                return true;
                            }
                            else
                            {
                                value = ((float[])_value)[0].ToString();
                                return true;
                            }
                        }
                    case "DOUBLE":
                        {
                            object _value = Read<double[]>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                return false;
                            }
                            else if (length > 1)
                            {
                                value = string.Join(",", (double[])_value);
                                return true;
                            }
                            else
                            {
                                value = ((double[])_value)[0].ToString();
                                return true;
                            }
                        }
                    case "BIT":
                    case "BOOL":
                        {
                            object _value = Read<bool[]>(address, ref msg, length);
                            if (_value == null)
                            {
                                value = string.Empty;
                                LogInDB.Error(msg);
                                return false;
                            }
                            else if (length > 1)
                            {
                                value = string.Join(",", (bool[])_value);
                                return true;
                            }
                            else
                            {
                                value = ((bool[])_value)[0].ToString();
                                return true;
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.ToString());
                return false;
            }
            return false;
        }


        /// <summary>
        /// 此方法的参数1，address，必须为PLC通信协议里支持的直接读写的地址。比如欧姆龙PLC支持W100.0位操作，而不能是D100.0的位操作。
        /// </summary>
        /// <param name="address">必须为PLC通信协议里支持的直接读写的地址。比如欧姆龙PLC支持W100.0位操作，而不能是D100.0的位操作。</param>
        /// <param name="ValueType"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public virtual bool WriteByAddress(string address, string ValueType, object Value)
        {
            if (string.IsNullOrEmpty(address)) return true;
            string msg = string.Empty;
            bool res = false;
            if (!IsConnect)
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error($"PLC[{ip}]通信失败: 写入 {address} 失败");
                }
                else
                {
                    LogInDB.Error($"PLC[{ip}]Communication Failure: Write {address}Failure");
                }
            }
            switch (ValueType.ToUpper())
            {
                case "BIT":
                case "BOOL":
                    {
                        res = Write(address, Array.ConvertAll(Value.ToString().Split(','), s => bool.Parse(s)), ref msg);
                    }
                    break;
                case "DOUBLE":
                    {
                        res = Write(address, Array.ConvertAll(Value.ToString().Split(','), s => double.Parse(s)), ref msg);
                    }
                    break;
                case "FLOAT":
                    {
                        res = Write(address, Array.ConvertAll(Value.ToString().Split(','), s => float.Parse(s)), ref msg);
                    }
                    break;
                case "INT16":
                    {
                        res = Write(address, Array.ConvertAll(Value.ToString().Split(','), s => short.Parse(s)), ref msg);
                    }
                    break;
                case "INT32":
                    {
                        res = Write(address, Array.ConvertAll(Value.ToString().Split(','), s => int.Parse(s)), ref msg);
                    }
                    break;
                case "INT64":
                    {
                        res = Write(address, Array.ConvertAll(Value.ToString().Split(','), s => long.Parse(s)), ref msg);
                    }
                    break;
                case "STRING":
                    {
                        res = Write(address, Value.ToString(), ref msg);
                    }
                    break;
            }
            if (!res)
            {
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    msg = $"写入[{address}] 失败: {msg}";
                    LogInDB.Error(msg);
                }
                else
                {
                    msg = $"Write[{address}] failed: {msg}";
                    LogInDB.Error(msg);
                }
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// PLC通信状态修改事件
    /// </summary>
    public class ConnectChangedEventArgs : EventArgs
    {
        public bool isConnected;
    }
}
