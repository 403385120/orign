using Ivi.Visa.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Logging;

namespace Instrument
{
    public class TCP34461A
    {
        private string _ip;
        private Ivi.Visa.Interop.FormattedIO488 myDmm;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ip">仪表IP地址</param>
        public TCP34461A(string ip)
        {
            try
            {
                _ip = ip;
                //string DutAddr = "TCPIP0::192.168.180.2";
                string DutAddr = "TCPIP0::" + _ip;
                Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager(); //Open up a new resource manager
                myDmm = new Ivi.Visa.Interop.FormattedIO488(); //Open a new Formatted IO 488 session 
                myDmm.IO = (IMessage)rm.Open(DutAddr, AccessMode.NO_LOCK, 500, ""); //Open up a handle to the DMM with a 2 second timeout
                myDmm.IO.Timeout = 3000; //You can also set your timeout by doing this command, sets to 3 seconds

                //First start off with a reset state
                myDmm.IO.Clear(); //Send a device clear first to stop any measurements in process

            }
            catch (Exception ex)
            {
                LoggingIF.Log("电压表初始化异常：" + ex, LogLevels.Error, "TCP34461A");
            }
        }

        public double Get34461AVoltage()
        {            
            //return "3.9";
            try
            {
                double data = 0;
                for (int i = 0; i < 3; i++)
                {

                    //myDmm.WriteString("CONF:VOLT:DC 10, 0.0001", true);
                    myDmm.WriteString("READ?", true);
                    string DCVResult = myDmm.ReadString();
                    DCVResult = DCVResult.Replace("\n", "");
                    data = Math.Abs(Convert.ToDouble(DCVResult));
                    if (data > 2 && data < 5)
                    {
                        break;
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                LoggingIF.Log("电压表异常：" + ex, LogLevels.Error, "Get34461AVoltage");
                return 999;
            }

        }
    }
}
