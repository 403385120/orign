using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XRayClient.Core
{
    public class BatteryTestData
    {
        private string _batteryBarCode = string.Empty;
        private DateTime _startDate = DateTime.Now;
        private string _productLineNO = string.Empty;
        private string _qR_Code = string.Empty;
        private string _pictureCode = string.Empty;
        private string _picturePath = string.Empty;
        private string _pictureName_A1 = string.Empty;
        private float _max_A1 = 0.0f;
        private float _min_A1 = 0.0f;
        private string _l_A1 = string.Empty;
        private int _layer_A1 = 0;
        private string _aNG_A1 = string.Empty;
        private int _angle_A1 = 0;

        private string _pictureName_A2 = string.Empty;
        private float _max_A2 = 0.0f;
        private float _min_A2 = 0.0f;
        private string _l_A2 = string.Empty;
        private int _layer_A2 = 0;
        private string _aNG_A2 = string.Empty;
        private int _angle_A2 = 0;

        private string _pictureName_A3 = string.Empty;
        private float _max_A3 = 0.0f;
        private float _min_A3 = 0.0f;
        private string _l_A3 = string.Empty;
        private int _layer_A3 = 0;
        private string _aNG_A3 = string.Empty;
        private int _angle_A3 = 0;

        private string _pictureName_A4 = string.Empty;
        private float _max_A4 = 0.0f;
        private float _min_A4 = 0.0f;
        private string _l_A4 = string.Empty;
        private int _layer_A4 = 0;
        private string _aNG_A4 = string.Empty;
        private int _angle_A4 = 0;

        private string _marking = string.Empty;
        private string _results = string.Empty;

        private float _polarDisplaceDistance = 0.0f;
        private DateTime _productionEndDate = DateTime.Now;
        private string _updateBy = string.Empty;
        private string _remarks = string.Empty;
        private string _defectCode = string.Empty;


        /// <summary>
        /// 电芯条码
        /// 必填
        /// </summary>
        public string BatteryBarCode
        {
            get
            {
                return this._batteryBarCode;
            }
            set
            {
                this._batteryBarCode = value;
            }
        }

        /// <summary>
        /// 生产时间
        /// 必填
        /// </summary>
        [JsonConverter(typeof(DateTimeFormatLong))]
        public DateTime StartDate
        {
            get
            {
                return this._startDate;
            }
            set
            {
                this._startDate = value;
            }
        }

        /// <summary>
        /// 产线编号
        /// 必填
        /// </summary>
        public string ProductLineNO
        {
            get
            {
                return this._productLineNO;
            }
            set
            {
                this._productLineNO = value;
            }
        }

        /// <summary>
        /// 二维码
        /// 必填
        /// </summary>
        public string QR_Code
        {
            get
            {
                return this._qR_Code;
            }
            set
            {
                this._qR_Code = value;
            }
        }

        /// <summary>
        /// 图片转码
        /// 必填
        /// </summary>
        public string PictureCode
        {
            get
            {
                return this._pictureCode;
            }
            set
            {
                this._pictureCode = value;
            }
        }

        /// <summary>
        /// 图片路径
        /// 必填
        /// </summary>
        public string PicturePath
        {
            get
            {
                return this._picturePath;
            }
            set
            {
                this._picturePath = value;
            }
        }

        /// <summary>
        /// 图片文件名称
        /// 必填
        /// </summary>
        public string PictureName_A1
        {
            get
            {
                return this._pictureName_A1;
            }
            set
            {
                this._pictureName_A1 = value;
            }
        }
        
        /// <summary>
        /// 左上最大值
        /// 必填
        /// </summary>
        public float Max_A1
        {
            get
            {
                return this._max_A1;
            }
            set
            {
                this._max_A1 = value;
            }
        }

        /// <summary>
        /// 左上最小值
        /// 必填
        /// </summary>
        public float Min_A1
        {
            get
            {
                return this._min_A1;
            }
            set
            {
                this._min_A1 = value;
            }
        }
        
        /// <summary>
        /// 左上各层长度
        /// 必填
        /// </summary>
        public string L_A1
        {
            get
            {
                return this._l_A1;
            }
            set
            {
                this._l_A1 = value;
            }
        }
        
        /// <summary>
        /// 左上层数
        /// 必填
        /// </summary>
        public int Layer_A1
        {
            get
            {
                return this._layer_A1;
            }
            set
            {
                this._layer_A1 = value;
            }
        }

        /// <summary>
        /// 左上各层各角度度数
        /// 必填
        /// </summary>
        public string ANG_A1
        {
            get
            {
                return this._aNG_A1;
            }
            set
            {
                this._aNG_A1 = value;
            }
        }

        /// <summary>
        /// 左上总个数
        /// 必填
        /// </summary>
        public int Angle_A1
        {
            get
            {
                return this._angle_A1;
            }
            set
            {
                this._angle_A1 = value;
            }
        }

        
        /// <summary>
        /// 图片文件名称
        /// 必填
        /// </summary>
        public string PictureName_A2
        {
            get
            {
                return this._pictureName_A2;
            }
            set
            {
                this._pictureName_A2 = value;
            }
        }

        /// <summary>
        /// 右下最大值
        /// 必填
        /// </summary>
        public float Max_A2
        {
            get
            {
                return this._max_A2;
            }
            set
            {
                this._max_A2 = value;
            }
        }

        /// <summary>
        /// 右下最小值
        /// 必填
        /// </summary>
        public float Min_A2
        {
            get
            {
                return this._min_A2;
            }
            set
            {
                this._min_A2 = value;
            }
        }

        /// <summary>
        /// 右下各层长度
        /// 必填
        /// </summary>
        public string L_A2
        {
            get
            {
                return this._l_A2;
            }
            set
            {
                this._l_A2 = value;
            }
        }

        /// <summary>
        /// 右下层数
        /// 必填
        /// </summary>
        public int Layer_A2
        {
            get
            {
                return this._layer_A2;
            }
            set
            {
                this._layer_A2 = value;
            }
        }

        /// <summary>
        /// 右下各层各角度度数
        /// 必填
        /// </summary>
        public string ANG_A2
        {
            get
            {
                return this._aNG_A2;
            }
            set
            {
                this._aNG_A2 = value;
            }
        }

        /// <summary>
        /// 右下总个数
        /// 必填
        /// </summary>
        public int Angle_A2
        {
            get
            {
                return this._angle_A2;
            }
            set
            {
                this._angle_A2 = value;
            }
        }



        /// <summary>
        /// 图片文件名称
        /// 必填
        /// </summary>
        public string PictureName_A3
        {
            get
            {
                return this._pictureName_A3;
            }
            set
            {
                this._pictureName_A3 = value;
            }
        }

        /// <summary>
        /// 右上最大值
        /// 必填
        /// </summary>
        public float Max_A3
        {
            get
            {
                return this._max_A3;
            }
            set
            {
                this._max_A3 = value;
            }
        }

        /// <summary>
        /// 右上最小值
        /// 必填
        /// </summary>
        public float Min_A3
        {
            get
            {
                return this._min_A3;
            }
            set
            {
                this._min_A3 = value;
            }
        }

        /// <summary>
        /// 右上各层长度
        /// 必填
        /// </summary>
        public string L_A3
        {
            get
            {
                return this._l_A3;
            }
            set
            {
                this._l_A3 = value;
            }
        }

        /// <summary>
        /// 右上层数
        /// 必填
        /// </summary>
        public int Layer_A3
        {
            get
            {
                return this._layer_A3;
            }
            set
            {
                this._layer_A3 = value;
            }
        }

        /// <summary>
        /// 右上各层各角度度数
        /// 必填
        /// </summary>
        public string ANG_A3
        {
            get
            {
                return this._aNG_A3;
            }
            set
            {
                this._aNG_A3 = value;
            }
        }

        /// <summary>
        /// 右上总个数
        /// 必填
        /// </summary>
        public int Angle_A3
        {
            get
            {
                return this._angle_A3;
            }
            set
            {
                this._angle_A3 = value;
            }
        }

        
        /// <summary>
        /// 图片文件名称
        /// 必填
        /// </summary>
        public string PictureName_A4
        {
            get
            {
                return this._pictureName_A4;
            }
            set
            {
                this._pictureName_A4 = value;
            }
        }

        /// <summary>
        /// 左下最大值
        /// 必填
        /// </summary>
        public float Max_A4
        {
            get
            {
                return this._max_A4;
            }
            set
            {
                this._max_A4 = value;
            }
        }

        /// <summary>
        /// 左下最小值
        /// 必填
        /// </summary>
        public float Min_A4
        {
            get
            {
                return this._min_A4;
            }
            set
            {
                this._min_A4 = value;
            }
        }

        /// <summary>
        /// 左下各层长度
        /// 必填
        /// </summary>
        public string L_A4
        {
            get
            {
                return this._l_A4;
            }
            set
            {
                this._l_A4 = value;
            }
        }

        /// <summary>
        /// 左下层数
        /// 必填
        /// </summary>
        public int Layer_A4
        {
            get
            {
                return this._layer_A4;
            }
            set
            {
                this._layer_A4 = value;
            }
        }

        /// <summary>
        /// 左下各层各角度度数
        /// 必填
        /// </summary>
        public string ANG_A4
        {
            get
            {
                return this._aNG_A4;
            }
            set
            {
                this._aNG_A4 = value;
            }
        }

        /// <summary>
        /// 左下总个数
        /// 必填
        /// </summary>
        public int Angle_A4
        {
            get
            {
                return this._angle_A4;
            }
            set
            {
                this._angle_A4 = value;
            }
        }
        
        /// <summary>
        /// Marking 标志
        /// 必填
        /// </summary>
        public string Marking
        {
            get
            {
                return this._marking;
            }
            set
            {
                this._marking = value;
            }
        }

        /// <summary>
        /// 测试结果
        /// 必填
        /// </summary>
        public string Results
        {
            get
            {
                return this._results;
            }
            set
            {
                this._results = value;
            }
        }

        /// <summary>
        /// 阴阳极错位距离
        /// 必填
        /// </summary>
        public float PolarDisplaceDistance
        {
            get
            {
                return this._polarDisplaceDistance;
            }
            set
            {
                this._polarDisplaceDistance = value;
            }
        }

        /// <summary>
        /// 生产结束时间
        /// 必填
        /// </summary>
        [JsonConverter(typeof(DateTimeFormatLong))]
        public DateTime ProductionEndDate
        {
            get
            {
                return this._productionEndDate;
            }
            set
            {
                this._productionEndDate = value;
            }
        }

        /// <summary>
        /// 更新者
        /// 必填
        /// </summary>
        public string UpdateBy
        {
            get
            {
                return this._updateBy;
            }
            set
            {
                this._updateBy = value;
            }
        }

        /// <summary>
        /// 备注
        /// 可空
        /// </summary>
        public string Remarks
        {
            get
            {
                return this._remarks;
            }
            set
            {
                this._remarks = value;
            }
        }

        /// <summary>
        /// 缺陷代码（OK&缺陷代）
        /// 必填
        /// </summary>
        public string DefectCode
        {
            get
            {
                return this._defectCode;
            }
            set
            {
                this._defectCode = value;
            }
        }
    }
}
