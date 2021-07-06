using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
    /// <summary>
    /// 数据反射类
    /// </summary>
    public class ReflectConvert
    {
        private static readonly object lockModel1 = new object();

        private static readonly object lockModel2 = new object();

        private static readonly object lockModel3 = new object();

        private static readonly object lockModel4 = new object();

        private static readonly object lockModel5 = new object();

        /// <summary>
        /// list转datatable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToTable<T>(List<T> list)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                Type coreType = GetCoreType(propertyInfo.PropertyType);
                dataTable.Columns.Add(propertyInfo.Name, coreType);
            }
            foreach (T item in list)
            {
                object[] array2 = new object[properties.Length];
                for (int j = 0; j < properties.Length; j++)
                {
                    array2[j] = properties[j].GetValue(item, null);
                }
                dataTable.Rows.Add(array2);
            }
            return dataTable;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (!(t != (Type)null) || !IsNullable(t))
            {
                return t;
            }
            if (t.IsValueType)
            {
                return Nullable.GetUnderlyingType(t);
            }
            return t;
        }

        /// <summary>  
        /// DataTable转化为List集合  
        /// </summary>  
        /// <typeparam name="T">实体对象</typeparam>  
        /// <param name="dt">datatable表</param>  
        /// <param name="isStoreDB">是否存入数据库datetime字段，date字段没事，取出不用判断</param>  
        /// <returns>返回list集合</returns>  
        public static List<T> TableToList<T>(DataTable dt, bool isStoreDB = true)
        {
            List<T> list = new List<T>();
            Type typeFromHandle = typeof(T);
            List<string> list2 = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] properties = typeFromHandle.GetProperties();
                T val = Activator.CreateInstance<T>();
                PropertyInfo[] array = properties;
                foreach (PropertyInfo propertyInfo in array)
                {
                    if (dt.Columns.Contains(propertyInfo.Name) && row[propertyInfo.Name] != null && row[propertyInfo.Name] != DBNull.Value && (!isStoreDB || !(propertyInfo.PropertyType == typeof(DateTime)) || !(Convert.ToDateTime(row[propertyInfo.Name]) < Convert.ToDateTime("1753-01-01"))))
                    {
                        try
                        {
                            object value = Convert.ChangeType(row[propertyInfo.Name], propertyInfo.PropertyType);
                            propertyInfo.SetValue(val, value, null);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                list.Add(val);
            }
            return list;
        }

        /// <summary>
        /// 根据字符串获取Model对应属性的值
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="model">对象值</param>
        /// <param name="key">字符串</param>
        /// <returns></returns>
        public static object GetModelValue<T>(object model, string key) where T : class, new()
        {
            Type typeFromHandle = typeof(T);
            PropertyInfo propertyInfo = typeFromHandle.GetProperties().FirstOrDefault((PropertyInfo x) => x.Name.Equals(key));
            if (!(propertyInfo == (PropertyInfo)null))
            {
                return propertyInfo.GetValue(model, null);
            }
            return null;
        }

        public static object ChangeType(object value, Type conversion)
        {
            Type type = conversion;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }
                type = Nullable.GetUnderlyingType(type);
            }
            return Convert.ChangeType(value, type);
        }

        public static T SetModelValue<T>(object obj, string propertyType, object propertyValue) where T : class, new()
        {
            T val = null;
            Type typeFromHandle = typeof(T);
            PropertyInfo[] properties = typeFromHandle.GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                try
                {
                    if (propertyInfo.Name.Equals(propertyType))
                    {
                        object value = ChangeType(propertyValue, propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj, value, null);
                        break;
                    }
                }
                catch (Exception)
                {
                }
            }
            return obj as T;
        }

        /// <summary>
        /// 根据现有 obj 对 Model 进行赋值
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="obj">已实例化Model</param>
        /// <param name="propertyType">赋值属性</param>
        /// <param name="propertyValue">值</param>
        /// <returns></returns>
        public static T SetModelValue<T>(object obj, string propertyType, object propertyValue, ref string errMsg) where T : class, new()
        {
            errMsg = string.Empty;
            T val = null;
            Type typeFromHandle = typeof(T);
            PropertyInfo[] properties = typeFromHandle.GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                try
                {
                    if (propertyInfo.Name.Equals(propertyType))
                    {
                        object value = ChangeType(propertyValue, propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj, value, null);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            return obj as T;
        }

        public static T SetModelValue2<T>(object obj, string propertyType, object propertyValue, ref string errMsg) where T : class, new()
        {
            errMsg = string.Empty;
            T val = null;
            Type typeFromHandle = typeof(T);
            PropertyInfo[] properties = typeFromHandle.GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                try
                {
                    if (propertyInfo.Name.Equals(propertyType))
                    {
                        object value = ChangeType(propertyValue, propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj, value, null);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            return obj as T;
        }

        public static T SetModelValue3<T>(object obj, string propertyType, object propertyValue, ref string errMsg) where T : class, new()
        {
            errMsg = string.Empty;
            T val = null;
            Type typeFromHandle = typeof(T);
            PropertyInfo[] properties = typeFromHandle.GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                try
                {
                    if (propertyInfo.Name.Equals(propertyType))
                    {
                        object value = ChangeType(propertyValue, propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj, value, null);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            return obj as T;
        }

        public static T SetModelValue4<T>(object obj, string propertyType, object propertyValue, ref string errMsg) where T : class, new()
        {
            errMsg = string.Empty;
            T val = null;
            Type typeFromHandle = typeof(T);
            PropertyInfo[] properties = typeFromHandle.GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                try
                {
                    if (propertyInfo.Name.Equals(propertyType))
                    {
                        object value = ChangeType(propertyValue, propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj, value, null);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            return obj as T;
        }
    }

}
