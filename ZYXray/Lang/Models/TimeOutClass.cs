using System;

namespace ZYXray.Models
{
    public class TimeOutClass
    {
        // 设定超时间隔为1000ms
        private int TimeoutInterval = 1000;
        // lastTicks 用于存储新建操作开始时的时间
        public long lastTicks;
        // 用于存储操作消耗的时间
        public long elapsedTicks;

        /// <summary>
        /// 默认为1000ms
        /// </summary>
        public TimeOutClass(int time_ms = 1000)
        {
            TimeoutInterval = time_ms;
            lastTicks = DateTime.Now.Ticks;
        }

        public bool IsTimeout()
        {
            elapsedTicks = DateTime.Now.Ticks - lastTicks;
            TimeSpan span = new TimeSpan(elapsedTicks);
            double diff = span.TotalMilliseconds;
            if (diff > TimeoutInterval)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
