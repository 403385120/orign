using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceLib;

namespace WpfApp1
{
    public class ModVariable
    {
   
        //public static DeviceMainPage deviceMainPage = null;
  
        //public static frmTCPDevice frmCellBarcode;

        public static DeviceProcess deviceoverview;
        public static DevicePage devicepage = null;
        public static DeviceSamplePage samplepage = null;
        public static DeviceRunPage runpage = null;
        public static DeviceInitPage initpage = null;

        public static DeviceProcess deviceOverView
        {
            get
            {
                if (ModVariable.deviceoverview == null)
                {
                    ModVariable.deviceoverview = DeviceProcess.getInstanse();

                }
                return ModVariable.deviceoverview;
            }
        }

        public static DeviceInitPage initPage
        {
            get
            {
                if (ModVariable.initpage == null)
                {
                    ModVariable.initpage = new DeviceInitPage();

                }
                return ModVariable.initpage;
            }
        }
        public static DevicePage devicePage
        {
            get
            {
                if (ModVariable.devicepage == null)
                {
                    ModVariable.devicepage = new DevicePage();

                }
                return ModVariable.devicepage;
            }
        }

        public static DeviceSamplePage samplePage
        {
            get
            {
                if (ModVariable.samplepage == null)
                {
                    ModVariable.samplepage = new DeviceSamplePage();

                }
                return ModVariable.samplepage;
            }
        }

        public static DeviceRunPage runPage
        {
            get
            {
                if (ModVariable.runpage == null)
                {
                    ModVariable.runpage = new DeviceRunPage();

                }
                return ModVariable.runpage;
            }
        }


       
    }
}
