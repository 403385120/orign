using SuperCom.DataCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
    public abstract class SocketTcpAbs
    {

        /// <summary>
        /// 通讯类型
        /// </summary>
        public Common.EnumCommunicationType commType { get; set; }
        /// <summary>
        /// Socket网络通讯类型
        /// </summary>
        public Common.EnumSocketTcpType keyTcp { get; set; }
        /// <summary>
        /// 网络通讯对象
        /// </summary>
        public SocketTCPInterface socketTcp { get; set; }
        /// <summary>
        /// 倍福通讯对象
        /// </summary>
        public TwinCATInterface socketAds { get; set; }


        public OmronCipNetInterface socketCipNet { get; set; }


        /// <summary>
        /// Modbus TCP
        /// </summary>
        public ModbusTcpNetInterface socketModbus { get; set; }

        /// <summary>
        /// 欧姆龙TCP通讯对象
        /// </summary>
        public OmronFinsTcpInterface socketFinsTcp { get; set; }

        /// <summary>
        /// 欧姆龙Udp通讯对象
        /// </summary>
        public OmronFinsUdpInterface socketFinsUdp { get; set; }

        /// <summary>
        /// 串口通讯
        /// </summary>
        public SerialPortInterface serialPort { get; set; }

        /// <summary>
        /// 三菱通讯对象
        /// </summary>
        public MelsecMcInterface socketMc { get; set; }

        /// <summary>
        /// 西门子PLC
        /// </summary>
        public SiemensInterface socketSiemens { get; set; }
        /// <summary>
        /// UDP服务端
        /// </summary>

        public SocketUdpServer socketUpdServer { get; set; }

        /// <summary>
        /// TCP服务端
        /// </summary>
        public SocketTcpServer socketTcpServer { get; set; }


        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// 连接超时次数
        /// </summary>
        public int TimeOutCount { get; set; }

        /// <summary>
        /// 是否需要连接
        /// </summary>
        public bool IsNeedConnect { get; set; }
        /// <summary>
        /// 上次连接状态
        /// </summary>
        public bool IsBeforeConnect { get; set; }

        /// <summary>
        /// PLC数据计数、报警通讯对象
        /// </summary>
        public SocketTcp SocketTcpCountAlarm { get; set; }
        /// <summary>
        /// 欧姆龙NX CIP 通讯对象
        /// </summary>
        public NXCIPInterface socketNxCip { get; set; }


    }
}
