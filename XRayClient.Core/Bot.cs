using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZY.Logging;
using XRayClient.VisionSysWrapper;
using ZY.XRayTube;
using System.Timers;

namespace XRayClient.Core
{
    public delegate void StopReasonPagShowHandler(bool isShow);

    /// <summary>
    /// 检测控制逻辑
    /// </summary>
    public class Bot
    {
        private ICheckLogic _checkLogic = CheckLogicFactory.CreateCheckLogic(CheckParamsConfig.Instance.CheckMode);   //recompose by fjy

        private Thread _alarmhandleThread = null;

        private bool _isAlarmHandling = false;
        private bool _isAllAlaemResetHandle = false;
        private ManualResetEvent _waitEvent = new ManualResetEvent(false);
        private bool _isAppExit = false;
        private bool _allowToStart = true;
        public static int DalayTime = 0;    //add by fjy



        public StopReasonPagShowHandler StopReasonPagShowHandlers = null;

        //创建定时器上传NG数量
        private static System.Timers.Timer _uploadTimer = null;

        public CheckStatus MyCheckStatus
        {
            get { return this._checkLogic.MyCheckStatus; }
        }

        public BatterySeat ResultSeat
        {
            get { return this._checkLogic.ResultSeat; }
        }

        public void ChangeParam(CameraParams camParam1, CameraParams camParam2)
        {
            _checkLogic.LoadWorkParams(CheckParamsConfig.Instance, camParam1, camParam2);
        }

        public void InspectTest(string imgPath, ref string imgLeftUp, ref string imgLeftDown,
                         ref string imgRightUp, ref string imgRightDown, bool autoDetect, ref List<float> timeList)
        {
            _checkLogic.InspectTest(imgPath, ref imgLeftUp, ref imgLeftDown, ref imgRightUp, ref imgRightDown, autoDetect, ref timeList);
        }

        public void Init(CameraParams camParam1, CameraParams camParam2)
        {


            // STF使能之后交由STF校验成功后开机
            this.Start();

            _checkLogic.LoadWorkParams(CheckParamsConfig.Instance, camParam1, camParam2);



        }

        public void UnInit()
        {

            this.Stop();

            this._isAppExit = true;
            this._waitEvent.Set();


            if (!this._alarmhandleThread.Join(5000))
            {
                try { this._alarmhandleThread.Abort(); } catch { }
            }

            _checkLogic.Destroy();
        }

        /// <summary>
        /// 检测启动，开启消息循环
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// 上传NG数定时事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimed_uploadNgCountEvent(object source, ElapsedEventArgs e)
        {
        }

        /// <summary>
        /// 检测停止
        /// </summary>
        public void Stop()
        {
        }

        /// <summary>
        /// 允许开机
        /// </summary>
        public void AllowStart()
        {
            this._allowToStart = true;
        }

        /// <summary>
        /// 禁止开机
        /// </summary>
        public void DisallowStart()
        {
            this._allowToStart = false;
        }

        /// <summary>
        /// PLC事件循环
        /// 注意： 为了防止阻塞，这里每个事件都是一个独立的线程
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="priority"></param>
        /// <param name="eventDesc"></param>
        /// <param name="eventArgs"></param>



        /// <summary>
        /// 添加报警时间
        /// </summary>
        /// <param name="alarm"></param>


        /// <summary>
        /// 报警事件线程处理
        /// </summary>


        /// <summary>
        /// 报警事件本地处理
        /// </summary>
        /// <param name="alarm"></param>

    }
}
