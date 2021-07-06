using ATL.Common;
using ATL.Core;
using HslCommunication.Profinet.Melsec;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Panasonic;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ATL.PLC;
using System.Threading;

namespace ATL.Engine
{
    public class InitPLC
    {
        public static void InitDevices()
        {
            //DisconnectAndDisposeDevice();
            
            foreach (var plcConfig in PlcConfigurationInfo.lstPlcConfiguration.Where(x => x.Enabled == 1))
            {
                AddDevices(plcConfig);
            }
            foreach(var v in ID_Device.lstDevices)
            {
                Thread t = new Thread(v.PingPLC);
                t.Start();
            }
        }
        
        private static void AddDevices(PlcConfigurationInfo plcConfig)
        {
            switch (plcConfig.ProtocolName)
            {
                #region
                case "S7Net_1200":
                    {
                        SiemensS7Net plc = new SiemensS7Net(SiemensPLCS.S1200, plcConfig.Address);
                        plc.Rack = 0;
                        plc.Slot = 0;
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "S7Net_1500":
                    {
                        SiemensS7Net plc = new SiemensS7Net(SiemensPLCS.S1500, plcConfig.Address);
                        plc.Rack = 0;
                        plc.Slot = 0;
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "S7Net_300":
                    {
                        SiemensS7Net plc = new SiemensS7Net(SiemensPLCS.S300, plcConfig.Address);
                        plc.Rack = 0;
                        plc.Slot = 2;
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "S7Net_400":
                    {
                        SiemensS7Net plc = new SiemensS7Net(SiemensPLCS.S400, plcConfig.Address);
                        plc.Rack = 0;
                        plc.Slot = 3;
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "S7Net_200":
                    {
                        SiemensS7Net plc = new SiemensS7Net(SiemensPLCS.S200, plcConfig.Address);
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "S7Net_200Smart":
                    {
                        SiemensS7Net plc = new SiemensS7Net(SiemensPLCS.S200Smart, plcConfig.Address);
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "Fins_Tcp":
                    {
                        int port;
                        byte SA1, DA1;
                        if (plcConfig.AddressPara.Split(',').Count() == 3)
                        {
                            port = Convert.ToInt32(plcConfig.AddressPara.Split(',')[0]);
                            SA1 = byte.Parse(plcConfig.AddressPara.Split(',')[1]);
                            DA1 = byte.Parse(plcConfig.AddressPara.Split(',')[2]);
                        }
                        else
                        {
                            port = Convert.ToInt32(plcConfig.AddressPara);
                            SA1 = 0;//上位机的节点地址，假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13
                            DA1 = 110;//PLC的节点地址，这个值在配置了ip地址之后是默认赋值的，默认为Ip地址的最后一位
                        }
                        //实例化OmronFinsNet对象的时候HSL里自动 DA1 = Convert.ToByte( plcConfig.Address.Substring( plcConfig.Address.LastIndexOf( "." ) + 1 ) );
                        HslCommunication.Profinet.Omron.OmronFinsNet plc = new HslCommunication.Profinet.Omron.OmronFinsNet(plcConfig.Address, port);
                        //OmronFinsNet对象连接PLC的时候会自动分配SA1
                        plc.SA1 = SA1;//上位机的节点地址，假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13
                        plc.DA1 = DA1;//PLC的节点地址，这个值在配置了ip地址之后是默认赋值的，默认为Ip地址的最后一位
                        plc.ByteTransform.IsStringReverseByteWord = false;  //用于字符串颠倒
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "MC_3E_Binary":
                    {
                        MelsecMcNet plc = new MelsecMcNet(plcConfig.Address, Convert.ToInt32(plcConfig.AddressPara));
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "MC_3E_Ascii":
                    {
                        MelsecMcAsciiNet plc = new MelsecMcAsciiNet(plcConfig.Address, Convert.ToInt32(plcConfig.AddressPara));
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "A_1ENet_Binary":
                    {
                        MelsecA1ENet plc = new MelsecA1ENet(plcConfig.Address, Convert.ToInt32(plcConfig.AddressPara));
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "A_1ENet_Ascii":
                    {
                        MelsecA1EAsciiNet plc = new MelsecA1EAsciiNet(plcConfig.Address, Convert.ToInt32(plcConfig.AddressPara));
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "MewtocolOverTcp":
                    {
                        PanasonicMewtocolOverTcp plc = new PanasonicMewtocolOverTcp(plcConfig.Address, Convert.ToInt32(plcConfig.AddressPara));  //可能需要设置站号
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "CIP":
                    {
                        OmronCipNet plc = new OmronCipNet(plcConfig.Address, Convert.ToInt32(plcConfig.AddressPara));
                        plcConfig.PLC = new HslPLC(plc);
                    }
                    break;
                case "ADS":
                    {
                        plcConfig.PLC = new AdsPlc_tag();
                        plcConfig.PLC.ip = plcConfig.Address;
                        int port = 0;
                        if (int.TryParse(plcConfig.AddressPara, out port))
                        {
                            plcConfig.PLC.port = port;
                            //device.plc.OnConnectedChanged += device.OnConnectedChanged;
                        }
                        else
                        {
                            MessageBox.Show("BeckhoffTwinCAT PLC Port Setting Error");
                        }
                    }
                    break;
                case "ADS_D":
                    {
                        plcConfig.PLC = new AdsPlc_D();
                        plcConfig.PLC.ip = plcConfig.Address;
                        int port = 0;
                        if (int.TryParse(plcConfig.AddressPara, out port))
                        {
                            plcConfig.PLC.port = port;
                            //device.plc.OnConnectedChanged += device.OnConnectedChanged;
                        }
                        else
                        {
                            MessageBox.Show("BeckhoffTwinCAT PLC Port Setting Error");
                        }
                    }
                    break;
                    #endregion
            }
            ID_Device device = new ID_Device();
            device.PlcConfig = plcConfig;
            ID_Device.lstDevices.Add(device);
        }
    }
}
