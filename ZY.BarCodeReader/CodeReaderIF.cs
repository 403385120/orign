using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.SerialDevice;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows;

namespace ZY.BarCodeReader
{
    public class CodeReaderIF
    {
        #region   串口通信
        /// <summary>
        /// 串口的参数应由外部配置得到
        /// </summary>
        private static CodeReaderConfig _codeReaderConfig = new CodeReaderConfig(new SerialDeviceConfig("COM1", 115200, 8, 0, 2));
        private static ICodeReader _codeReader= new KeyenceBarCodeReaderSerial(_codeReaderConfig.SerialConfig);

        public CodeReaderConfig MyCodeReaderConfig
        {
            get { return _codeReaderConfig; }
        }

        /// <summary>
        /// 端口列表
        /// </summary>
        public static List<string> PortList
        {
            get
            {
                return _codeReader.PortList;
            }
        }

        /// <summary>
        /// 初始化，将串口打开
        /// </summary>
        /// <param name="_SerialDeviceConfig">
        /// 串口号：COM3-COM6按实际情况
        /// 波特率：默认115200
        /// 数据位：默认8
        /// 停止为：0
        /// 奇偶位：偶
        /// </param>
        /// <returns></returns>
        public static bool Init(CodeReaderConfig readerConfig)
        {
            _codeReaderConfig = readerConfig;

            _codeReader = CodeReaderFactory.CreateCodeReader(_codeReaderConfig);
            _codeReader.Open();
          
            return true;
        }

        /// <summary>
        /// 关闭串口通信
        /// </summary>
        /// <returns></returns>
        public static bool UnInit()
        {
            _codeReader.Close();

            return true;
        }

        /// <summary>
        /// 使用新的端口重新打开
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool ReOpen(string portName)
        {
            _codeReaderConfig.SerialConfig.PortName = portName;

            return _codeReader.Open();
        }

        /// <summary>
        /// 读取二维码(注意此函数会吃掉异常)
        /// </summary>
        /// <param name="code"> 将存储的二维码放在code里面</param>
        /// <returns></returns>
        public static bool ReadBarCode(ref string code)
        {
            try
            {
                _codeReader.Read(ref code);
            }
            catch { }

            return false;
        }
        /// <summary>
        /// 扫码枪是否打开(注意此函数会吃掉异常)
        /// </summary>
        /// <param name="code"> 将存储的二维码放在code里面</param>
        /// <returns></returns>
        public static bool IsOpen()
        {
             return _codeReader.IsOpen();
        }

        #endregion

        #region   Socket通信
        static Socket socketClient_ScanBarcode1 = null;
        static Socket socketClient_ScanBarcode2 = null;
        static Socket socketClient_Dimension = null;

        private static object objLockSend = new object();

        /// <summary>
        /// 发送信息到服务端
        /// </summary>     
        public static bool ClientSendMsg(string sendMsg,int type)
        {
            lock (objLockSend)
            {
                //将输入的内容字符串转换为机器可以识别的字节数组
                byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
                //调用客户端套接字发送字节数组
                if (type == 1) //扫码
                {
                    socketClient_ScanBarcode1.Send(arrClientSendMsg);
                }
                else if (type == 3)
                {
                    socketClient_ScanBarcode2.Send(arrClientSendMsg);
                }
                else //尺寸测量
                {
                    socketClient_Dimension.Send(arrClientSendMsg);
                }

                return true;
            }
        }

        private static object objLockReceive = new object();

        /// <summary>
        /// 接收服务端发来的消息
        /// </summary>     
        public static string ClientReceiveMsg(int type)
        {
            lock (objLockReceive)
            {
                //定义一个内存缓冲区，用于临时存储接收到的消息
                byte[] arrRecvMsg = new byte[1024];
                //将客户端套接字接收到的数据存入到内存缓冲区中，并获取长度
                int length = 0;
                if (type == 1)
                {
                    length = socketClient_ScanBarcode1.Receive(arrRecvMsg);
                }
                else if (type == 3)
                {
                    length = socketClient_ScanBarcode2.Receive(arrRecvMsg);
                }
                else
                {
                    length = socketClient_Dimension.Receive(arrRecvMsg);
                }
                //将套接字获取到的字符数组转换为可以看懂的字符串

                return Encoding.UTF8.GetString(arrRecvMsg, 0, length).Replace("\r", "");
            }
        }

        /// <summary>
        /// 初始化，与服务端建立连接
        /// </summary>   
        public static bool Init_ClientConnet(string ip,int port,int type)
        {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint point = new IPEndPoint(address, port);
            string str = string.Empty;
            try
            {
                if (type == 1)
                {
                    str = "扫码1";
                    socketClient_ScanBarcode1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketClient_ScanBarcode1.Connect(point);
                }
                else if (type == 3)
                {
                    str = "扫码2";
                    socketClient_ScanBarcode2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketClient_ScanBarcode2.Connect(point);
                }
                else
                {
                    str = "尺寸测量";
                    socketClient_Dimension = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketClient_Dimension.Connect(point);
                    socketClient_Dimension.ReceiveTimeout = 5000;
                }
                
                return true;
            }
            catch (Exception e)
            {
                //Console.WriteLine("与" + str + "连接出错:" + e.Message);
                System.Windows.Forms.MessageBox.Show("与"+str+"连接出错:" + e.Message);
                return false;
            }
            
        }

        /// <summary>
        /// 关闭Socket通讯
        /// </summary>
        public static bool UnInit_ClientConnet()
        {
            try
            {
                //释放资源
                socketClient_ScanBarcode1.Shutdown(SocketShutdown.Both);
                socketClient_ScanBarcode1.Close();
                socketClient_ScanBarcode2.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
        #endregion
    }
}
