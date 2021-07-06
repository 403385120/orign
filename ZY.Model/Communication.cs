using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Systems;

namespace ZY.Model
{
    public partial class Communication : EsqDbBusinessEntity
    {
        public Communication()
        {
            selectTable = "Communication";
            saveTable = "Communication";
            backupTable = "bak_Communication";
        }
        #region Model
        public string NXConnType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Iden
        {
            set; get;
        }
        /// <summary>
        /// 工厂
        /// </summary>
        public string Factory
        {
            set; get;
        }
        /// <summary>
        /// 通讯类型
        /// </summary>
        public string Code
        {
            set; get;
        }
        /// <summary>
        /// 通讯名称
        /// </summary>
        public string Name
        {
            set; get;
        }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string Ip
        {
            set; get;
        }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int? IpPort
        {
            set; get;
        }
        /// <summary>
        /// 本地端口
        /// </summary>
        public int? IpLocalPort
        {
            set; get;
        }
        /// <summary>
        /// 使用本地端口数量
        /// </summary>
        public int? IpLocalPortNum
        {
            set; get;
        }
        /// <summary>
        /// 串口号
        /// </summary>
        public string ComPort
        {
            set; get;
        }
        /// <summary>
        /// 波特率
        /// </summary>
        public int? ComBaudRate
        {
            set; get;
        }
        /// <summary>
        /// 寄偶数
        /// </summary>
        public string ComParity
        {
            set; get;
        }
        /// <summary>
        /// 停止位
        /// </summary>
        public string ComStopBits
        {
            set; get;
        }
        /// <summary>
        /// 字节数据长度
        /// </summary>
        public int? ComDataBits
        {
            set; get;
        }
        /// <summary>
        /// 发送指令1
        /// </summary>
        public string SendCommand1
        {
            set; get;
        }

        /// <summary>
        /// 指令1备注
        /// </summary>
        public string SendCommand1Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令2
        /// </summary>
        public string SendCommand2
        {
            set; get;
        }

        /// <summary>
        /// 指令2备注
        /// </summary>
        public string SendCommand2Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令3
        /// </summary>
        public string SendCommand3
        {
            set; get;
        }

        /// <summary>
        /// 指令3备注
        /// </summary>
        public string SendCommand3Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令4
        /// </summary>
        public string SendCommand4
        {
            set; get;
        }

        /// <summary>
        /// 指令4备注
        /// </summary>
        public string SendCommand4Remark
        {
            set; get;
        }

        /// <summary>
        /// 是否需要回车
        /// </summary>
        public bool? IsEnter
        {
            set; get;
        }
        /// <summary>
        /// 是否需要换行
        /// </summary>
        public bool? IsLine
        {
            set; get;
        }
        /// <summary>
        /// 是否需要发送16进制
        /// </summary>
        public bool? IsHex
        {
            set; get;
        }
        /// <summary>
        /// 是否需要发送二进制
        /// </summary>
        public bool? IsByte
        {
            set; get;
        }
        /// <summary>
        /// CRC检验否
        /// </summary>
        public bool? IsCRC
        {
            set; get;
        }
        /// <summary>
        /// FCS检验否
        /// </summary>
        public bool? IsFCS
        {
            set; get;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set; get;
        }
        /// <summary>
        /// 扫描次数
        /// </summary>
        public int? ScanCount
        {
            set; get;
        }
        /// <summary>
        /// 扫描间隔时间
        /// </summary>
        public int? ScanInterval
        {
            set; get;
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsUse
        {
            set; get;
        }
        /// <summary>
        /// 是否为Socket
        /// </summary>
        public bool? IsSocket
        {
            set; get;
        }
        /// <summary>
        /// 上下限比较否
        public bool? IsSocketTcp
        {
            set; get;
        }
        /// <summary>
        /// 通讯对象
        /// </summary>
        public int? SocketTcpType
        {
            set; get;
        }
        /// <summary>
        /// 通讯超时
        /// </summary>
        public int? SocketTimeOut
        {
            set; get;
        }
        /// <summary>
        /// 通讯校验时间
        /// </summary>
        public DateTime? CheckDate
        {
            set; get;
        }
        /// <summary>
        /// 通讯校验人
        /// </summary>
        public string CheckUser
        {
            set; get;
        }
        /// <summary>
        /// 通讯校验结果
        /// </summary>
        public string CheckResult
        {
            set; get;
        }
        /// <summary>
        /// 通讯校验数据
        /// </summary>
        public string CheckData
        {
            set; get;
        }
        /// <summary>
        /// 检验图片
        /// </summary>
        public Byte[] InsImage
        {
            set; get;
        }
        /// <summary>
        /// 是否检查通讯正确
        /// </summary>
        public bool? IsCheck
        {
            set; get;
        }
        /// <summary>
        /// 是否加锁
        /// </summary>
        public bool? IsLock
        {
            set; get;
        }
        /// <summary>
        /// 实时关闭通讯连接
        /// </summary>
        public bool? IsClose
        {
            set; get;
        }
        /// <summary>
        /// 发送指令5
        /// </summary>
        public string SendCommand5
        {
            set; get;
        }

        /// <summary>
        /// 指令5备注
        /// </summary>
        public string SendCommand5Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令6
        /// </summary>
        public string SendCommand6
        {
            set; get;
        }

        /// <summary>
        /// 指令6备注
        /// </summary>
        public string SendCommand6Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令7
        /// </summary>
        public string SendCommand7
        {
            set; get;
        }

        /// <summary>
        /// 指令7备注
        /// </summary>
        public string SendCommand7Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令8
        /// </summary>
        public string SendCommand8
        {
            set; get;
        }

        /// <summary>
        /// 指令8备注
        /// </summary>
        public string SendCommand8Remark
        {
            set; get;
        }

        /// <summary>
        /// 上料否
        /// </summary>
        public bool? IsLoading
        {
            set; get;
        }
        /// <summary>
        /// 下料否
        /// </summary>
        public bool? IsUnLoading
        {
            set; get;
        }
        /// <summary>
        /// 通讯仪器品牌
        /// </summary>
        public int? InstrumentBrand
        {
            set; get;
        }
        /// <summary>
        /// 读取延时
        /// </summary>
        public int? ReadDelay
        {
            set; get;
        }
        /// <summary>
        /// 设备表ID
        /// </summary>
        public int? MachineID
        {
            set; get;
        }
        /// <summary>
        /// 写日志否
        /// </summary>
        public bool IsWriteLog
        {
            set; get;
        }
        /// <summary>
        /// 通讯协议
        /// </summary>
        public int? CommunicationType
        {
            set; get;
        }
        /// <summary>
        /// 清缓存否
        /// </summary>
        public bool? IsClearCacheBeforeRead
        {
            set; get;
        }
        /// <summary>
        /// 清零延时
        /// </summary>
        public int? ClearDelayTime { get; set; }
        /// <summary>
        /// 通讯前延时
        /// </summary>
        public int? CommDelayBfTime { get; set; }
        /// <summary>
        /// 通讯中延时
        /// </summary>
        public int? CommDelayIngTime { get; set; }
        /// <summary>
        /// 通讯延时1
        /// </summary>
        public int? CommDelayTime1 { get; set; }
        /// <summary>
        /// 通讯延时2
        /// </summary>
        public int? CommDelayTime2 { get; set; }

        /// <summary>
        /// 换算值
        /// </summary>
        public int? DeviedVal { get; set; }
        /// <summary>
        /// 结束字节长度
        /// </summary>
        public int? EndByteLen { get; set; }
        /// <summary>
        /// 结束字符串长度
        /// </summary>
        public int? EndStrLen { get; set; }
        /// <summary>
        /// 是否反馈PLC
        /// </summary>
        public bool? IsRecPlc { get; set; }
        /// <summary>
        /// 编译规则
        /// </summary>
        public int? Encoding { get; set; }

        /// <summary>
        /// 发送指令9
        /// </summary>
        public string SendCommand9
        {
            set; get;
        }

        /// <summary>
        /// 指令9备注
        /// </summary>
        public string SendCommand9Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令10
        /// </summary>
        public string SendCommand10
        {
            set; get;
        }

        /// <summary>
        /// 指令10备注
        /// </summary>
        public string SendCommand10Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令11
        /// </summary>
        public string SendCommand11
        {
            set; get;
        }

        /// <summary>
        /// 指令11备注
        /// </summary>
        public string SendCommand11Remark
        {
            set; get;
        }

        /// <summary>
        /// 发送指令12
        /// </summary>
        public string SendCommand12
        {
            set; get;
        }

        /// <summary>
        /// 指令12备注
        /// </summary>
        public string SendCommand12Remark
        {
            set; get;
        }
        /// <summary>
        /// socket 缓存字节大小
        /// </summary>
        public int? BufferSize { get; set; }
        /// <summary>
        /// 换算值2
        /// </summary>
        public int? DeviedVal2 { get; set; }

        /// <summary>
        /// 换算值3
        /// </summary>
        public int? DeviedVal3 { get; set; }

        #endregion Model
        public bool IsResult { get; set; }

        public string ReviceData { get; set; }

        public string StrCode { get; set; }

        #region 点检信息
        public string PervTestDate { get; set; }
        public string PervShift { get; set; }
        public string PervTestResult { get; set; }

        public string TestResultString
        {
            get
            {
                return TestResult.GetValueOrDefault(false) ? "OK" : "NG";
            }
            set { TestResult = value.Equals("OK"); }
        }
        public string InstrumentBrandName
        {
            get; set;
        }
        public string SocketTcpTypeName { get; set; }

        public string CommunicationTypeName { get; set; }


        public string Shift
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? TestDate
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Double? WeightsNum
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Double? WeightsMin
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Double? WeightsMax
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        public Double? TestWeight
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool? TestResult
        {
            set; get;
        }
        public string UserID
        {
            set; get;
        }



        #endregion
    }
}
