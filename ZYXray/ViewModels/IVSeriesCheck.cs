using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XRayClient.Core;

namespace ZYXray.ViewModels
{
    /// <summary>
    /// IV点检逻辑处理 
    /// 1、点检12个或以上电芯，要求存在超规格、刀不通、探针不通各4个
    /// 2、NG电芯各类型的四个电芯必须是连续的
    /// 如：电芯中必须有4个连续“刀不通”的电芯，不能分散
    /// ZhangKF 2021-4-6
    /// </summary>
    public class IVSeriesCheck
    {
        ///<summary>需要达到的NG电芯的数量</summary>
        public const int MAX_NG_BATTERY_COUNT = 12;

        ///<summary>各NG类型中必须连续达到的电芯数量</summary>
        public const int MAX_NG_SERIES_COUNT = 4;

        ///<summary>点检电芯列表</summary>
        static List<NGBatteryWrapper> _master_battery_list = new List<NGBatteryWrapper>();

        ///<summary>记录点检Master电芯</summary>
        public static void Record(BatterySeat battery)
        {
            NGBatteryWrapper ngBattery = new NGBatteryWrapper();
            ngBattery.NGBattery = battery;

            if (battery.IVRemark.Contains("超规格")) ngBattery.NGType = NGBatteryWrapper.NGTypes.Over;
            else if (battery.IVPLCConduction.Contains("刀不通") && !battery.IVPLCConduction.Contains("探针不通")) ngBattery.NGType = NGBatteryWrapper.NGTypes.Knife;
            else if (battery.IVPLCConduction.Contains("探针不通") && !battery.IVPLCConduction.Contains("刀不通")) ngBattery.NGType = NGBatteryWrapper.NGTypes.Probe;
            else if (battery.IVPLCConduction.Contains("刀不通") || battery.IVPLCConduction.Contains("探针不通")) ngBattery.NGType = NGBatteryWrapper.NGTypes.KnifeOrProbe;

            ngBattery.Index = _master_battery_list.Count;

            _master_battery_list.Add(ngBattery);
        }

        ///<summary>根据业务逻辑判断IV NG电芯是否通过点检</summary>
        public static bool CheckPassed()
        {
            bool over = IsSeries(NGBatteryWrapper.NGTypes.Over);
            bool knife = IsSeries(NGBatteryWrapper.NGTypes.Knife);
            bool probe = IsSeries(NGBatteryWrapper.NGTypes.Probe);

            bool success = over && knife && probe;

            if (success)
            {
                //点检通过后，清除数据
                _master_battery_list.Clear();
            }

            return success;
        }

        ///<summary>检查电芯NG类型是否连续 ngType:当前NG类型</summary>
        private static bool IsSeries(NGBatteryWrapper.NGTypes ngType)
        {
            bool isSeries = false;

            //复制到新数组，防止修改数据源
            NGBatteryWrapper[] arr = new NGBatteryWrapper[_master_battery_list.Count];
            _master_battery_list.CopyTo(arr);

            //TODO:将KnifeOrProbe(刀不通/探针不通)改为当前类型    zxh 2021-04-11
            foreach (NGBatteryWrapper bt in arr)
            {
                if (bt.NGType.ToString().Contains(ngType.ToString())) bt.NGType = ngType;
            }

            //检测指定的NG类型是否在序号上是连续的
            var list = arr.Where(x => x.NGType == ngType).ToList();
            
            ////将KnifeOrProbe(刀不通/探针不通)改为当前类型
            //list.ForEach(x =>
            //{
            //    if (x.NGType.ToString().Contains(ngType.ToString())) x.NGType = ngType;
            //});

            int max = 1;
            for (int i = 0; i < list.Count; i++)
            {
                if (i < (list.Count -1) && list[i].Index == (list[i + 1].Index - 1))
                {
                    max++;
                    if (max >= MAX_NG_SERIES_COUNT) return true;
                }
                else
                {
                    max = 1;
                }
            }

            //if (max >= MAX_NG_SERIES_COUNT) isSeries = true;

            return isSeries;
        }
        //end class
    }

    #region 数据处理辅助类
    /// <summary>
    /// 对当前NG电芯Battery的包装，方便数据统计
    /// ZhangKF 2021-4-6
    /// </summary>
    public class NGBatteryWrapper
    {
        ///<summary>电芯对象</summary>
        public BatterySeat NGBattery
        {
            get;
            set;
        }

        ///<summary>电芯NG类型</summary>
        public enum NGTypes
        {
            None,
            ///<summary>超规格</summary>
            Over,
            ///<summary>刀不通</summary>
            Knife,
            ///<summary>探针不通</summary>
            Probe,
            ///<summary>刀不通或探针不通</summary>
            KnifeOrProbe
        }

        ///<summary>异常类型</summary>
        public NGTypes NGType
        {
            get;
            set;
        }

        ///<summary>电芯加入列表中的顺序</summary>
        public int Index
        {
            get;
            set;
        }
    }
    #endregion
}
