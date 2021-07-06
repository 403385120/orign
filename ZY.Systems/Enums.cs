using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
    /// <summary>
    /// 枚举审核
    /// </summary>
    public class Enums
    {
        public class EnumberHelper
        {
            /// <summary>
            /// 获取枚举集合的内容
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static List<EnumberEntity> EnumToList<T>()
            {
                List<EnumberEntity> list = new List<EnumberEntity>();
                foreach (object value in Enum.GetValues(typeof(T)))
                {
                    EnumberEntity enumberEntity = new EnumberEntity();
                    object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (customAttributes != null && customAttributes.Length != 0)
                    {
                        DescriptionAttribute descriptionAttribute = customAttributes[0] as DescriptionAttribute;
                        enumberEntity.Desction = descriptionAttribute.Description;
                    }
                    enumberEntity.EnumValue = Convert.ToInt32(value);
                    enumberEntity.EnumName = value.ToString();
                    list.Add(enumberEntity);
                }
                return list;
            }
        }

        public class EnumberEntity
        {
            /// <summary>  
            /// 枚举的描述  
            /// </summary>  
            public string Desction
            {
                get;
                set;
            }

            /// <summary>  
            /// 枚举名称  
            /// </summary>  
            public string EnumName
            {
                get;
                set;
            }

            /// <summary>  
            /// 枚举对象的值  
            /// </summary>  
            public int EnumValue
            {
                get;
                set;
            }

            /// <summary>
            /// 是否提示
            /// </summary>
            public bool IsCheck
            {
                get;
                set;
            }

            /// <summary>
            /// 是否选择
            /// </summary>
            public bool IsSelected
            {
                get;
                set;
            }

            /// <summary>
            /// 资源标识符+方法名
            /// </summary>
            public string IRI
            {
                get;
                set;
            }

            /// <summary>
            /// 执行动作
            /// </summary>
            public string Action
            {
                get;
                set;
            }

            /// <summary>
            /// XML文件模板
            /// </summary>
            public string XmlTemplate
            {
                get;
                set;
            }

            /// <summary>
            /// 引用DLL
            /// </summary>
            public string ImportDll
            {
                get;
                set;
            }

            /// <summary>
            /// 调用命名空间+类名
            /// </summary>
            public string ImportAssemblies
            {
                get;
                set;
            }

            /// <summary>
            /// 调用方法名
            /// </summary>
            public string ImportFunc
            {
                get;
                set;
            }

            /// <summary>
            /// 编译器对象
            /// </summary>
            public CompilerResults CR
            {
                get;
                set;
            }

            /// <summary>
            /// 引用DLL函数的参数个数
            /// </summary>
            public string ImportFunArgsNum
            {
                get;
                set;
            }

            /// <summary>
            /// 引用DLL文件路径
            /// </summary>
            public string ImportAddress
            {
                get;
                set;
            }

            /// <summary>
            /// API接口方法参数个数
            /// </summary>
            public string IRIFunArgsNum
            {
                get;
                set;
            }

            /// <summary>
            /// API接口调用方式：GET/POST
            /// </summary>
            public string IRIFunType
            {
                get;
                set;
            }

            /// <summary>
            /// 接口类型：DLL/URL
            /// </summary>
            public string InterfaceType
            {
                get;
                set;
            }

            /// <summary>
            /// DLL函数索引标志
            /// </summary>
            public string ImportFunIdex
            {
                get;
                set;
            }

            /// <summary>
            /// 设备类型
            /// </summary>
            public int MachineType
            {
                get;
                set;
            }
        }

        /// <summary>
        /// MODBUS数据转换格式
        /// </summary>
        public enum ModbusDataFormat
        {
            /// <summary>
            /// 无类型
            /// </summary>
            [Description("无类型")]
            None,
            /// <summary>
            /// short \ushort
            /// </summary>
            [Description("AB")]
            AB,
            /// <summary>
            /// short\ushort
            /// </summary>
            [Description("BA")]
            BA,
            /// <summary>
            /// int\float ABCD
            /// </summary>
            [Description("ABCD")]
            ABCD,
            /// <summary>
            /// int\float CDAB
            /// </summary>
            [Description("CDAB")]
            CDAB,
            /// <summary>
            /// int\float BADC
            /// </summary>
            [Description("BADC")]
            BADC,
            /// <summary>
            /// int\float DCBA
            /// </summary>
            [Description("DCBA")]
            DCBA,
            /// <summary>
            /// long double ABCDEFGH
            /// </summary>
            [Description("ABCDEFGH")]
            ABCDEFGH,
            /// <summary>
            /// long double GHEFCDAB
            /// </summary>
            [Description("GHEFCDAB")]
            GHEFCDAB,
            /// <summary>
            /// long double BADCFEHG
            /// </summary>
            [Description("BADCFEHG")]
            BADCFEHG,
            /// <summary>
            /// long double HGFEDCBA
            /// </summary>
            [Description("HGFEDCBA")]
            HGFEDCBA
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// 无类型
            /// </summary>
            [Description("无类型")]
            None,
            /// <summary>
            /// 无符号16位整形
            /// </summary>
            [Description("无符号16位")]
            ushortType,
            /// <summary>
            ///             有符号16位整形
            /// </summary>
            [Description("有符号16位")]
            shortType,
            /// <summary>
            /// 无符号32位整形
            /// </summary>
            [Description("无符号32位")]
            uintType,
            /// <summary>
            /// 有符号32位整形
            /// </summary>
            [Description("有符号32位")]
            intType,
            /// <summary>
            /// 无符号64位整形
            /// </summary>
            [Description("无符号64位")]
            ulongType,
            /// <summary>
            /// 有符号64位整形
            /// </summary>
            [Description("有符号64位")]
            longType,
            /// <summary>
            /// 单精度
            /// </summary>
            [Description("单精度")]
            floatType,
            /// <summary>
            /// 双精度
            /// </summary>
            [Description("双精度")]
            doubleType,
            /// <summary>
            /// 字节
            /// </summary>
            [Description("字节")]
            byteType,
            /// <summary>
            /// 布尔
            /// </summary>
            [Description("布尔")]
            boolType,
            /// <summary>
            /// 字符串
            /// </summary>
            [Description("字符串")]
            stringType
        }

        public enum ButtonEditStatus
        {
            Edit,
            Nomral,
            Submit,
            Unsubmit,
            Audit,
            Unaudit,
            Open,
            Close
        }

        public enum WorkflowStatus
        {
            Unsubmitted,
            Submitted,
            Audited,
            PointAudited,
            Opened,
            Closed
        }

        /// <summary>
        /// 编辑状态
        /// </summary>
        public enum EditorStatus
        {
            Normal,
            AddNew,
            Modify,
            Delete,
            Other,
            Right
        }

        public enum LanguageType
        {
            ZH_CN,
            ZH_CHT,
            EN,
            JA
        }

        /// <summary>
        /// 全选/反选/取消选择条件
        /// </summary>
        public enum CheckedRelation
        {
            AllChecked,
            ReverseChecked,
            CancelChecked
        }

        public enum SqlType
        {
            Sql,
            Oracle,
            Access
        }
    }
}
