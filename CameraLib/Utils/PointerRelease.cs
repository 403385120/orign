//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace ZY.Vision.Utils
//{
//    /// <summary>
//    /// 创建对指针对象的记录，并在后台自动释放对应的指针数据
//    /// ZhangKF 2021-1-7
//    /// </summary>
//    public class PointerRelease
//    {
//        //指针数据
//        IntPtr _data;
//        DateTime _createTime = DateTime.Now;
//        ///<summary>记录需要释放的指针对象</summary>
//        static ConcurrentDictionary<Int64, PointerRelease> _dicts = new ConcurrentDictionary<Int64, PointerRelease>();
//        ///<summary>释放对象的间隙时长(10秒)</summary>
//        const int RELEASE_TIME = 10000;
//        ///<summary>时钟频率</summary>
//        const int RELEASE_LOOP_TIME = 1000;

//        ///<summary>指针释放定时器任务</summary>
//        static System.Threading.Timer _timer = new System.Threading.Timer(status =>
//        {
//            PointerRelease pointer = null;
//            //释放资源
//            foreach (var key in _dicts.Keys)
//            {
//                _dicts.TryGetValue(key, out pointer);
//                if (pointer != null)
//                {
//                    var ts = DateTime.Now - pointer._createTime;
//                    //回收数据的间隔时间
//                    if (ts.TotalMilliseconds >= RELEASE_TIME && pointer._data != IntPtr.Zero)
//                    {
//                        try
//                        {
//                            if (_dicts.TryRemove(key, out pointer))
//                            {
//                                Marshal.FreeHGlobal(pointer._data);
//                                Share.Info(string.Format("指针数据释放成功:{0}", key));
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Share.Info(string.Format("指针数据释放失败:{0}", key));
//                            Share.Error(ex.Message);
//                            Share.Error(ex.StackTrace);
//                        }
//                    }
//                }
//            }
//        }, null, RELEASE_TIME, RELEASE_LOOP_TIME);

//        ///<summary>指针记录</summary>
//        public static void Record(IntPtr ptr)
//        {
//            if (ptr != IntPtr.Zero)
//            {
//                PointerRelease pointer = new PointerRelease();
//                pointer._data = ptr;
//                pointer._createTime = DateTime.Now;
//                _dicts.TryAdd(ptr.ToInt64(), pointer);
//            }
//        }

//        ///<summary>查询指定的指针是否已经被记录下来，避免重复释放</summary>
//        public static bool Exist(IntPtr ptr)
//        {
//            return _dicts.ContainsKey(ptr.ToInt64());
//        }
//        //end class
//    }
//}
