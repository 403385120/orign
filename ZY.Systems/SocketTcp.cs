using SuperCom.DataCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
    public partial class SocketTcp : SocketTcpAbs
    {
        /// <summary>
        /// Modbus TCP 计数通讯
        /// </summary>
        public ModbusTcpNetInterface sokectModbusCount { get; set; }

        /// <summary>
        /// 网络通讯对象 计数通讯
        /// </summary>
        public SocketTCPInterface socketTcpCount { get; set; }
        /// <summary>
        /// 倍福通讯对象 计数通讯
        /// </summary>
        public TwinCATInterface socketAdsCount { get; set; }


        /// <summary>
        /// Ormon Cip 计数通讯
        /// </summary>
        public OmronCipNetInterface sokectCipNetCount { get; set; }


        /// <summary>
        /// Modbus TCP 报警通讯
        /// </summary>
        public ModbusTcpNetInterface sokectModbusAlarm { get; set; }

        /// <summary>
        /// Ormon Cip 报警通讯
        /// </summary>
        public OmronCipNetInterface sokectCipNetAlarm { get; set; }


        /// <summary>
        /// 网络通讯对象 报警通讯
        /// </summary>
        public SocketTCPInterface socketTcpAlarm { get; set; }
        /// <summary>
        /// 倍福通讯对象 报警通讯
        /// </summary>
        public TwinCATInterface socketAdsAlarm { get; set; }


        /// <summary>
        /// 欧姆龙TCP 报警 通讯对象 
        /// </summary>
        public OmronFinsTcpInterface socketFinsTcpAlarm { get; set; }

        /// <summary>
        /// 欧姆龙Udp 报警通讯对象
        /// </summary>
        public OmronFinsUdpInterface socketFinsUdpAlarm { get; set; }

        /// <summary>
        /// 三菱 报警通讯对象
        /// </summary>
        public MelsecMcInterface socketMcAlarm { get; set; }

        /// <summary>
        /// 欧姆龙TCP 计数通讯对象
        /// </summary>
        public OmronFinsTcpInterface socketFinsTcpCount { get; set; }

        /// <summary>
        /// 欧姆龙Udp 计数通讯对象
        /// </summary>
        public OmronFinsUdpInterface socketFinsUdpCount { get; set; }

        /// <summary>
        /// 三菱通讯 计数对象
        /// </summary>
        public MelsecMcInterface socketMcCount { get; set; }

        /// <summary>
        /// 西门子PLC 计数对象
        /// </summary>
        public SiemensInterface socketSiemensCount { get; set; }

        /// <summary>
        /// 西门子PLC 报警通讯对象
        /// </summary>
        public SiemensInterface socketSiemensAlarm { get; set; }



        public string Name { get; set; }
        /// <summary>
        /// 上次连接状态
        /// </summary>
        public bool IsPevConnected { get; set; }

        /// <summary>
        /// 欧姆龙 NX CIP计数通讯对象
        /// </summary>
        public NXCIPInterface socketNxCipCount { get; set; }
        /// <summary>
        /// 欧姆龙 NX CIP报警通讯对象
        /// </summary>

        public NXCIPInterface sockettNxCipAlarm { get; set; }

        public int MachineID { get; set; }

    }
}
