using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using ATL.Common;
using ATL.Core;
using ZY.Logging;

namespace ZY.Motion
{
    public class Motion
    {
        public double m_dConveyorConver = 357.142857;
        public double m_dNgHandConver = 250;
        public double m_dTransferHandConver = 250;
        public ushort m_axisX1;
        public ushort m_axisX1OutMode;
        public ushort m_axisX1DirP;
        public ushort m_axisX1DirN;
        public ushort m_axisY1;
        public ushort m_axisY1OutMode;
        public ushort m_axisY1DirP;
        public ushort m_axisY1DirN;
        public ushort m_axisX2;
        public ushort m_axisX2OutMode;
        public ushort m_axisX2DirP;
        public ushort m_axisX2DirN;
        public ushort m_axisY2;
        public ushort m_axisY2OutMode;
        public ushort m_axisY2DirP;
        public ushort m_axisY2DirN;
        public ushort m_axisRay1;
        public ushort m_axisRay1OutMode;
        public ushort m_axisRay1DirP;
        public ushort m_axisRay1DirN;
        public ushort m_axisNg;
        public ushort m_axisNgOutMode;
        public ushort m_axisNgDirP;
        public ushort m_axisNgDirN;
        public ushort m_axisIn;
        public ushort m_axisInOutMode;
        public ushort m_axisInDirP;
        public ushort m_axisInDirN;
        public ushort m_axisOut;
        public ushort m_axisOutOutMode;
        public ushort m_axisOutDirP;
        public ushort m_axisOutDirN;
        public int m_limitX1Min;
        public int m_limitX1Max;
        public int m_limitY1Min;
        public int m_limitY1Max;
        public int m_limitX2Min;
        public int m_limitX2Max;
        public int m_limitY2Min;
        public int m_limitY2Max;
        public double m_dInSpeedTemp;
        public double m_dInSpeed;
        public double m_dOutSpeedTemp;
        public double m_dOutSpeed;
        public double m_dNgSpeedTemp;
        public double m_dNgSpeed;
        public double m_dToCatchSpeed2Temp;
        public double m_dToCatchSpeed2;
        public double m_dTo1stInspectSpeed2Temp;
        public double m_dTo1stInspectSpeed2;
        public double m_dTo2ndInspectSpeed2Temp;
        public double m_dTo2ndInspectSpeed2;
        public double m_dToPutSpeed2Temp;
        public double m_dToPutSpeed2;
        public double m_dToBoardsideSpeed2Temp;
        public double m_dToBoardsideSpeed2;
        public double m_dToWaitSpeed2Temp;
        public double m_dToWaitSpeed2;
        public double m_dToCatchSpeed1Temp;
        public double m_dToCatchSpeed1;
        public double m_dTo1stInspectSpeed1Temp;
        public double m_dTo1stInspectSpeed1;
        public double m_dTo2ndInspectSpeed1Temp;
        public double m_dTo2ndInspectSpeed1;
        public double m_dToPutSpeed1Temp;
        public double m_dToPutSpeed1;
        public double m_dToBoardsideSpeed1Temp;
        public double m_dToBoardsideSpeed1;
        public double m_dToWaitSpeed1Temp;
        public double m_dToWaitSpeed1;
        public int m_outInPush;
        public int m_inZ2Down;
        public int m_inZ2Up;
        public int m_inZ1Down;
        public int m_inZ1Up;
        public int m_inInBack;
        public int m_inInPush;
        public int m_inZ3Down;
        public int m_inZ3Up;
        public int m_inBoxPush;
        public int m_inBoxBack;
        public int m_inInAlarm;
        public int m_inInAlarmOn;
        public int m_inOutAlarm;
        public int m_inOutAlarmOn;
        public int m_inX1Alarm;
        public int m_inX1AlarmOn;
        public int m_inY1Alarm;
        public int m_inY1AlarmOn;
        public int m_inX2Alarm;
        public int m_inX2AlarmOn;
        public int m_inY2Alarm;
        public int m_inY2AlarmOn;
        public int m_inNgAlarm;
        public int m_inNgAlarmOn;
        public int m_inIn1;
        public int m_inIn2;
        public int m_inIn3;
        public int m_inOut1;
        public int m_outBoxPush;
        public Point[] m_ptDistance1 = new Point[8];
        public Point[] m_ptDistance2 = new Point[8];

        public Point g_ptNewWaitAB1;
        public Point g_ptNewWaitAB2;
        public Point g_ptNewShinkArmAB1;
        public Point g_ptNewShinkArmAB2;

        public Point g_ptNewWaitCD1;
        public Point g_ptNewWaitCD2;
        public Point g_ptNewShinkArmCD1;
        public Point g_ptNewShinkArmCD2;

        public int m_lNgWaitPosition;
        public int m_lNgPutPosition1;
        public int m_lNgPutPosition2;
        public double m_dOutStepTemp;
        public double m_dOutStep;
        public ushort m_inZ2Vacuum;
        public ushort m_outZ2Vacuum;
        public ushort m_outZ2UpDown;
        public ushort m_inZ1Vacuum;
        public ushort m_outZ1Vacuum;
        public ushort m_outZ1UpDown;
        public ushort m_inZ3Vacuum;
        public int m_outZ3Vacuum;
        public ushort m_outZ3UpDown;
        public int m_inX1Home;
        public int m_inX2Home;
        public int m_inY1Home;
        public int m_inY2Home;
        public int m_outX1Home;
        public int m_outX2Home;
        public int m_outY1Home;
        public int m_outY2Home;
        public int m_iBlockTime;
        public int m_iNGBoxNum;
        public int m_iCaptureTime;
        public double m_dNgKeepDown;
        public int m_iRayMoveEnable;
        public int m_iEnableEndSensor;
        public int m_iConnectMachine;
        public int m_iInspectType;
        public int m_outInAllow;
        public int m_outRedLight;
        public int m_outYellowLight;
        public int m_outGreenLight;
        public int m_outBuzzer;

        private static Motion _instance = new Motion();
        
        public static Motion Instance
        {
            get { return _instance; }
        }

        private Motion()
        {
            GetParams();
        }

        public static bool Init()
        {
            bool res = false;
            int m = DMC2C80.d2c80_board_init();
            int n = IOC0640.ioc_board_init();

            LoggingIF.Log("found " + m.ToString() + " 2C80 card," + n.ToString() + " 0640 card", LogLevels.Info, "Motion");

            if (m == 1 && n == 1)
            {
                res = true;

                Motion._instance.GetParams();
                Motion._instance.SetParamForAxis();

                LoggingIF.Log("initialization motion control card success", LogLevels.Info, "Motion");
            }

            return res;
        }

        public static void UnInit()
        {
            Motion._instance.StopConveyor(Motion.Instance.m_axisIn);

            DMC2C80.d2c80_board_close();
            IOC0640.ioc_board_close();

            LoggingIF.Log("ioc_board_close d2c80_board_close", LogLevels.Info, "Motion");
        }

        public void GetParams()
        {
            m_axisX1 = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX1"].ToString());
            m_axisX1OutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX1OutMode"].ToString());
            m_axisX1DirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX1DirP"].ToString());
            m_axisX1DirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX1DirN"].ToString());
            m_axisY1 = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY1"].ToString());
            m_axisY1OutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY1OutMode"].ToString());
            m_axisY1DirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY1DirP"].ToString());
            m_axisY1DirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY1DirN"].ToString());
            m_axisX2 = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX2"].ToString());
            m_axisX2OutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX2OutMode"].ToString());
            m_axisX2DirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX2DirP"].ToString());
            m_axisX2DirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisX2DirN"].ToString());
            m_axisY2 = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY2"].ToString());
            m_axisY2OutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY2OutMode"].ToString());
            m_axisY2DirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY2DirP"].ToString());
            m_axisY2DirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisY2DirN"].ToString());
            m_axisRay1 = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisRay1"].ToString());
            m_axisRay1OutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisRay1OutMode"].ToString());
            m_axisRay1DirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisRay1DirP"].ToString());
            m_axisRay1DirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisRay1DirN"].ToString());
            m_axisNg = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisNg"].ToString());
            m_axisNgOutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisNgOutMode"].ToString());
            m_axisNgDirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisNgDirP"].ToString());
            m_axisNgDirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisNgDirN"].ToString());
            m_axisIn = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisIn"].ToString());
            m_axisInOutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisInOutMode"].ToString());
            m_axisInDirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisInDirP"].ToString());
            m_axisInDirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisInDirN"].ToString());
            m_axisOut = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisOut"].ToString());
            m_axisOutOutMode = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisOutOutMode"].ToString());
            m_axisOutDirP = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisOutDirP"].ToString());
            m_axisOutDirN = ushort.Parse(UserDefineVariableInfo.DicVariables["m_axisOutDirN"].ToString());

            m_limitX1Min = int.Parse(UserDefineVariableInfo.DicVariables["m_limitX1Min"].ToString());
            m_limitX1Max = int.Parse(UserDefineVariableInfo.DicVariables["m_limitX1Max"].ToString());
            m_limitY1Min = int.Parse(UserDefineVariableInfo.DicVariables["m_limitY1Min"].ToString());
            m_limitY1Max = int.Parse(UserDefineVariableInfo.DicVariables["m_limitY1Max"].ToString());
            m_limitX2Min = int.Parse(UserDefineVariableInfo.DicVariables["m_limitX2Min"].ToString());
            m_limitX2Max = int.Parse(UserDefineVariableInfo.DicVariables["m_limitX2Max"].ToString());
            m_limitY2Min = int.Parse(UserDefineVariableInfo.DicVariables["m_limitY2Min"].ToString());
            m_limitY2Max = int.Parse(UserDefineVariableInfo.DicVariables["m_limitY2Max"].ToString());

            m_dInSpeedTemp = double.Parse(UserDefineVariableInfo.DicVariables["m_dInSpeedTemp"].ToString());
            m_dInSpeed = m_dInSpeedTemp * m_dConveyorConver;
            m_dOutSpeedTemp = double.Parse(UserDefineVariableInfo.DicVariables["m_dOutSpeedTemp"].ToString());
            m_dOutSpeed = m_dOutSpeedTemp * m_dConveyorConver;
            m_dNgSpeedTemp = double.Parse(UserDefineVariableInfo.DicVariables["m_dNgSpeedTemp"].ToString());
            m_dNgSpeed = m_dNgSpeedTemp * m_dNgHandConver;
            m_dToCatchSpeed2Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToCatchSpeed2Temp"].ToString());
            m_dToCatchSpeed2 = m_dToCatchSpeed2Temp * m_dTransferHandConver;
            m_dTo1stInspectSpeed2Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dTo1stInspectSpeed2Temp"].ToString());
            m_dTo1stInspectSpeed2 = m_dTo1stInspectSpeed2Temp * m_dTransferHandConver;
            m_dTo2ndInspectSpeed2Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dTo2ndInspectSpeed2Temp"].ToString());
            m_dTo2ndInspectSpeed2 = m_dTo2ndInspectSpeed2Temp * m_dTransferHandConver;
            m_dToPutSpeed2Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToPutSpeed2Temp"].ToString());
            m_dToPutSpeed2 = m_dToPutSpeed2Temp * m_dTransferHandConver;
            m_dToBoardsideSpeed2Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToBoardsideSpeed2Temp"].ToString());
            m_dToBoardsideSpeed2 = m_dToBoardsideSpeed2Temp * m_dTransferHandConver;
            m_dToWaitSpeed2Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToWaitSpeed2Temp"].ToString());
            m_dToWaitSpeed2 = m_dToWaitSpeed2Temp * m_dTransferHandConver;
            m_dToCatchSpeed1Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToCatchSpeed1Temp"].ToString());
            m_dToCatchSpeed1 = m_dToCatchSpeed1Temp * m_dTransferHandConver;
            m_dTo1stInspectSpeed1Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dTo1stInspectSpeed1Temp"].ToString());
            m_dTo1stInspectSpeed1 = m_dTo1stInspectSpeed1Temp * m_dTransferHandConver;
            m_dTo2ndInspectSpeed1Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dTo2ndInspectSpeed1Temp"].ToString());
            m_dTo2ndInspectSpeed1 = m_dTo2ndInspectSpeed1Temp * m_dTransferHandConver;
            m_dToPutSpeed1Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToPutSpeed1Temp"].ToString());
            m_dToPutSpeed1 = m_dToPutSpeed1Temp * m_dTransferHandConver;
            m_dToBoardsideSpeed1Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToBoardsideSpeed1Temp"].ToString());
            m_dToBoardsideSpeed1 = m_dToBoardsideSpeed1Temp * m_dTransferHandConver;
            m_dToWaitSpeed1Temp = double.Parse(UserDefineVariableInfo.DicVariables["m_dToWaitSpeed1Temp"].ToString());
            m_dToWaitSpeed1 = m_dToWaitSpeed1Temp * m_dTransferHandConver;

            m_outInPush = int.Parse(UserDefineVariableInfo.DicVariables["m_outInPush"].ToString());
            m_inZ2Down = int.Parse(UserDefineVariableInfo.DicVariables["m_inZ2Down"].ToString());
            m_inZ2Up = int.Parse(UserDefineVariableInfo.DicVariables["m_inZ2Up"].ToString());
            m_inZ1Down = int.Parse(UserDefineVariableInfo.DicVariables["m_inZ1Down"].ToString());
            m_inZ1Up = int.Parse(UserDefineVariableInfo.DicVariables["m_inZ1Up"].ToString());
            m_inInBack = int.Parse(UserDefineVariableInfo.DicVariables["m_inInBack"].ToString());
            m_inInPush = int.Parse(UserDefineVariableInfo.DicVariables["m_inInPush"].ToString());
            m_inZ3Down = int.Parse(UserDefineVariableInfo.DicVariables["m_inZ3Down"].ToString());
            m_inZ3Up = int.Parse(UserDefineVariableInfo.DicVariables["m_inZ3Up"].ToString());
            m_inBoxPush = int.Parse(UserDefineVariableInfo.DicVariables["m_inBoxPush"].ToString());
            m_inBoxBack = int.Parse(UserDefineVariableInfo.DicVariables["m_inBoxBack"].ToString());
            m_inInAlarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inInAlarm"].ToString());
            m_inInAlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inInAlarmOn"].ToString());
            m_inOutAlarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inOutAlarm"].ToString());
            m_inOutAlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inOutAlarmOn"].ToString());
            m_inX1Alarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inX1Alarm"].ToString());
            m_inX1AlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inX1AlarmOn"].ToString());
            m_inY1Alarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inY1Alarm"].ToString());
            m_inY1AlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inY1AlarmOn"].ToString());
            m_inX2Alarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inX2Alarm"].ToString());
            m_inX2AlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inX2AlarmOn"].ToString());
            m_inY2Alarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inY2Alarm"].ToString());
            m_inY2AlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inY2AlarmOn"].ToString());
            m_inNgAlarm = int.Parse(UserDefineVariableInfo.DicVariables["m_inNgAlarm"].ToString());
            m_inNgAlarmOn = int.Parse(UserDefineVariableInfo.DicVariables["m_inNgAlarmOn"].ToString());
            m_inIn1 = int.Parse(UserDefineVariableInfo.DicVariables["m_inIn1"].ToString());
            m_inIn2 = int.Parse(UserDefineVariableInfo.DicVariables["m_inIn2"].ToString());
            m_inIn3 = int.Parse(UserDefineVariableInfo.DicVariables["m_inIn3"].ToString());
            m_inOut1 = int.Parse(UserDefineVariableInfo.DicVariables["m_inOut1"].ToString());
            m_outBoxPush = int.Parse(UserDefineVariableInfo.DicVariables["m_outBoxPush"].ToString());


            m_ptDistance1[0].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[0].X"].ToString());
            m_ptDistance1[0].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[0].Y"].ToString());
            m_ptDistance1[1].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[1].X"].ToString());
            m_ptDistance1[1].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[1].Y"].ToString());
            m_ptDistance1[2].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[2].X"].ToString());
            m_ptDistance1[2].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[2].Y"].ToString());
            m_ptDistance1[3].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[3].X"].ToString());
            m_ptDistance1[3].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[3].Y"].ToString());
            m_ptDistance1[4].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[4].X"].ToString());
            m_ptDistance1[4].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[4].Y"].ToString());
            m_ptDistance1[5].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[5].X"].ToString());
            m_ptDistance1[5].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[5].Y"].ToString());
            m_ptDistance1[6].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[6].X"].ToString());
            m_ptDistance1[6].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[6].Y"].ToString());
            m_ptDistance1[7].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[7].X"].ToString());
            m_ptDistance1[7].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance1[7].Y"].ToString());


            m_ptDistance2[0].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[0].X"].ToString());
            m_ptDistance2[0].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[0].Y"].ToString());
            m_ptDistance2[1].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[1].X"].ToString());
            m_ptDistance2[1].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[1].Y"].ToString());
            m_ptDistance2[2].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[2].X"].ToString());
            m_ptDistance2[2].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[2].Y"].ToString());
            m_ptDistance2[3].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[3].X"].ToString());
            m_ptDistance2[3].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[3].Y"].ToString());
            m_ptDistance2[4].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[4].X"].ToString());
            m_ptDistance2[4].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[4].Y"].ToString());
            m_ptDistance2[5].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[5].X"].ToString());
            m_ptDistance2[5].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[5].Y"].ToString());
            m_ptDistance2[6].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[6].X"].ToString());
            m_ptDistance2[6].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[6].Y"].ToString());
            m_ptDistance2[7].X = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[7].X"].ToString());
            m_ptDistance2[7].Y = int.Parse(UserDefineVariableInfo.DicVariables["m_ptDistance2[7].Y"].ToString());

            RePlanPath();

            m_lNgWaitPosition = int.Parse(UserDefineVariableInfo.DicVariables["m_lNgWaitPosition"].ToString());
            m_lNgPutPosition1 = int.Parse(UserDefineVariableInfo.DicVariables["m_lNgPutPosition1"].ToString());
            m_lNgPutPosition2 = int.Parse(UserDefineVariableInfo.DicVariables["m_lNgPutPosition2"].ToString());


            m_dOutStepTemp = double.Parse(UserDefineVariableInfo.DicVariables["m_dOutStepTemp"].ToString());
            m_dOutStep = m_dOutStepTemp * m_dConveyorConver;

            m_inZ2Vacuum = ushort.Parse(UserDefineVariableInfo.DicVariables["m_inZ2Vacuum"].ToString());
            m_outZ2Vacuum = ushort.Parse(UserDefineVariableInfo.DicVariables["m_outZ2Vacuum"].ToString());
            m_outZ2UpDown = ushort.Parse(UserDefineVariableInfo.DicVariables["m_outZ2UpDown"].ToString());

            m_inZ1Vacuum = ushort.Parse(UserDefineVariableInfo.DicVariables["m_inZ1Vacuum"].ToString());
            m_outZ1Vacuum = ushort.Parse(UserDefineVariableInfo.DicVariables["m_outZ1Vacuum"].ToString());
            m_outZ1UpDown = ushort.Parse(UserDefineVariableInfo.DicVariables["m_outZ1UpDown"].ToString());

            m_inZ3Vacuum = ushort.Parse(UserDefineVariableInfo.DicVariables["m_inZ3Vacuum"].ToString());
            m_outZ3Vacuum = int.Parse(UserDefineVariableInfo.DicVariables["m_outZ3Vacuum"].ToString());
            m_outZ3UpDown = ushort.Parse(UserDefineVariableInfo.DicVariables["m_outZ3UpDown"].ToString());

            m_inX1Home = int.Parse(UserDefineVariableInfo.DicVariables["m_inX1Home"].ToString());
            m_inX2Home = int.Parse(UserDefineVariableInfo.DicVariables["m_inX2Home"].ToString());
            m_inY1Home = int.Parse(UserDefineVariableInfo.DicVariables["m_inY1Home"].ToString());
            m_inY2Home = int.Parse(UserDefineVariableInfo.DicVariables["m_inY2Home"].ToString());

            m_outX1Home = int.Parse(UserDefineVariableInfo.DicVariables["m_outX1Home"].ToString());
            m_outX2Home = int.Parse(UserDefineVariableInfo.DicVariables["m_outX2Home"].ToString());
            m_outY1Home = int.Parse(UserDefineVariableInfo.DicVariables["m_outY1Home"].ToString());
            m_outY2Home = int.Parse(UserDefineVariableInfo.DicVariables["m_outY2Home"].ToString());

            m_iBlockTime = int.Parse(UserDefineVariableInfo.DicVariables["m_iBlockTime"].ToString());
            m_iNGBoxNum = int.Parse(UserDefineVariableInfo.DicVariables["m_iNGBoxNum"].ToString());
            m_iCaptureTime = int.Parse(UserDefineVariableInfo.DicVariables["m_iCaptureTime"].ToString());
            m_dNgKeepDown = double.Parse(UserDefineVariableInfo.DicVariables["m_dNgKeepDown"].ToString());

            m_iRayMoveEnable = int.Parse(UserDefineVariableInfo.DicVariables["m_iRayMoveEnable"].ToString());
            m_iEnableEndSensor = int.Parse(UserDefineVariableInfo.DicVariables["m_iEnableEndSensor"].ToString());
            m_iConnectMachine = int.Parse(UserDefineVariableInfo.DicVariables["m_iConnectMachine"].ToString());
            m_iInspectType = int.Parse(UserDefineVariableInfo.DicVariables["m_iInspectType"].ToString());

            m_outInAllow = int.Parse(UserDefineVariableInfo.DicVariables["m_outInAllow"].ToString());
            m_outRedLight = int.Parse(UserDefineVariableInfo.DicVariables["RedLight"].ToString());
            m_outYellowLight = int.Parse(UserDefineVariableInfo.DicVariables["YellowLight"].ToString());
            m_outGreenLight = int.Parse(UserDefineVariableInfo.DicVariables["GreenLight"].ToString());
            m_outBuzzer = int.Parse(UserDefineVariableInfo.DicVariables["Buzzer"].ToString());
        }

        public void RePlanPath()
        {
            int iBatteryWidth1 = Math.Abs(m_ptDistance1[2].X - m_ptDistance1[1].X) + 2000;
            int iBatteryHeight1 = Math.Abs(m_ptDistance1[2].Y - m_ptDistance1[1].Y) + 4000;

            int iBatteryWidth2 = Math.Abs(m_ptDistance2[2].X - m_ptDistance2[1].X) + 2000;
            int iBatteryHeight2 = (int)(Math.Abs(m_ptDistance2[2].Y - m_ptDistance2[1].Y) * 1.5);

            g_ptNewShinkArmAB1.X = m_ptDistance1[5].X;
            if (Math.Abs(m_ptDistance1[1].Y + 2000) < Math.Abs(m_ptDistance1[0].Y))
            {
                g_ptNewShinkArmAB1.Y = m_ptDistance1[1].Y + iBatteryHeight1;
            }
            else
            {
                g_ptNewShinkArmAB1.Y = m_ptDistance1[0].Y + iBatteryHeight1;
            }
            if (g_ptNewShinkArmAB1.Y > 0)
            {
                g_ptNewShinkArmAB1.Y = 0;
            }

            g_ptNewWaitAB1.X = m_ptDistance1[1].X + iBatteryWidth1;
            g_ptNewWaitAB1.Y = g_ptNewShinkArmAB1.Y;

            g_ptNewShinkArmAB2.X = m_ptDistance2[5].X;
            g_ptNewShinkArmAB2.Y = m_ptDistance2[2].Y + iBatteryHeight2;
            if (g_ptNewShinkArmAB2.Y > 0)
            {
                g_ptNewShinkArmAB2.Y = 0;
            }
            g_ptNewWaitAB2.X = m_ptDistance2[1].X + iBatteryWidth2;
            g_ptNewWaitAB2.Y = g_ptNewShinkArmAB2.Y;


            iBatteryWidth1 = Math.Abs(m_ptDistance1[3].X - m_ptDistance1[4].X) + 2000;
            iBatteryHeight1 = Math.Abs(m_ptDistance1[3].Y - m_ptDistance1[4].Y) + 8000;
            iBatteryWidth2 = Math.Abs(m_ptDistance2[3].X - m_ptDistance2[4].X) + 2000;
            iBatteryHeight2 = (int)(Math.Abs(m_ptDistance2[3].Y - m_ptDistance2[4].Y) * 1.5);

            g_ptNewShinkArmCD1.X = m_ptDistance1[5].X;
            if (Math.Abs(m_ptDistance1[3].Y + 2000) < Math.Abs(m_ptDistance1[0].Y))
            {
                g_ptNewShinkArmCD1.Y = m_ptDistance1[3].Y + iBatteryHeight1;
            }
            else
            {
                g_ptNewShinkArmCD1.Y = m_ptDistance1[0].Y + iBatteryHeight1;
            }
            if (g_ptNewShinkArmCD1.Y > 0)
            {
                g_ptNewShinkArmCD1.Y = 0;
            }
            g_ptNewWaitCD1.X = m_ptDistance1[3].X + iBatteryWidth1;
            g_ptNewWaitCD1.Y = g_ptNewShinkArmCD1.Y;

            g_ptNewShinkArmCD2.X = m_ptDistance2[5].X;
            g_ptNewShinkArmCD2.Y = m_ptDistance2[3].Y + iBatteryHeight2;
            if (g_ptNewShinkArmCD2.Y > 0)
            {
                g_ptNewShinkArmCD2.Y = 0;
            }
            g_ptNewWaitCD2.X = m_ptDistance2[3].X + iBatteryWidth2;
            g_ptNewWaitCD2.Y = g_ptNewShinkArmCD2.Y;
        }

        public void SaveParams()
        {
            SaveHandLocation();

            GetParams();
        }

        private void SaveHandLocation()
        {
            UserDefineVariableInfo.DicVariables["m_ptDistance1[0].X"] = m_ptDistance1[0].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[0].Y"] = m_ptDistance1[0].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[1].X"] = m_ptDistance1[1].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[1].Y"] = m_ptDistance1[1].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[2].X"] = m_ptDistance1[2].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[2].Y"] = m_ptDistance1[2].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[3].X"] = m_ptDistance1[3].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[3].Y"] = m_ptDistance1[3].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[4].X"] = m_ptDistance1[4].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[4].Y"] = m_ptDistance1[4].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[5].X"] = m_ptDistance1[5].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[5].Y"] = m_ptDistance1[5].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[6].X"] = m_ptDistance1[6].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[6].Y"] = m_ptDistance1[6].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance1[7].X"] = m_ptDistance1[7].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance1[7].Y"] = m_ptDistance1[7].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[0].X"] = m_ptDistance2[0].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[0].Y"] = m_ptDistance2[0].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[1].X"] = m_ptDistance2[1].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[1].Y"] = m_ptDistance2[1].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[2].X"] = m_ptDistance2[2].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[2].Y"] = m_ptDistance2[2].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[3].X"] = m_ptDistance2[3].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[3].Y"] = m_ptDistance2[3].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[4].X"] = m_ptDistance2[4].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[4].Y"] = m_ptDistance2[4].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[5].X"] = m_ptDistance2[5].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[5].Y"] = m_ptDistance2[5].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[6].X"] = m_ptDistance2[6].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[6].Y"] = m_ptDistance2[6].Y;

            UserDefineVariableInfo.DicVariables["m_ptDistance2[7].X"] = m_ptDistance2[7].X;
            UserDefineVariableInfo.DicVariables["m_ptDistance2[7].Y"] = m_ptDistance2[7].Y;


            UserDefineVariableInfo.DicVariables["m_lNgWaitPosition"] = m_lNgWaitPosition;
            UserDefineVariableInfo.DicVariables["m_lNgPutPosition1"] = m_lNgPutPosition1;
            UserDefineVariableInfo.DicVariables["m_lNgPutPosition2"] = m_lNgPutPosition2;
        }

        private void SetParamForAxis()
        {
            DMC2C80.d2c80_set_pulse_outmode(m_axisX1, m_axisX1OutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisY1, m_axisY1OutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisX2, m_axisX2OutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisY2, m_axisY2OutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisRay1, m_axisRay1OutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisNg, m_axisNgOutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisIn, m_axisInOutMode);
            DMC2C80.d2c80_set_pulse_outmode(m_axisOut, m_axisOutOutMode);

            DMC2C80.d2c80_config_softlimit(m_axisX1, 1, 0, 1, m_limitX1Min, m_limitX1Max);
            DMC2C80.d2c80_config_softlimit(m_axisY1, 1, 0, 1, m_limitY1Min, m_limitY1Max);
            DMC2C80.d2c80_config_softlimit(m_axisX2, 1, 0, 1, m_limitX2Min, m_limitX2Max);
            DMC2C80.d2c80_config_softlimit(m_axisY2, 1, 0, 1, m_limitY2Min, m_limitY2Max);

            DMC2C80.d2c80_config_EL_PIN(m_axisX1, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisY1, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisX2, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisY2, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisRay1, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisNg, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisIn, 0, 0);
            DMC2C80.d2c80_config_EL_PIN(m_axisOut, 0, 0);

            DMC2C80.d2c80_counter_config(m_axisX1, 1);
            DMC2C80.d2c80_counter_config(m_axisY1, 1);
            DMC2C80.d2c80_counter_config(m_axisX2, 1);
            DMC2C80.d2c80_counter_config(m_axisY2, 1);

            DMC2C80.d2c80_config_EZ_PIN(m_axisX1, 0, 0);
            DMC2C80.d2c80_config_EZ_PIN(m_axisY1, 0, 0);
            DMC2C80.d2c80_config_EZ_PIN(m_axisX2, 0, 0);
            DMC2C80.d2c80_config_EZ_PIN(m_axisY2, 0, 0);
        }

        public void SetConveyorSpeed(ushort axis, double max_Vel)
        {
            DMC2C80.d2c80_set_profile(axis, 10000, max_Vel, 1000000, 1000000);
        }

        public void RunConveyor(ushort axis, double s_para, ushort dir, double vel)
        {
            DMC2C80.d2c80_set_s_profile(axis, s_para);
            DMC2C80.d2c80_vmove(axis, dir, vel);
        }

        public void RunConveyorFixedLength(ushort axis, double s_para, int dis, ushort posi_mode)
        {
            DMC2C80.d2c80_set_s_profile(axis, s_para);
            DMC2C80.d2c80_pmove(axis, dis, 0);
        }

        public void StopConveyor(ushort axis)
        {
            DMC2C80.d2c80_decel_stop(axis, 1500000);
        }

        public int GetConveyorState(ushort axis)   //0 正在运行   1 已停止
        {
            return DMC2C80.d2c80_check_done(axis);
        }

        public void ResetConveyorState(ushort axis)
        {
            DMC2C80.d2c80_set_position(axis, 0);
        }

        public void SetCylinder(int bitno, ushort on_off)     //on_off =1 代表收回 , on_off =0 代表伸出
        {
            if(bitno < 0)
                IOC0640.ioc_write_outbit(0, (ushort)-bitno, on_off);
            else
                DMC2C80.d2c80_write_outbit(0, (ushort)bitno, on_off);
        }

        public void TransferHandHome(ushort axis)
        {
            IOC0640.ioc_write_outbit(0, axis, 0);
        }

        public int TransferHandHomeState(ushort axis)    //1 复位失败   0 复位成功
        {
            return IOC0640.ioc_read_inbit(0, axis);
        }

        public void ResetTransferHandState(ushort axis, ushort axishome)
        {
            IOC0640.ioc_write_outbit(0, axishome, 1);
            DMC2C80.d2c80_set_position(axis, 0);
            DMC2C80.d2c80_set_encoder(axis, 0);
        }

        public void ResetNgHandState(ushort axis)
        {
            DMC2C80.d2c80_set_position(axis, 0);
            DMC2C80.d2c80_set_encoder(axis, 0);
        }

        public int HandState(ushort axis)    //1 运动
        {
            uint state = DMC2C80.d2c80_axis_io_status(axis);
            if ((state & 0x0001) == 1)
            {
                return 0;
            }
            else
            {
                if (0 == DMC2C80.d2c80_check_done(axis))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public void NGHandHome(ushort axis, double vel)
        {
            DMC2C80.d2c80_config_HOME_PIN_logic(axis, 0, 0);
            DMC2C80.d2c80_config_home_mode(axis, 1, vel, 1, 0);
            DMC2C80.d2c80_home_move(axis);
        }

        public int GetCylinder(int push_bitbo, int back_bitno)
        {
            int ret = 0;   //异常
            int push_state = 0;
            int banck_state = 0;

            if(push_bitbo > 0)
            {
                push_state = IOC0640.ioc_read_inbit(0, (ushort)push_bitbo);
                banck_state = IOC0640.ioc_read_inbit(0, (ushort)back_bitno);
            }
            else
            {
                push_state = DMC2C80.d2c80_read_inbit(0, (ushort)(-push_bitbo));
                banck_state = DMC2C80.d2c80_read_inbit(0, (ushort)(-back_bitno));
            }

            if (push_state == 0 && banck_state == 1)
                ret = 1;   // 伸出
            else if (push_state == 1 && banck_state == 0)
                ret = -1;   // 收回

            return ret;
        }

        public void RunHand(string strHand, int dir, double axisSpeed)
        {
            ushort axis = 0;
            ushort axisDir = 0;

            if (strHand == "X1")
            {
                axis = m_axisX1;
                axisDir = (dir == 1 ? m_axisX1DirP : m_axisX1DirN);
            }
            else if (strHand == "Y1")
            {
                axis = m_axisY1;
                axisDir = (dir == 1 ? m_axisY1DirP : m_axisY1DirN);
            }
            else if (strHand == "X2")
            {
                axis = m_axisX2;
                axisDir = (dir == 1 ? m_axisX2DirP : m_axisX2DirN);
            }
            else if (strHand == "Y2")
            {
                axis = m_axisY2;
                axisDir = (dir == 1 ? m_axisY2DirP : m_axisY2DirN);
            }
            else if (strHand == "3")
            {
                axis = m_axisNg;
                axisDir = (dir == 1 ? m_axisNgDirP : m_axisNgDirN);
            }
            else if (strHand == "4")
            {
                axis = m_axisRay1;
                axisDir = (dir == 1 ? m_axisRay1DirP : m_axisRay1DirN);
            }

            DMC2C80.d2c80_set_s_profile(axis, 0.2);
            DMC2C80.d2c80_vmove(axis, axisDir, axisSpeed);
        }

        public void StopHand(ushort axis)
        {
            DMC2C80.d2c80_imd_stop(axis);
        }

        public void SetHandSpeed(string strHand, double dMin, double dMax, double dAcc, double dDec)
        {
            if (strHand == "X1")
            {
                while (HandState(m_axisX1) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_profile(m_axisX1, dMin, dMax, dAcc, dDec);
            }
            else if (strHand == "Y1")
            {
                while (HandState(m_axisY1) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_profile(m_axisY1, dMin, dMax, dAcc, dDec);
            }
            else if (strHand == "X2")
            {
                while (HandState(m_axisX2) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_profile(m_axisX2, dMin, dMax, dAcc, dDec);
            }
            else if (strHand == "Y2")
            {
                while (HandState(m_axisY2) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_profile(m_axisY2, dMin, dMax, dAcc, dDec);
            }
            else if (strHand == "1")
            {
                while (HandState(m_axisX1) == 1 || HandState(m_axisY1) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_vector_profile_MultiCoor(0, 0.2, dMax, dAcc, dDec, 1);
            }
            else if (strHand == "2")
            {
                while (HandState(m_axisX2) == 1 || HandState(m_axisY2) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_vector_profile_MultiCoor(0, 0.2, dMax, dAcc, dDec, 2);
            }
            else if (strHand == "3")   //NG 机械手
            {
                while (HandState(m_axisNg) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_profile(m_axisNg, dMin, dMax, dAcc, dDec);
            }
            else if (strHand == "4")   //光管
            {
                while (HandState(m_axisRay1) == 1)
                {
                    Thread.Sleep(5);
                }

                DMC2C80.d2c80_set_profile(m_axisRay1, dMin, dMax, dAcc, dDec);
            }
        }

        public void GotoTransferHand(int hand, Point position)
        {
            if (hand == 1)
            {
                DMC2C80.d2c80_line2_MultiCoor(m_axisX1, position.X, m_axisY1, position.Y, 1, 1);

                while (HandState(m_axisX1) == 1 || HandState(m_axisY1) == 1)
                {
                    Thread.Sleep(5);
                }
            }
            else if (hand == 2)
            {
                DMC2C80.d2c80_line2_MultiCoor(m_axisX2, position.X, m_axisY2, position.Y, 1, 2);

                while (HandState(m_axisX2) == 1 || HandState(m_axisY2) == 1)
                {
                    Thread.Sleep(5);
                }
            }
        }

        public void GotoNGHand(int dist)
        {
            DMC2C80.d2c80_pmove(m_axisNg, dist, 1);

            while (HandState(m_axisNg) == 1)
            {
                Thread.Sleep(5);
            }
        }

        public int GetHandPosition(ushort axis)
        {
            return DMC2C80.d2c80_get_position(axis);
        }

        public bool GetVacuum(ushort bitno)       //读真空表的输入
        {
            if (IOC0640.ioc_read_inbit(0, bitno) == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int ReadVacuum(int bitno)     //0 打开真空 , 1 关闭真空
        {
            if(bitno < 0)
            {
                return IOC0640.ioc_read_outbit(0, (ushort)-bitno);
            }
            else
            {
                return DMC2C80.d2c80_read_outbit(0, (ushort)bitno);
            }
        }

        public void SetVacuum(int bitno, ushort on_off)     //0 打开真空 , 1 关闭真空
        {
            if (bitno < 0)
                IOC0640.ioc_write_outbit(0, (ushort)-bitno, on_off);
            else
                DMC2C80.d2c80_write_outbit(0, (ushort)bitno, on_off);
        }

        public void UpdateTransferHandPosition(int handno, int positionno)
        {
            if (handno == 1)
            {
                if (positionno >= 0 && positionno <= 5)
                {
                    m_ptDistance1[positionno].X = GetHandPosition(m_axisX1);
                    m_ptDistance1[positionno].Y = GetHandPosition(m_axisY1);
                }

                if (positionno == 0)
                {
                    m_ptDistance1[7].X = m_ptDistance1[0].X;
                    m_ptDistance1[7].Y = 0;
                }

                if (positionno == 5)
                {
                    m_ptDistance1[6].X = m_ptDistance1[5].X;
                    m_ptDistance1[6].Y = 0;
                }
            }
            else if (handno == 2)
            {
                if (positionno >= 0 && positionno <= 5)
                {
                    m_ptDistance2[positionno].X = GetHandPosition(m_axisX2);
                    m_ptDistance2[positionno].Y = GetHandPosition(m_axisY2);
                }

                if (positionno == 0)
                {
                    m_ptDistance2[7].X = m_ptDistance2[0].X;
                    m_ptDistance2[7].Y = 0;
                }

                if (positionno == 5)
                {
                    m_ptDistance2[6].X = m_ptDistance2[5].X;
                    m_ptDistance2[6].Y = 0;
                }
            }

        }

        public void UpdateNGHandPosition(int positionno)
        {
            if (positionno == 0)
            {
                m_lNgWaitPosition = GetHandPosition(m_axisNg);
            }
            else if (positionno == 1)
            {
                m_lNgPutPosition1 = GetHandPosition(m_axisNg);
            }
            else if (positionno == 2)
            {
                m_lNgPutPosition2 = GetHandPosition(m_axisNg);
            }
        }

        public int FeedingSensorState()    // 1 无料   0 有料
        {
            return IOC0640.ioc_read_inbit(0, (ushort)m_inIn3);
        }

        public int BlockingSensorState()    // 1 无料   0 有料
        {
            return IOC0640.ioc_read_inbit(0, (ushort)m_inIn2);
        }

        public int EntrySensorState()    // 1 无料   0 有料
        {
            return IOC0640.ioc_read_inbit(0, (ushort)m_inIn1);
        }

        public int GetTriggerIn0640(int bitno, int onstate)      // 1 触发    0 未触发
        {
            int ret = 0;
            int st = IOC0640.ioc_read_inbit(0, (ushort)bitno);

            if ((st == 1 && onstate == 0) || (st == 0 && onstate == 1))
                ret = 1;

            return ret;
        }

        public int GetTriggerIn2C80(int bitno, int onstate)      // 1 触发    0 未触发
        {
            int ret = 0;
            int st = DMC2C80.d2c80_read_inbit(0, (ushort)bitno);

            if ((st == 1 && onstate == 0) || (st == 0 && onstate == 1))
                ret = 1;

            return ret;
        }

        public void SetSignal(int bitno, int onstate)
        {
            if (bitno < 0)
            {
                DMC2C80.d2c80_write_outbit(0, (ushort)(-bitno), (ushort)onstate);
            }
            else
            {
                IOC0640.ioc_write_outbit(0, (ushort)(bitno), onstate);
            }
        }

        public void SetThreeLight(string state, bool onoff)
        {
            int bitno = 0;
            int onstate = 0;
            if (state == "Yellow")
                bitno = m_outYellowLight;
            else if(state == "Green")
                bitno = m_outGreenLight;
            else if (state == "Red")
                bitno = m_outRedLight;
            else if (state == "Buzzer")
                bitno = m_outBuzzer;

            onstate = onoff ? 0 : 1;
            if (bitno > 0)
                DMC2C80.d2c80_write_outbit(0, (ushort)bitno, (ushort)onstate);
            else
                IOC0640.ioc_write_outbit(0, (ushort)-bitno, (ushort)onstate);
        }

    }
}
