using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Instrument
{
    public class Toprie
    {
        private Socket _toprieSocket;

        public bool InitialToprie(string ip, int port)
        {
            _toprieSocket = Tool.InitSocketClient(_toprieSocket, ip, port);
            if (_toprieSocket == null)
            {
                return false;
            }
            return true;
        }

        ~Toprie()
        {
            _toprieSocket.Close();
        }
        public float GetTemperatrue()
        {
            if (_toprieSocket == null)
            {
                return 9999;
            }
            try
            {
                if (_toprieSocket != null)
                {
                    byte[] buffer = new byte[14];
                    buffer[0] = 0x00;
                    buffer[1] = 0x01;
                    buffer[2] = 0x00;
                    buffer[3] = 0x00;
                    buffer[4] = 0x00;
                    buffer[5] = 0x06;
                    buffer[6] = 0x01;
                    buffer[7] = 0x03;
                    buffer[8] = 0x00;
                    buffer[9] = 0x00;
                    buffer[10] = 0x00;
                    buffer[11] = 0x04;
                    buffer[12] = 0x42;
                    buffer[13] = 0xEC;

                    _toprieSocket.Send(buffer);
                    Thread.Sleep(100);
                    byte[] receive = new byte[100];
                    int bufferLenght = _toprieSocket.Receive(receive);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < bufferLenght; i++)
                    {
                        sb.AppendFormat("{0:x2}" + " ", receive[i]);
                    }
                    string strData = sb.ToString().Replace(" ", "");
                    strData = strData.Substring(18, 8);
                    MatchCollection matches = Regex.Matches(strData, @"[0-9A-Fa-f]{2}");
                    byte[] bytes = new byte[matches.Count];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = byte.Parse(matches[i].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    float temperatrue = BitConverter.ToSingle(bytes.Reverse().ToArray(), 0);
                    return temperatrue;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 999;
            }
        }
    }
}

