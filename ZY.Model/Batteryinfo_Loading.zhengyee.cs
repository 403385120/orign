using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Systems;

namespace ZY.Model
{
    public partial class Batteryinfo_Loading : EsqDbBusinessEntity
    {
        public Batteryinfo_Loading()
        {
            selectTable = "Batteryinfo_Loading";
            saveTable = "Batteryinfo_Loading";
            backupTable = "Batteryinfo_Loading";
        }
        public bool IsCheck { get; set; }

        #region Model

        /// <summary>
        /// 序号
        /// </summary>
        public long Iden
        {
            set; get;
        }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string MachineNo
        {
            set; get;
        }
        /// <summary>
        /// 电池编号
        /// </summary>
        public int? BatteryNo
        {
            set; get;
        }
        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductType
        {
            set; get;
        }
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode
        {
            set; get;
        }
        /// <summary>
        /// 结果
        /// </summary>
        public string Flag
        {
            set; get;
        }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate
        {
            set; get;
        }
        /// <summary>
        /// 角号
        /// </summary>
        public int? Horn
        {
            set; get;
        }
        /// <summary>
        /// 算法结果
        /// </summary>
        public int? Algorithm
        {
            set; get;
        }
        /// <summary>
        /// 最小值
        /// </summary>
        public float? Min
        {
            set; get;
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public float? Max
        {
            set; get;
        }
        /// <summary>
        /// L_1
        /// </summary>
        public float? L_1
        {
            set; get;
        }
        /// <summary>
        /// L_2
        /// </summary>
        public float? L_2
        {
            set; get;
        }
        /// <summary
        ///         /// <summary>
        /// L_3
        /// </summary>
        public float? L_3
        {
            set; get;
        }
        /// <summary>
        /// L_4
        /// </summary>
        public float? L_4
        {
            set; get;
        }
        /// <summary>
        /// L_5
        /// </summary>
        public float? L_5
        {
            set; get;
        }
        /// <summary>
        /// L_6
        /// </summary>
        public float? L_6
        {
            set; get;
        }
        /// <summary>
        /// L_7
        /// </summary>
        public float? L_7
        {
            set; get;
        }
        /// <summary>
        /// L_8
        /// </summary>
        public float? L_8
        {
            set; get;
        }
        /// <summary>
        /// L_9
        /// </summary>
        public float? L_9
        {
            set; get;
        }
        /// <summary>
        /// L_10
        /// </summary>
        public float? L_10
        {
            set; get;
        }
        /// <summary>
        /// L_11
        /// </summary>
        public float? L_11
        {
            set; get;
        }
        /// <summary>
        /// L_12
        /// </summary>
        public float? L_12
        {
            set; get;
        }
        /// <summary>
        /// L_13
        /// </summary>
        public float? L_13
        {
            set; get;
        }
        /// <summary>
        /// L_14
        /// </summary>
        public float? L_14
        {
            set; get;
        }
        /// <summary>
        /// L_15
        /// </summary>
        public float? L_15
        {
            set; get;
        }
        /// <summary>
        /// L_16
        /// </summary>
        public float? L_16
        {
            set; get;
        }
        /// <summary>
        /// L_17
        /// </summary>
        public float? L_17
        {
            set; get;
        }
        /// <summary>
        /// L_18
        /// </summary>
        public float? L_18
        {
            set; get;
        }

        /// <summary>
        /// L_19
        /// </summary>
        public float? L_19
        {
            set; get;
        }
        /// <summary>
        /// L_20
        /// </summary>
        public float? L_20
        {
            set; get;
        }
        /// <summary>
        /// L_21
        /// </summary>
        public float? L_21
        {
            set; get;
        }
        /// <summary>
        /// L_22
        /// </summary>
        public float? L_22
        {
            set; get;
        }
        /// <summary>
        /// L_23
        /// </summary>
        public float? L_23
        {
            set; get;
        }
        /// <summary>
        /// L_24
        /// </summary>
        public float? L_24
        {
            set; get;
        }
        /// <summary>
        /// L_25
        /// </summary>
        public float? L_25
        {
            set; get;
        }
        /// <summary>
        /// L_26
        /// </summary>
        public float? L_26
        {
            set; get;
        }
        /// <summary>
        /// L_27
        /// </summary>
        public float? L_27
        {
            set; get;
        }
        /// <summary>
        /// L_28
        /// </summary>
        public float? L_28
        {
            set; get;
        }
        /// <summary>
        /// L_29
        /// </summary>
        public float? L_29
        {
            set; get;
        }
        /// <summary>
        /// L_30
        /// </summary>
        public float? L_30
        {
            set; get;
        }
        /// <summary>
        /// ANG_1
        /// </summary>
        public float? ANG_1
        {
            set; get;
        }
        /// <summary>
        /// ANG_2
        /// </summary>
        public float? ANG_2
        {
            set; get;
        }
        /// <summary>
        /// ANG_3
        /// </summary>
        public float? ANG_3
        {
            set; get;
        }
        /// <summary>
        /// ANG_4
        /// </summary>
        public float? ANG_4
        {
            set; get;
        }
        /// <summary>
        /// ANG_5
        /// </summary>
        public float? ANG_5
        {
            set; get;
        }
        /// <summary>
        /// ANG_6
        /// </summary>
        public float? ANG_6
        {
            set; get;
        }
        /// <summary>
        /// ANG_7
        /// </summary>
        public float? ANG_7
        {
            set; get;
        }
        /// <summary>
        /// ANG_8
        /// </summary>
        public float? ANG_8
        {
            set; get;
        }
        /// <summary>
        /// ANG_9
        /// </summary>
        public float? ANG_9
        {
            set; get;
        }
        /// <summary>
        /// ANG_10
        /// </summary>
        public float? ANG_10
        {
            set; get;
        }
        /// <summary>
        /// ANG_11
        /// </summary>
        public float? ANG_11
        {
            set; get;
        }
        /// <summary>
        /// ANG_12
        /// </summary>
        public float? ANG_12
        {
            set; get;
        }
        /// <summary>
        /// ANG_13
        /// </summary>
        public float? ANG_13
        {
            set; get;
        }
        /// <summary>
        /// ANG_14
        /// </summary>
        public float? ANG_14
        {
            set; get;
        }
        /// <summary>
        /// ANG_15
        /// </summary>
        public float? ANG_15
        {
            set; get;
        }
        /// <summary>
        /// ANG_16
        /// </summary>
        public float? ANG_16
        {
            set; get;
        }
        /// <summary>
        /// ANG_17
        /// </summary>
        public float? ANG_17
        {
            set; get;
        }
        /// <summary>
        /// ANG_18
        /// </summary>
        public float? ANG_18
        {
            set; get;
        }
        /// <summary>
        /// ANG_19
        /// </summary>
        public float? ANG_19
        {
            set; get;
        }

        /// <summary>
        /// ANG_20
        /// </summary>
        public float? ANG_20
        {
            set; get;
        }
        
        /// <summary>
        /// ANG_21
        /// </summary>
        public float? ANG_21
        {
            set; get;
        }
        /// <summary>
        /// ANG_22
        /// </summary>
        public float? ANG_22
        {
            set; get;
        }
        /// <summary>
        /// ANG_23
        /// </summary>
        public float? ANG_23
        {
            set; get;
        }
        /// <summary>
        /// ANG_24
        /// </summary>
        public float? ANG_24
        {
            set; get;
        }
        /// <summary>
        /// ANG_25
        /// </summary>
        public float? ANG_25
        {
            set; get;
        }
        /// <summary>
        /// ANG_26
        /// </summary>
        public float? ANG_26
        {
            set; get;
        }
        /// <summary>
        /// ANG_27
        /// </summary>
        public float? ANG_27
        {
            set; get;
        }
        /// <summary>
        /// ANG_28
        /// </summary>
        public float? ANG_28
        {
            set; get;
        }
        /// <summary>
        /// ANG_29
        /// </summary>
        public float? ANG_29
        {
            set; get;
        }
        /// <summary>
        /// ANG_30
        /// </summary>
        public float? ANG_30
        {
            set; get;
        }
        /// <summary>
        /// ANG_30
        /// </summary>
        public float? isUpload
        {
            set; get;
        }
        /// <summary>
        /// ANG_30
        /// </summary>
        public string UploadReason
        {
            set; get;
        }
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime? UploadDate
        {
            set; get;
        }
        /// <summary>
        /// 员工工号
        /// </summary>
        public string EmployeeID
        {
            set; get;
        }
        #endregion
    }
}
