using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Instrument
{
    public class ShangLingVision
    {
        private Socket _visionSocket;

        /// <summary>
        /// 初始化TCP客户端
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>
        /// <returns></returns>
        public bool InitialShangLingVision(string ip, int port)
        {
            _visionSocket = Tool.InitSocketClient(_visionSocket, ip, port);
            if (_visionSocket == null)
            {
                return false;
            }
            _visionSocket.SendTimeout = 2000;
            _visionSocket.ReceiveTimeout = 4000;
            return true;
        }

        ~ShangLingVision()
        {
            if (_visionSocket != null)
            {
                _visionSocket.Close();
            }
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="cmd">指令</param>
        /// <returns></returns>
        public bool SendCommand(string cmd)
        {
            byte[] buffer = Encoding.Default.GetBytes(cmd);
            _visionSocket.Send(buffer, 00, buffer.Length, SocketFlags.None);
            return true;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns>接收到的数据</returns>
        public string ReceiveData()
        {
            byte[] buffer = new byte[1024];
            int lenght = _visionSocket.Receive(buffer);
            string strData = Encoding.Default.GetString(buffer, 0, lenght);
            return strData;
        }
    }
}
