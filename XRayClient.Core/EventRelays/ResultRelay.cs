using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayClient.VisionSysWrapper;

namespace XRayClient.Core
{
    public delegate void CheckResultHandler(BatterySeat batSeat);

    /// <summary>
    /// 结果处理事件路由
    /// </summary>
    public class ResultRelay
    {
        public static ResultRelay _instance = new ResultRelay();
        public static ResultRelay Instance
        {
            get { return _instance; }
        }

        public CheckResultHandler CheckResultHandlers;

        public void HandlerResult(ref BatterySeat batSeat)
        {
            if(null != this.CheckResultHandlers)
            {
                this.CheckResultHandlers(batSeat);
            }
        }
    }
}
