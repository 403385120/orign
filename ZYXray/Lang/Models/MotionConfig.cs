using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;
using ATL.Core;

namespace ZYXray.Models
{
    class MotionConfig: ObservableObject
    {
        private static MotionConfig _instance = new MotionConfig();

        public static MotionConfig Instance
        {
            get { return _instance; }
        }

        public string InBeltspeed
        {
            get { return UserDefineVariableInfo.DicVariables["m_dInSpeedTemp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dInSpeedTemp"] = double.Parse(value);
                RaisePropertyChanged("InBeltspeed");
            }
        }

        public string BlockTime
        {
            get { return UserDefineVariableInfo.DicVariables["m_iBlockTime"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_iBlockTime"] = int.Parse(value);
                RaisePropertyChanged("BlockTime");
            }
        }

        public string ToCatchSpeed2
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToCatchSpeed2Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToCatchSpeed2Temp"] = double.Parse(value);
                RaisePropertyChanged("ToCatchSpeed2");
            }
        }

        public string To1stInspectSpeed2
        {
            get { return UserDefineVariableInfo.DicVariables["m_dTo1stInspectSpeed2Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dTo1stInspectSpeed2Temp"] = double.Parse(value);
                RaisePropertyChanged("To1stInspectSpeed2");
            }
        }

        public string To2ndInspectSpeed2
        {
            get { return UserDefineVariableInfo.DicVariables["m_dTo2ndInspectSpeed2Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dTo2ndInspectSpeed2Temp"] = double.Parse(value);
                RaisePropertyChanged("To2ndInspectSpeed2");
            }
        }

        public string ToPutSpeed2
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToPutSpeed2Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToPutSpeed2Temp"] = double.Parse(value);
                RaisePropertyChanged("ToPutSpeed2");
            }
        }

        public string ToBoardsideSpeed2
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToBoardsideSpeed2Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToBoardsideSpeed2Temp"] = double.Parse(value);
                RaisePropertyChanged("ToBoardsideSpeed2");
            }
        }

        public string ToWaitSpeed2
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToWaitSpeed2Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToWaitSpeed2Temp"] = double.Parse(value);
                RaisePropertyChanged("ToWaitSpeed2");
            }
        }

        public bool IsRayMoveEnable
        {
            get { return UserDefineVariableInfo.DicVariables["m_iRayMoveEnable"].ToString() == "1" ? true : false; }
            set
            {
                if (value == true)
                    UserDefineVariableInfo.DicVariables["m_iRayMoveEnable"] = 1;
                else
                    UserDefineVariableInfo.DicVariables["m_iRayMoveEnable"] = 0;

                RaisePropertyChanged("IsRayMoveEnable");
            }
        }

        public string CaptureTime
        {
            get { return UserDefineVariableInfo.DicVariables["m_iCaptureTime"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_iCaptureTime"] = int.Parse(value);
                RaisePropertyChanged("CaptureTime");
            }
        }

        public string ToCatchSpeed1
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToCatchSpeed1Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToCatchSpeed1Temp"] = double.Parse(value);
                RaisePropertyChanged("ToCatchSpeed1");
            }
        }

        public string To1stInspectSpeed1
        {
            get { return UserDefineVariableInfo.DicVariables["m_dTo1stInspectSpeed1Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dTo1stInspectSpeed1Temp"] = double.Parse(value);
                RaisePropertyChanged("To1stInspectSpeed1");
            }
        }

        public string To2ndInspectSpeed1
        {
            get { return UserDefineVariableInfo.DicVariables["m_dTo2ndInspectSpeed1Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dTo2ndInspectSpeed1Temp"] = double.Parse(value);
                RaisePropertyChanged("To2ndInspectSpeed1");
            }
        }

        public string ToPutSpeed1
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToPutSpeed1Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToPutSpeed1Temp"] = double.Parse(value);
                RaisePropertyChanged("ToPutSpeed1");
            }
        }

        public string ToBoardsideSpeed1
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToBoardsideSpeed1Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToBoardsideSpeed1Temp"] = double.Parse(value);
                RaisePropertyChanged("ToBoardsideSpeed1");
            }
        }

        public string ToWaitSpeed1
        {
            get { return UserDefineVariableInfo.DicVariables["m_dToWaitSpeed1Temp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dToWaitSpeed1Temp"] = double.Parse(value);
                RaisePropertyChanged("ToWaitSpeed1");
            }
        }

        public string OutBeltspeed
        {
            get { return UserDefineVariableInfo.DicVariables["m_dOutSpeedTemp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dOutSpeedTemp"] = double.Parse(value);
                RaisePropertyChanged("OutBeltspeed");
            }
        }

        public string OutBeltStepLength
        {
            get { return UserDefineVariableInfo.DicVariables["m_dOutStepTemp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dOutStepTemp"] = double.Parse(value);
                RaisePropertyChanged("OutBeltStepLength");
            }
        }

        public bool IsEndSensorEnable
        {

            get { return UserDefineVariableInfo.DicVariables["m_iEnableEndSensor"].ToString() == "1" ? true : false; }
            set
            {
                if (value == true)
                    UserDefineVariableInfo.DicVariables["m_iEnableEndSensor"] = 1;
                else
                    UserDefineVariableInfo.DicVariables["m_iEnableEndSensor"] = 0;

                RaisePropertyChanged("IsEndSensorEnable");
            }
        }

        public bool IsOnlineEnable
        {
            get { return UserDefineVariableInfo.DicVariables["m_iConnectMachine"].ToString() == "1" ? true : false; }
            set
            {
                if (value == true)
                    UserDefineVariableInfo.DicVariables["m_iConnectMachine"] = 1;
                else
                    UserDefineVariableInfo.DicVariables["m_iConnectMachine"] = 0;

                RaisePropertyChanged("IsOnlineEnable");
            }
        }

        public string NGBoxNum
        {
            get { return UserDefineVariableInfo.DicVariables["m_iNGBoxNum"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_iNGBoxNum"] = int.Parse(value);
                RaisePropertyChanged("NGBoxNum");
            }
        }

        public string NgHandSpeed
        {
            get { return UserDefineVariableInfo.DicVariables["m_dNgSpeedTemp"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dNgSpeedTemp"] = double.Parse(value);
                RaisePropertyChanged("NgHandSpeed");
            }
        }

        public string NgHandKeepDown
        {
            get { return UserDefineVariableInfo.DicVariables["m_dNgKeepDown"].ToString(); }
            set
            {
                UserDefineVariableInfo.DicVariables["m_dNgKeepDown"] = double.Parse(value);
                RaisePropertyChanged("NgHandKeepDown");
            }
        }
    }
}
