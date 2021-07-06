using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Instrument
{
    public class LR8401
    {
        private Socket _lR8401Socket;
        private int _cannelCount = 1;

        public bool InitialLR8401(string ip, int port, int cannelCount = 1)
        {
            _cannelCount = cannelCount;
            _lR8401Socket = Tool.InitSocketClient(_lR8401Socket, ip, port);
            if (_lR8401Socket == null)
            {
                return false;
            }
            return true;
        }

        public bool StartTest()
        {
            try
            {
                ClearMemory();
                string cmd = ":STARt\r\n";
                _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetData()
        {
            if (_lR8401Socket == null)
            {
                return "9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999,9999";
            }
            try
            {
                string cmd = ":MEMory:VDATa? 6\r\n";
                _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                byte[] buffer = new byte[1024];
                int r = _lR8401Socket.Receive(buffer, buffer.Length, SocketFlags.None);
                string data = Encoding.Default.GetString(buffer, 0, r);
                return data;
            }
            catch (Exception ex)
            {
                return "999";
            }
        }

        public bool Initial()
        {
            try
            {
                string cmd = ":UNIT:FILTer 60HZ\r\n";
                _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                cmd = ":CONFigure:SAMPle 0.1\r\n";
                _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                cmd = ":CONFigure: RECTime 0,0,0,2\r\n";
                _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));

                for (int i = 0; i < _cannelCount; i++)
                {
                    cmd = ":UNIT:RANGe ch1_" + (i + 1) + ",10\r\n";
                    _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                    cmd = ":UNIT:STORe ch1_" + (i + 1) + ",ON\r\n";
                    _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool ClearMemory()
        {
            try
            {
                for (int i = 0; i < _cannelCount; i++)
                {
                    string cmd = ":MEMory:POINt ch1_" + (i + 1) + ",0\r\n";
                    _lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
