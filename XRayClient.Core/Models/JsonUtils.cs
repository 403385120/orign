using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XRayClient.Core
{
    public class DateTimeFormatShort : IsoDateTimeConverter
    {
        /// <summary>
        /// DataTime格式为yyyy-MM-dd
        /// </summary>
        public DateTimeFormatShort()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }

    public class DateTimeFormatLong : IsoDateTimeConverter
    {
        /// <summary>
        /// DataTime格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTimeFormatLong()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }

    public class JsonUtils
    {
        /// <summary>
        /// BaseResult返回Data反序列化方法
        /// </summary>
        /// <typeparam name="T">反序列化类型</typeparam>
        /// <param name="obj">转换object对象</param>
        /// <returns></returns>
        public static T ObjectConversiony<T>(object obj)
        {
            try
            {
                string josnData = JsonConvert.SerializeObject(obj);
                return JsonConvert.DeserializeObject<T>(josnData);
            }
            catch
            {
                return default(T);
            }
        }

    }




}
