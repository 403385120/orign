using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using HslCommunication.Core;
using ATL.Common;

namespace ATL.PLC
{
    public class AdsPlc_D: PLCClientBase
    {
        public AdsPlc_D()
        {
            AddressWordLength = 2;
            _ByteTransform = new RegularByteTransform();
        }
        private TcAdsClient adsClient;

        /// <summary>
        /// 倍福PLC读取起始位置
        /// </summary>
        private const long BeckhoffBeginPos = 0x4020;

        public override bool Connect(ref string msg)
        {
            try
            {
                if(adsClient == null)
                {
                    adsClient = new TcAdsClient();
                    ip = ip + ".1.1";
                }
                adsClient.Connect(ip, port);
                if (TestConnect())
                {
                    IsConnect = true;
                    //LogInDB.Error("连接倍福PLC成功");
                    if (ATL.Common.StringResources.IsDefaultLanguage)
                    {
                        LogInDB.Error("连接倍福PLC成功");
                    }
                    else
                    {
                        LogInDB.Error("Successfully connected to Beifu PLC ");
                    }
                    return true;
                }
                else
                {
                    IsConnect = false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            return false;
        }

        public override bool DisConnect()
        {
            try
            {
                adsClient.Disconnect();
                IsConnect = false;
                return true;
            }
            catch (Exception ex)
            {
                LogInDB.Error(ex.Message + Environment.NewLine + ex.StackTrace);
                return false;
            }
        }
        
        ~AdsPlc_D()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (adsClient != null)
            {
                if (adsClient.IsConnected)
                {
                    adsClient.Disconnect();
                }
                adsClient.Dispose();
                adsClient = null;
            }
            IsConnect = false;
        }
        private bool TestConnect()
        {
            try
            {
                short s = (short)adsClient.ReadAny(BeckhoffBeginPos, 0, typeof(short));
                IsConnect = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 按地址读取指定类型
        /// </summary>
        /// <typeparam name="T">类型 数组时需要加[]</typeparam>
        /// <param name="address">索引偏移</param>
        /// <param name="type"></param>
        /// <param name="length">读取长度,指16位寄存器的长度</param>
        /// <returns></returns>
        public override object Read<T>(string _address, ref string Msg, ushort length = 1)
        {
            try
            {
                string address = _address.Substring(1);

                long offset;
                long.TryParse(address, out offset);
                offset = offset * 2;
                object obj = null;
                Type dataType = typeof(T);
                switch (dataType.ToString())
                {
                    case "System.Byte":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Boolean":
                    case "System.Single":
                    case "System.Double":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType);
                            }
                        }
                        break;
                    case "System.Int16[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length });
                            }
                        }
                        break;
                    case "System.Int32[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length / 2 });
                            }
                        }
                        break;
                    case "System.Single[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length / 2 });
                            }
                        }
                        break;
                    case "System.Double[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length / 4 });
                            }
                        }
                        break;
                    case "System.Boolean[]":
                    case "System.Byte[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length * 2 });
                            }
                        }
                        break;
                    case "System.String":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length });
                            }
                        }
                        break;
                    case "System.String[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length });
                            }
                        }
                        break;
                    default:
                        {
                            Msg = $"{dataType.ToString()} error";
                        }
                        break;
                }
                if(obj == null)
                {
                    IsConnect = false;
                }
                else
                {
                    IsConnect = true;
                }
                return obj;
            }
            catch (Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                IsConnect = false;
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
        public override bool Write<T>(string address, T WriteValue, ref string Msg)
        {
            bool res = false;
            try
            {
                // 为兼容其他PLC协议，去掉地址位前面的第一个字符“D”
                address = address.Substring(1);
                long offset;
                long.TryParse(address, out offset);
                offset = offset * 2;
                int length = 0;
                Type dataType = typeof(T);
                byte[] arr = null;
                switch (dataType.ToString())
                {
                    case "System.Byte":
                        arr = BitConverter.GetBytes(Convert.ToByte(WriteValue));
                        break;
                    case "System.Int16":
                        arr = BitConverter.GetBytes(Convert.ToInt16(WriteValue));
                        break;
                    case "System.Int32":
                        arr = BitConverter.GetBytes(Convert.ToInt32(WriteValue));
                        break;
                    case "System.Int64":
                        arr = BitConverter.GetBytes(Convert.ToInt64(WriteValue));
                        break;
                    case "System.Boolean":
                        arr = BitConverter.GetBytes(Convert.ToBoolean(WriteValue));
                        break;
                    case "System.Single":
                        arr = BitConverter.GetBytes(Convert.ToSingle(WriteValue));
                        break;
                    case "System.Double":
                        arr = BitConverter.GetBytes(Convert.ToDouble(WriteValue));
                        break;
                    case "System.String":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue.ToString(), Encoding.ASCII);
                            length = (WriteValue as string).Length;
                        }
                        break;
                    case "System.Int16[]":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue as short[]);
                            length = (WriteValue as short[]).Length;
                        }
                        break;
                    case "System.Int32[]":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue as int[]);
                            length = (WriteValue as int[]).Length;
                        }
                        break;
                    case "System.Int64[]":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue as long[]);
                            length = (WriteValue as long[]).Length;
                        }
                        break;
                    case "System.Boolean[]":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue as bool[]);
                            length = (WriteValue as double[]).Length;
                        }
                        break;
                    case "System.Single[]":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue as float[]);
                            length = (WriteValue as double[]).Length;
                        }
                        break;
                    case "System.Double[]":
                        {
                            ByteTransformBase transformBase = new ByteTransformBase();
                            arr = transformBase.TransByte(WriteValue as double[]);
                            length = (WriteValue as double[]).Length;
                        }
                        break;
                    case "System.Byte[]":
                        {
                            arr = WriteValue as byte[];
                            length = arr.Length;
                        }
                        break;
                    default:
                        break;
                }

                using (AdsStream ds = new AdsStream(arr.Length))
                {
                    using (BinaryWriter bw = new BinaryWriter(ds))
                    {
                        try
                        {
                            lock (CommunicationObjLock)
                            {
                                //将数据写入数据流          
                                ds.Position = 0;
                                bw.Write(arr, 0, arr.Length);
                                adsClient.Write(BeckhoffBeginPos, offset, ds);
                                res = true;
                                IsConnect = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                            IsConnect = false;
                            res = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                res = false;
            }
            return res;
        }

        private bool[] ReadBool(string address, ref string Msg, ushort length = 1)
        {
            try
            {
                address = address.Substring(1);
                long offset;
                long.TryParse(address, out offset);
                offset = offset * 2;
                Type dataType = typeof(bool[]);
                object obj;
                lock (CommunicationObjLock)
                {
                    obj = adsClient.ReadAny(BeckhoffBeginPos, offset, dataType, new int[] { length * 2 });
                }
                if(obj != null)
                {
                    IsConnect = true;
                    return (bool[])obj;
                }
                else
                {
                    Msg = $"Read [{address}] error";
                    IsConnect = false;
                    return null;
                }
            }
            catch(Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                return null;
            }
        }
    }
}
