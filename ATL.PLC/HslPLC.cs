using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Panasonic;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATL.Common;

namespace ATL.PLC
{
    public class HslPLC : PLCClientBase
    {
        public HslPLC(IReadWriteNet plc)
        {
            AddressWordLength = 2;  //默认寄存器为16位；西门子的PLC和CIP协议的PLC为8位的寄存器
            Device = plc;
        }
        private IReadWriteNet device;
        /// <summary>
        /// 实例化的PLC连接
        /// </summary>
        public IReadWriteNet Device
        {
            get
            {
                return device;
            }
            set
            {
                device = value;
                if (device is SiemensS7Net)
                {
                    _ByteTransform = ((SiemensS7Net)device).ByteTransform;
                    AddressWordLength = 1;
                }
                else if (device is OmronFinsNet)
                {
                    _ByteTransform = ((OmronFinsNet)device).ByteTransform;
                    AddressWordLength = 2;
                }
                else if (device is OmronCipNet)
                {
                    _ByteTransform = ((OmronCipNet)device).ByteTransform;
                    AddressWordLength = 1;
                }
                else if (device is MelsecMcNet)
                {
                    _ByteTransform = ((MelsecMcNet)device).ByteTransform;
                    AddressWordLength = 2;
                }
                else if (device is MelsecMcAsciiNet)
                {
                    _ByteTransform = ((MelsecMcAsciiNet)device).ByteTransform;
                    AddressWordLength = 2;
                }
                else if (device is MelsecA1ENet)
                {
                    _ByteTransform = ((MelsecA1ENet)device).ByteTransform;
                    AddressWordLength = 2;
                }
                else if (device is MelsecA1EAsciiNet)
                {
                    _ByteTransform = ((MelsecA1EAsciiNet)device).ByteTransform;
                    AddressWordLength = 2;
                }
                else if (device is PanasonicMewtocolOverTcp)
                {
                    _ByteTransform = ((PanasonicMewtocolOverTcp)device).ByteTransform;
                    AddressWordLength = 2;
                }
            }
        }

        public override bool Connect(ref string msg)
        {
            try
            {
                OperateResult connect = null;
                if (Device is SiemensS7Net)
                {
                    connect = ((SiemensS7Net)Device).ConnectServer();
                }
                else if (Device is OmronFinsNet)
                {
                    connect = ((OmronFinsNet)Device).ConnectServer();
                }
                else if (Device is OmronCipNet)
                {
                    connect = ((OmronCipNet)Device).ConnectServer();
                }
                else if (Device is MelsecMcNet)
                {
                    connect = ((MelsecMcNet)Device).ConnectServer();
                }
                else if (Device is MelsecMcAsciiNet)
                {
                    connect = ((MelsecMcAsciiNet)Device).ConnectServer();
                }
                else if (Device is MelsecA1ENet)
                {
                    connect = ((MelsecA1ENet)Device).ConnectServer();
                }
                else if (Device is MelsecA1EAsciiNet)
                {
                    connect = ((MelsecA1EAsciiNet)Device).ConnectServer();
                }
                else if (Device is PanasonicMewtocolOverTcp)
                {
                    connect = ((PanasonicMewtocolOverTcp)Device).ConnectServer();
                }
                if(connect != null && connect.IsSuccess)
                {
                    IsConnect = true;
                }
                else
                {
                    IsConnect = false;
                    msg = connect.ToMessageShowString();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return IsConnect;
        }

        public override bool DisConnect()
        {
            try
            {
                if (Device is SiemensS7Net)
                {
                    SiemensS7Net plc = (SiemensS7Net)Device;
                    plc.ConnectClose();
                }
                else if (Device is OmronFinsNet)
                {
                    OmronFinsNet plc = (OmronFinsNet)Device;
                    plc.ConnectClose();
                }
                else if (Device is OmronCipNet)
                {
                    OmronCipNet plc = (OmronCipNet)Device;
                    plc.ConnectClose();
                }
                else if (Device is MelsecMcNet)
                {
                    MelsecMcNet plc = (MelsecMcNet)Device;
                    plc.ConnectClose();
                }
                else if (Device is MelsecMcAsciiNet)
                {
                    MelsecMcAsciiNet plc = (MelsecMcAsciiNet)Device;
                    plc.ConnectClose();
                }
                else if (Device is MelsecA1ENet)
                {
                    MelsecA1ENet plc = (MelsecA1ENet)Device;
                    plc.ConnectClose();
                }
                else if(Device is MelsecA1EAsciiNet)
                {
                    MelsecA1EAsciiNet plc = (MelsecA1EAsciiNet)Device;
                    plc.ConnectClose();
                }
                else if (Device is PanasonicMewtocolOverTcp)
                {
                    PanasonicMewtocolOverTcp plc = (PanasonicMewtocolOverTcp)Device;
                    plc.ConnectClose();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
        }

        ~HslPLC()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (Device is SiemensS7Net)
            {
                SiemensS7Net plc = (SiemensS7Net)Device;
                plc.ConnectClose();
                plc.Dispose();
                plc = null;
            }
            else if (Device is OmronFinsNet)
            {
                OmronFinsNet plc = (OmronFinsNet)Device;
                plc.ConnectClose();
                plc.Dispose();
                plc = null;
            }
            else if (Device is OmronCipNet)
            {
                OmronCipNet plc = (OmronCipNet)Device;
                plc.ConnectClose();
                plc.Dispose();
                plc = null;
            }
            else if (Device is MelsecMcNet)
            {
                MelsecMcNet plc = (MelsecMcNet)Device;
                plc.ConnectClose();
                plc.Dispose();
                plc = null;
            }
            else if (Device is MelsecMcAsciiNet)
            {
                MelsecMcAsciiNet plc = (MelsecMcAsciiNet)Device;
                plc.ConnectClose();
                plc.Dispose();
                plc = null;
            }
            else if (Device is MelsecA1ENet)
            {
                MelsecA1ENet plc = (MelsecA1ENet)Device;
                plc.ConnectClose();
                plc.Dispose();
            }
            else if (Device is MelsecA1EAsciiNet)
            {
                MelsecA1EAsciiNet plc = (MelsecA1EAsciiNet)Device;
                plc.ConnectClose();
                plc.Dispose();
            }
            else if (Device is PanasonicMewtocolOverTcp)
            {
                PanasonicMewtocolOverTcp plc = (PanasonicMewtocolOverTcp)Device;
                plc.ConnectClose();
                plc.Dispose();
                plc = null;
            }
            Device = null;
        }

        /// <summary>
        /// 按地址读取指定类型
        /// </summary>
        /// <typeparam name="T">类型 数组时需要加[]</typeparam>
        /// <param name="address">索引偏移</param>
        /// <param name="type"></param>
        /// <param name="length">读取长度,指16位寄存器的长度</param>
        /// <returns></returns>
        public override object Read<T>(string address, ref string Msg, ushort length = 1)
        {
            try
            {
                Type dataType = typeof(T);
                switch (dataType.ToString())
                {
                    case "System.Byte":
                        {
                            OperateResult<byte[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.Read(address, 1);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Int16":
                        {
                            OperateResult<short[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadInt16(address, 1);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Int32":
                        {
                            OperateResult<int[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadInt32(address, 1);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Int64":
                        {
                            OperateResult<long[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadInt64(address, 1);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Boolean":
                        {
                            OperateResult<bool[]> result;
                            if (Device is AllenBradleyNet)  //因为HSL DLL里面没有 ReadBool(address, length)
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = ((AllenBradleyNet)Device).ReadBoolArray(address); 
                                }
                                if(result.IsSuccess)
                                {
                                    IsConnect = true;
                                    return result.Content;
                                }
                                else
                                {
                                    Msg = result.ToMessageShowString();
                                    IsConnect = false;
                                    return null;
                                }
                            }
                            else if (Device is OmronCipNet)  //因为HSL DLL里面没有 ReadBool(address, length)
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = ((OmronCipNet)Device).ReadBoolArray(address);
                                }
                                if (result.IsSuccess)
                                {
                                    IsConnect = true;
                                    return result.Content;
                                }
                                else
                                {
                                    Msg = result.ToMessageShowString();
                                    IsConnect = false;
                                    return null;
                                }
                            }
                            else
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = Device.ReadBool(address, 1);
                                }
                                if (result.IsSuccess)
                                {
                                    IsConnect = true;
                                    return result.Content;
                                }
                                else
                                {
                                    Msg = result.ToMessageShowString();
                                    IsConnect = false;
                                    return null;
                                }
                            }
                        }
                    case "System.Single":
                        {
                            OperateResult<float[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadFloat(address, 1);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Double":
                        {
                            OperateResult<double[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadDouble(address, 1);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Int16[]":
                        {
                            OperateResult<short[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadInt16(address, length);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Int32[]":
                        {
                            OperateResult<int[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadInt32(address, length);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Boolean[]":
                        {
                            OperateResult<bool[]> result;
                            if (Device is AllenBradleyNet)  //因为HSL DLL里面没有 ReadBool(address, length)
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = ((AllenBradleyNet)Device).ReadBoolArray(address);
                                }
                                if (result.IsSuccess)
                                {
                                    IsConnect = true;
                                    return result.Content;
                                }
                                else
                                {
                                    Msg = result.ToMessageShowString();
                                    IsConnect = false;
                                    return null;
                                }
                            }
                            else if (Device is OmronCipNet)  //因为HSL DLL里面没有 ReadBool(address, length)
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = ((OmronCipNet)Device).ReadBoolArray(address);
                                }
                                if (result.IsSuccess)
                                {
                                    IsConnect = true;
                                    return result.Content;
                                }
                                else
                                {
                                    Msg = result.ToMessageShowString();
                                    IsConnect = false;
                                    return null;
                                }
                            }
                            else
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = Device.ReadBool(address, length);
                                }
                                if (result.IsSuccess)
                                {
                                    IsConnect = true;
                                    return result.Content;
                                }
                                else
                                {
                                    Msg = result.ToMessageShowString();
                                    IsConnect = false;
                                    return null;
                                }
                            }
                        }
                    case "System.Single[]":
                        {
                            OperateResult<float[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadFloat(address, length);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Double[]":
                        {
                            OperateResult<double[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadDouble(address, length);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.Byte[]":
                        {
                            OperateResult<byte[]> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.Read(address, length);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    case "System.String":
                        {
                            OperateResult<string> result;
                            lock (CommunicationObjLock)
                            {
                                result = Device.ReadString(address, length, Encoding.ASCII);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return result.Content.Split('\0')[0].Trim();
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return null;
                            }
                        }
                    default:
                        {
                            Msg = $"{dataType.ToString()} error!";
                            return null;
                        }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                return null;
            }
        }

        /// <summary>
        /// 按地址写入字节数组
        /// </summary>
        /// <param name="address">索引偏移</param>
        /// <param name="WriteValue">字节数组object类型</param>
        /// <param name="type">数据类型</param>
        /// <param name="Msg">输出信息</param>
        /// <returns></returns>
        public override bool Write<T>(string address, T value, ref string Msg)
        {
            try
            {
                Type dataType = value.GetType();
                OperateResult result;
                switch (dataType.ToString())
                {
                    case "System.Byte":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (byte)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Int16":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (short)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Int32":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (int)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Int64":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (long)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Boolean":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (bool)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Single":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (float)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Double":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (double)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.String":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (string)(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Int16[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (short[])(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Int32[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (int[])(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Int64[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (long[])(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Boolean[]":
                        {
                            if (Device is AllenBradleyNet || Device is OmronCipNet)  //因为HSL DLL里面没有 ReadBool(address, length)
                            {
                                List<byte> lst = new List<byte>();
                                foreach (var v in (bool[])(object)value)
                                {
                                    lst.AddRange(v ? new byte[] { 0xFF, 0xFF } : new byte[] { 0x00, 0x00 });
                                }
                                lock (CommunicationObjLock)
                                {
                                    result = ((AllenBradleyNet)Device).WriteTag(address, AllenBradleyHelper.CIP_Type_Bool, lst.ToArray(), lst.Count());
                                }
                            }
                            else
                            {
                                lock (CommunicationObjLock)
                                {
                                    result = Device.Write(address, (bool[])(object)value);
                                }
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Single[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (float[])(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Double[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (double[])(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    case "System.Byte[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                result = Device.Write(address, (byte[])(object)value);
                            }
                            if (result.IsSuccess)
                            {
                                IsConnect = true;
                                return true;
                            }
                            else
                            {
                                Msg = result.ToMessageShowString();
                                IsConnect = false;
                                return false;
                            }
                        }
                    default:
                        {
                            Msg = $"{dataType.ToString()} error!!";
                            return false;
                        }
                }
                
            }
            catch (Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                return false;
            }
        }

        private bool[] ReadBool(string address, ref string Msg, ushort length = 1)
        {
            OperateResult<bool[]> result;
            if (Device is AllenBradleyNet)  //因为HSL DLL里面没有 ReadBool(address, length)
            {
                lock (CommunicationObjLock)
                {
                    result = ((AllenBradleyNet)Device).ReadBoolArray(address);
                }
                if (result.IsSuccess)
                {
                    IsConnect = true;
                    return result.Content;
                }
                else
                {
                    Msg = result.ToMessageShowString();
                    IsConnect = false;
                    return null;
                }
            }
            else if (Device is OmronCipNet)  //因为HSL DLL里面没有 ReadBool(address, length)
            {
                lock (CommunicationObjLock)
                {
                    result = ((OmronCipNet)Device).ReadBoolArray(address);
                }
                if (result.IsSuccess)
                {
                    IsConnect = true;
                    return result.Content;
                }
                else
                {
                    Msg = result.ToMessageShowString();
                    IsConnect = false;
                    return null;
                }
            }
            else
            {
                lock (CommunicationObjLock)
                {
                    result = Device.ReadBool(address, length);
                }
                if (result.IsSuccess)
                {
                    IsConnect = true;
                    return result.Content;
                }
                else
                {
                    Msg = result.ToMessageShowString();
                    IsConnect = false;
                    return null;
                }
            }
        }
    }
}
