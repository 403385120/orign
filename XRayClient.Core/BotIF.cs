using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayClient.VisionSysWrapper;


namespace XRayClient.Core
{
    public class BotIF
    {
        private static Bot _bot = new Bot();

        public static bool Init(CameraParams camParam1, CameraParams camParam2)
        {
            _bot.Init(camParam1, camParam2);

            return true;
        }

        public static bool UnInit()
        {
            _bot.UnInit();

            return true;
        }

        public static bool ChangeParam(CameraParams camParam1, CameraParams camParam2)
        {
            _bot.ChangeParam(camParam1, camParam2);
            return true;
        }

        public static void InspectTest(string imgPath, ref string imgLeftUp, ref string imgLeftDown,
                       ref string imgRightUp, ref string imgRightDown, bool autoDetect, ref List<float> timeList)
        {
            _bot.InspectTest(imgPath, ref imgLeftUp, ref imgLeftDown, ref imgRightUp, ref imgRightDown, autoDetect, ref timeList);
        }

        public static CheckStatus MyCheckStatus
        {
            get { return _bot.MyCheckStatus; }
        }

        public static BatterySeat ResultSeat
        {
            get { return _bot.ResultSeat; }
        }

        public static void Start()
        {
            _bot.Start();
        }

        public static void Stop()
        {
            _bot.Stop();
        }

        public static void AllowStart()
        {
            _bot.AllowStart();
        }

        public static void DisallowStart()
        {
            _bot.DisallowStart();
        }



        /// <summary>
        /// 
        /// </summary>
        public static void ClearActiveAlarm()
        {

        }

        public static void ClearAllAlarm()
        {

        }

        /// <summary>
        /// 检测结果处理事件
        /// </summary>
        public static event CheckResultHandler CheckResultHandlers
        {
            add
            {
                ResultRelay.Instance.CheckResultHandlers += value;
            }
            remove
            {
                ResultRelay.Instance.CheckResultHandlers -= value;
            }
        }

        /// <summary>
        /// 报警阻塞事件(用于UI用户处理)
        /// </summary>
    }
}
