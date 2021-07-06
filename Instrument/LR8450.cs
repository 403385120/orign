using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Instrument
{
    public class LR8450
    {
        private Socket _lR8450Socket;

        public bool InitialLR8450(string ip, int port)
        {
            _lR8450Socket = Tool.InitSocketClient(_lR8450Socket, ip, port);
            if (_lR8450Socket == null)
            {
                return false;
            }
            return true;
        }

        public bool StartTest()
        {
            try
            {
                //ClearMemory();
                //string cmd = ":STARt\r\n";
                //_lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                string cmd = ":TRIG:MANU\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                //Thread.Sleep(2000);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetData(int channel)
        {
            try
            {
                //string cmd = ":MEMory:VDATa? 6\r\n";
                //_lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                //byte[] buffer = new byte[1024];
                //int r = _lR8401Socket.Receive(buffer, 1024, SocketFlags.None);
                //string data = Encoding.Default.GetString(buffer, 0, r);
                string cmd = ":MEM:POIN CH2_" + channel + ",0;VDATa? 6\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                byte[] buffer = new byte[1024];
                //Thread.Sleep(100);
                int r = _lR8450Socket.Receive(buffer, 1024, SocketFlags.None);
                string data = Encoding.Default.GetString(buffer, 0, r);
                return data.Trim();
            }
            catch (Exception ex)
            {
                return "999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999,999";
                //return ex.Message;
            }
        }

        public bool Initial()
        {
            try
            {
                //string cmd = ":UNIT:FILTer 50HZ\r\n";
                //_lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                //cmd = ":CONFigure:SAMPle 0.2\r\n";
                //_lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                //cmd = ":CONFigure: RECTime 0,0,0,1\r\n";
                //_lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                //cmd = ":UNIT:RANGe CH2_1,10\r\n";
                //_lR8401Socket.Send(Encoding.Default.GetBytes(cmd));
                //cmd = ":UNIT:STORe CH2_1,ON\r\n";
                //_lR8401Socket.Send(Encoding.Default.GetBytes(cmd));

                string cmd = ":CONFigure:SAMPle 0.2\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                cmd = ":CONFigure:RECTime 0,0,0,1\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));

                cmd = ":TRIG:MODE REPE\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                cmd = ":TRIG:SET ON\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                cmd = ":TRIG:EXT:STAR:KIND ON\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));
                cmd = ":START\r\n";
                _lR8450Socket.Send(Encoding.Default.GetBytes(cmd));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
