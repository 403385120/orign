using ATL.Common;
using HslCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace ATL.PLC
{
    public class AdsPlc_tag: PLCClientBase
    {
        public AdsPlc_tag()
        {
            AddressWordLength = 2;
            _ByteTransform = new RegularByteTransform();
        }

        public TcAdsClient adsClient;
        public override bool Connect(ref string msg)
        {
            try
            {
                if (adsClient == null)
                {
                    adsClient = new TcAdsClient();
                }
                //adsClient.Connect(801);            // TW2是801 
                //连接PLC的IP地址和端口号
                //adsClient.Connect("192.168.100.30.1.1", 801);
                
                adsClient.Connect(ip, port);
                Thread.Sleep(1000);
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
                IsConnect = false;
                LogInDB.Error(ex.ToString());
            }
            if (!IsConnect)
            {
                //LogInDB.Error("连接倍福PLC失败:" + ex.Message);
                if (ATL.Common.StringResources.IsDefaultLanguage)
                {
                    LogInDB.Error("连接倍福PLC失败:");
                }
                else
                {
                    LogInDB.Error("Failed to connect BeckhoffTwinCAT");
                }
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
            catch (Exception)
            {
                return false;
            }
        }
        
        ~AdsPlc_tag()
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

        private const long BeckhoffBeginPos = 0x4020;
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

        public override object Read<T>(string tagName, ref string Msg, ushort length = 1)
        {
            object obj = null;
            int hvar1 = new int();
            try
            {
                Type dataType = typeof(T);
                hvar1 = adsClient.CreateVariableHandle(tagName);
                switch (dataType.ToString())
                {
                    case "System.Int16":
                    case "System.Int16[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(hvar1, typeof(short[]), new int[] { length });
                            }
                            break;
                        }
                    case "System.Int32":
                    case "System.Int32[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(hvar1, typeof(int[]), new int[] { length });
                            }
                            break;
                        }
                    case "System.Int64":
                    case "System.Int64[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(hvar1, typeof(long[]), new int[] { length });
                            }
                            break;
                        }
                    case "System.Boolean":
                    case "System.Boolean[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(hvar1, typeof(bool[]));
                            }
                            break;
                        }
                    case "System.Single":
                    case "System.Single[]":
                    case "System.Double":
                    case "System.Double[]":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(hvar1, typeof(T), new int[] { length });
                            }
                            break;
                        }
                    case "System.String":
                        {
                            lock (CommunicationObjLock)
                            {
                                obj = adsClient.ReadAny(hvar1, typeof(T), new int[] { length + 1 });
                            }
                            break;
                        }
                    default:
                        {
                            LogInDB.Error($"[{dataType.ToString()}] error!!");
                            return false;
                        }
                }
                adsClient.DeleteVariableHandle(hvar1);
            }
            catch(Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
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

        public override bool Write<T>(string tagName, T value, ref string Msg)
        {
            int hvar1 = new int();
            try
            {
                lock (CommunicationObjLock)
                {
                    hvar1 = adsClient.CreateVariableHandle(tagName);
                    adsClient.WriteAny(hvar1, (T)value);
                    adsClient.DeleteVariableHandle(hvar1);
                }
                IsConnect = true;
                return true;
            }
            catch (Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                IsConnect = false;
                return false;
            }
        }

        private bool[] ReadBool(string address, ref string Msg, ushort length = 1)
        {
            bool[] obj = null;
            int hvar1 = new int();
            try
            {
                lock (CommunicationObjLock)
                {
                    hvar1 = adsClient.CreateVariableHandle(address);
                    obj = (bool[])adsClient.ReadAny(hvar1, typeof(bool[]));
                    adsClient.DeleteVariableHandle(hvar1);
                }
                IsConnect = true;
            }
            catch (Exception ex)
            {
                Msg = ex.Message + Environment.NewLine + ex.StackTrace;
                IsConnect = false;
            }
            return obj;
        }
    }
}
