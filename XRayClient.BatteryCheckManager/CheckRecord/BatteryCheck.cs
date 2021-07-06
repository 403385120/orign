using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shuyz.Framework.Mvvm;

namespace XRayClient.BatteryCheckManager
{
    public class BatteryCheck : ObservableObject
    {
        private int _iden=-1;
        private string _productSN=string.Empty;
        private string _model = string.Empty;
        private int _quality = 0;
        private string _ngReason = string.Empty;
        private DateTime _productDate = DateTime.Now;
        private DateTime _outTime = DateTime.Now;
        private string _operator = string.Empty;
        
        private string _resultPath = string.Empty;
        private string _mesImagePath = string.Empty;

        private int _checkMode = 0;
        private float _thickness = 0;
        private int _a1_PhotographResult = 0;
        private string _a1_OriginalImagePath = string.Empty;
        private string _a1_ResultImagePath = string.Empty;
        private float _a1_Min = 0;
        private float _a1_Max = 0; 
        private float _a1_Distance1 = 0;
        private float _a1_Angle1 = 0;
        private float _a1_Distance2 = 0;
        private float _a1_Angle2 = 0;
        private float _a1_Distance3 = 0;
        private float _a1_Angle3 = 0;
        private float _a1_Distance4 = 0;
        private float _a1_Angle4 = 0;
        private float _a1_Distance5 = 0;
        private float _a1_Angle5 = 0;
        private float _a1_Distance6 = 0;
        private float _a1_Angle6 = 0;
        private float _a1_Distance7 = 0;
        private float _a1_Angle7 = 0;
        private float _a1_Distance8 = 0;
        private float _a1_Angle8 = 0;
        private float _a1_Distance9 = 0;
        private float _a1_Angle9 = 0;
        private float _a1_Distance10 = 0;
        private float _a1_Angle10 = 0;
        private float _a1_Distance11 = 0;
        private float _a1_Angle11 = 0;
        private float _a1_Distance12 = 0;
        private float _a1_Angle12 = 0;
        private float _a1_Distance13 = 0;
        private float _a1_Angle13 = 0;
        private float _a1_Distance14 = 0;
        private float _a1_Angle14 = 0;
        private float _a1_Distance15 = 0;
        private float _a1_Angle15 = 0;
        private float _a1_Distance16 = 0;
        private float _a1_Angle16 = 0;
        private float _a1_Distance17 = 0;
        private float _a1_Angle17 = 0;
        private float _a1_Distance18 = 0;
        private float _a1_Angle18 = 0;
        private float _a1_Distance19 = 0;
        private float _a1_Angle19 = 0;
        private float _a1_Distance20 = 0;
        private float _a1_Angle20 = 0;
        private float _a1_Distance21 = 0;
        private float _a1_Angle21 = 0;
        private float _a1_Distance22 = 0;
        private float _a1_Angle22 = 0;
        private float _a1_Distance23 = 0;
        private float _a1_Angle23 = 0;
        private float _a1_Distance24 = 0;
        private float _a1_Angle24 = 0;
        private float _a1_Distance25 = 0;
        private float _a1_Angle25 = 0;
        private float _a1_Distance26 = 0;
        private float _a1_Angle26 = 0;
        private float _a1_Distance27 = 0;
        private float _a1_Angle27 = 0;
        private float _a1_Distance28 = 0;
        private float _a1_Angle28 = 0;
        private float _a1_Distance29 = 0;
        private float _a1_Angle29 = 0;
        private float _a1_Distance30 = 0;
        private float _a1_Angle30 = 0;

        private int _a2_PhotographResult = 0;
        private string _a2_OriginalImagePath = string.Empty;
        private string _a2_ResultImagePath = string.Empty;
        private float _a2_Min = 0;
        private float _a2_Max = 0;
        private float _a2_Distance1 = 0;
        private float _a2_Angle1 = 0;
        private float _a2_Distance2 = 0;
        private float _a2_Angle2 = 0;
        private float _a2_Distance3 = 0;
        private float _a2_Angle3 = 0;
        private float _a2_Distance4 = 0;
        private float _a2_Angle4 = 0;
        private float _a2_Distance5 = 0;
        private float _a2_Angle5 = 0;
        private float _a2_Distance6 = 0;
        private float _a2_Angle6 = 0;
        private float _a2_Distance7 = 0;
        private float _a2_Angle7 = 0;
        private float _a2_Distance8 = 0;
        private float _a2_Angle8 = 0;
        private float _a2_Distance9 = 0;
        private float _a2_Angle9 = 0;
        private float _a2_Distance10 = 0;
        private float _a2_Angle10 = 0;
        private float _a2_Distance11 = 0;
        private float _a2_Angle11 = 0;
        private float _a2_Distance12 = 0;
        private float _a2_Angle12 = 0;
        private float _a2_Distance13 = 0;
        private float _a2_Angle13 = 0;
        private float _a2_Distance14 = 0;
        private float _a2_Angle14 = 0;
        private float _a2_Distance15 = 0;
        private float _a2_Angle15 = 0;
        private float _a2_Distance16 = 0;
        private float _a2_Angle16 = 0;
        private float _a2_Distance17 = 0;
        private float _a2_Angle17 = 0;
        private float _a2_Distance18 = 0;
        private float _a2_Angle18 = 0;
        private float _a2_Distance19 = 0;
        private float _a2_Angle19 = 0;
        private float _a2_Distance20 = 0;
        private float _a2_Angle20 = 0;
        private float _a2_Distance21 = 0;
        private float _a2_Angle21 = 0;
        private float _a2_Distance22 = 0;
        private float _a2_Angle22 = 0;
        private float _a2_Distance23 = 0;
        private float _a2_Angle23 = 0;
        private float _a2_Distance24 = 0;
        private float _a2_Angle24 = 0;
        private float _a2_Distance25 = 0;
        private float _a2_Angle25 = 0;
        private float _a2_Distance26 = 0;
        private float _a2_Angle26 = 0;
        private float _a2_Distance27 = 0;
        private float _a2_Angle27 = 0;
        private float _a2_Distance28 = 0;
        private float _a2_Angle28 = 0;
        private float _a2_Distance29 = 0;
        private float _a2_Angle29 = 0;
        private float _a2_Distance30 = 0;
        private float _a2_Angle30 = 0;

        private int _a3_PhotographResult = 0;
        private string _a3_OriginalImagePath = string.Empty;
        private string _a3_ResultImagePath = string.Empty;
        private float _a3_Min = 0;
        private float _a3_Max = 0;
        private float _a3_Distance1 = 0;
        private float _a3_Angle1 = 0;
        private float _a3_Distance2 = 0;
        private float _a3_Angle2 = 0;
        private float _a3_Distance3 = 0;
        private float _a3_Angle3 = 0;
        private float _a3_Distance4 = 0;
        private float _a3_Angle4 = 0;
        private float _a3_Distance5 = 0;
        private float _a3_Angle5 = 0;
        private float _a3_Distance6 = 0;
        private float _a3_Angle6 = 0;
        private float _a3_Distance7 = 0;
        private float _a3_Angle7 = 0;
        private float _a3_Distance8 = 0;
        private float _a3_Angle8 = 0;
        private float _a3_Distance9 = 0;
        private float _a3_Angle9 = 0;
        private float _a3_Distance10 = 0;
        private float _a3_Angle10 = 0;
        private float _a3_Distance11 = 0;
        private float _a3_Angle11 = 0;
        private float _a3_Distance12 = 0;
        private float _a3_Angle12 = 0;
        private float _a3_Distance13 = 0;
        private float _a3_Angle13 = 0;
        private float _a3_Distance14 = 0;
        private float _a3_Angle14 = 0;
        private float _a3_Distance15 = 0;
        private float _a3_Angle15 = 0;
        private float _a3_Distance16 = 0;
        private float _a3_Angle16 = 0;
        private float _a3_Distance17 = 0;
        private float _a3_Angle17 = 0;
        private float _a3_Distance18 = 0;
        private float _a3_Angle18 = 0;
        private float _a3_Distance19 = 0;
        private float _a3_Angle19 = 0;
        private float _a3_Distance20 = 0;
        private float _a3_Angle20 = 0;
        private float _a3_Distance21 = 0;
        private float _a3_Angle21 = 0;
        private float _a3_Distance22 = 0;
        private float _a3_Angle22 = 0;
        private float _a3_Distance23 = 0;
        private float _a3_Angle23 = 0;
        private float _a3_Distance24 = 0;
        private float _a3_Angle24 = 0;
        private float _a3_Distance25 = 0;
        private float _a3_Angle25 = 0;
        private float _a3_Distance26 = 0;
        private float _a3_Angle26 = 0;
        private float _a3_Distance27 = 0;
        private float _a3_Angle27 = 0;
        private float _a3_Distance28 = 0;
        private float _a3_Angle28 = 0;
        private float _a3_Distance29 = 0;
        private float _a3_Angle29 = 0;
        private float _a3_Distance30 = 0;
        private float _a3_Angle30 = 0;

        private int _a4_PhotographResult = 0;
        private string _a4_OriginalImagePath = string.Empty;
        private string _a4_ResultImagePath = string.Empty;
        private float _a4_Min = 0;
        private float _a4_Max = 0;
        private float _a4_Distance1 = 0;
        private float _a4_Angle1 = 0;
        private float _a4_Distance2 = 0;
        private float _a4_Angle2 = 0;
        private float _a4_Distance3 = 0;
        private float _a4_Angle3 = 0;
        private float _a4_Distance4 = 0;
        private float _a4_Angle4 = 0;
        private float _a4_Distance5 = 0;
        private float _a4_Angle5 = 0;
        private float _a4_Distance6 = 0;
        private float _a4_Angle6 = 0;
        private float _a4_Distance7 = 0;
        private float _a4_Angle7 = 0;
        private float _a4_Distance8 = 0;
        private float _a4_Angle8 = 0;
        private float _a4_Distance9 = 0;
        private float _a4_Angle9 = 0;
        private float _a4_Distance10 = 0;
        private float _a4_Angle10 = 0;
        private float _a4_Distance11 = 0;
        private float _a4_Angle11 = 0;
        private float _a4_Distance12 = 0;
        private float _a4_Angle12 = 0;
        private float _a4_Distance13 = 0;
        private float _a4_Angle13 = 0;
        private float _a4_Distance14 = 0;
        private float _a4_Angle14 = 0;
        private float _a4_Distance15 = 0;
        private float _a4_Angle15 = 0;
        private float _a4_Distance16 = 0;
        private float _a4_Angle16 = 0;
        private float _a4_Distance17 = 0;
        private float _a4_Angle17 = 0;
        private float _a4_Distance18 = 0;
        private float _a4_Angle18 = 0;
        private float _a4_Distance19 = 0;
        private float _a4_Angle19 = 0;
        private float _a4_Distance20 = 0;
        private float _a4_Angle20 = 0;
        private float _a4_Distance21 = 0;
        private float _a4_Angle21 = 0;
        private float _a4_Distance22 = 0;
        private float _a4_Angle22 = 0;
        private float _a4_Distance23 = 0;
        private float _a4_Angle23 = 0;
        private float _a4_Distance24 = 0;
        private float _a4_Angle24 = 0;
        private float _a4_Distance25 = 0;
        private float _a4_Angle25 = 0;
        private float _a4_Distance26 = 0;
        private float _a4_Angle26 = 0;
        private float _a4_Distance27 = 0;
        private float _a4_Angle27 = 0;
        private float _a4_Distance28 = 0;
        private float _a4_Angle28 = 0;
        private float _a4_Distance29 = 0;
        private float _a4_Angle29 = 0;
        private float _a4_Distance30 = 0;
        private float _a4_Angle30 = 0;

        private int _recheckState = 0;
        private DateTime _recheckTime = DateTime.Now;
        private string _recheckUserID = string.Empty;
        private DateTime _FQATime = DateTime.Now;
        private string _FQAUser = string.Empty;
        
        public BatteryCheck()
        {

        }

        /// <summary>
        /// 数据库记录ID
        /// </summary>
        public int Iden
        {
            get { return this._iden; }
            set
            {
                this._iden = value;
                RaisePropertyChanged("Iden");
            }
        }

        /// <summary>
        /// 电池条码
        /// </summary>
        public string ProductSN
        {
            get { return this._productSN; }
            set
            {
                this._productSN = value;
                //RaisePropertyChanged("ProductSN");
            }
        }

        /// <summary>
        /// 生产型号
        /// </summary>
        public string Model
        {
            get { return this._model; }
            set
            {
                this._model = value;
            }
        }

        /// <summary>
        /// 合格类型
        /// 1表示合格，0表示不合格
        /// </summary>
        public int Quality
        {
            get { return this._quality; }
            set
            {
                this._quality = value;
            }
        }

        /// <summary>
        /// 不合格原因
        /// </summary>
        public string NGreason
        {
            get { return this._ngReason; }
            set
            {
                this._ngReason = value;
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime ProductDate
        {
            get { return this._productDate; }
            set
            {
                this._productDate = value;
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime OutTime
        {
            get { return this._outTime; }
            set
            {
                this._outTime = value;
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Operator
        {
            get { return this._operator; }
            set
            {
                this._operator = value;
            }
        }
        
        /// <summary>
        /// 结果图路径
        /// </summary>
        public string ResultPath
        {
            get { return this._resultPath; }
            set
            {
                this._resultPath = value;
            }
        }

        /// <summary>
        /// mes 存储路径
        /// </summary>
        public string MesImagePath
        {
            get { return this._mesImagePath; }
            set
            {
                this._mesImagePath = value;
            }
        }

        public int CheckMode
        {
            get { return this._checkMode; }
            set
            {
                this._checkMode = value;
            }
        }


        /// <summary>
        /// 厚度
        /// </summary>
        public float Thickness
        {
            get { return this._thickness; }
            set
            {
                this._thickness = value;
            }
        }

        /// <summary>
        /// A角拍照结果
        /// </summary>
        public int A1_PhotographResult
        {
            get { return this._a1_PhotographResult; }
            set
            {
                this._a1_PhotographResult = value;
            }
        }

        /// <summary>
        /// A角原始图Path
        /// </summary>
        public string A1_OriginalImagePath
        {
            get { return this._a1_OriginalImagePath; }
            set
            {
                this._a1_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// A角结果图Path
        /// </summary>
        public string A1_ResultImagePath
        {
            get { return this._a1_ResultImagePath; }
            set
            {
                this._a1_ResultImagePath = value;
            }
        }

        /// <summary>
        /// A角最小
        /// </summary>
        public float A1_Min
        {
            get { return this._a1_Min; }
            set
            {
                this._a1_Min = value;
            }
        }

        /// <summary>
        /// A角最大
        /// </summary>
        public float A1_Max
        {
            get { return this._a1_Max; }
            set
            {
                this._a1_Max = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_1
        /// </summary>
        public float A1_Distance1
        {
            get { return this._a1_Distance1; }
            set
            {
                this._a1_Distance1 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_1
        /// </summary>
        public float A1_Angle1
        {
            get { return this._a1_Angle1; }
            set
            {
                this._a1_Angle1 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_2
        /// </summary>
        public float A1_Distance2
        {
            get { return this._a1_Distance2; }
            set
            {
                this._a1_Distance2 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_2
        /// </summary>
        public float A1_Angle2
        {
            get { return this._a1_Angle2; }
            set
            {
                this._a1_Angle2 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_3
        /// </summary>
        public float A1_Distance3
        {
            get { return this._a1_Distance3; }
            set
            {
                this._a1_Distance3 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_3
        /// </summary>
        public float A1_Angle3
        {
            get { return this._a1_Angle3; }
            set
            {
                this._a1_Angle3 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_4
        /// </summary>
        public float A1_Distance4
        {
            get { return this._a1_Distance4; }
            set
            {
                this._a1_Distance4 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_4
        /// </summary>
        public float A1_Angle4
        {
            get { return this._a1_Angle4; }
            set
            {
                this._a1_Angle4 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_5
        /// </summary>
        public float A1_Distance5
        {
            get { return this._a1_Distance5; }
            set
            {
                this._a1_Distance5 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_5
        /// </summary>
        public float A1_Angle5
        {
            get { return this._a1_Angle5; }
            set
            {
                this._a1_Angle5 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_6
        /// </summary>
        public float A1_Distance6
        {
            get { return this._a1_Distance6; }
            set
            {
                this._a1_Distance6 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_6
        /// </summary>
        public float A1_Angle6
        {
            get { return this._a1_Angle6; }
            set
            {
                this._a1_Angle6 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_7
        /// </summary>
        public float A1_Distance7
        {
            get { return this._a1_Distance7; }
            set
            {
                this._a1_Distance7 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_7
        /// </summary>
        public float A1_Angle7
        {
            get { return this._a1_Angle7; }
            set
            {
                this._a1_Angle7 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_8
        /// </summary>
        public float A1_Distance8
        {
            get { return this._a1_Distance8; }
            set
            {
                this._a1_Distance8 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_8
        /// </summary>
        public float A1_Angle8
        {
            get { return this._a1_Angle8; }
            set
            {
                this._a1_Angle8 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_9
        /// </summary>
        public float A1_Distance9
        {
            get { return this._a1_Distance9; }
            set
            {
                this._a1_Distance9 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_9
        /// </summary>
        public float A1_Angle9
        {
            get { return this._a1_Angle9; }
            set
            {
                this._a1_Angle9 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_10
        /// </summary>
        public float A1_Distance10
        {
            get { return this._a1_Distance10; }
            set
            {
                this._a1_Distance10 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_10
        /// </summary>
        public float A1_Angle10
        {
            get { return this._a1_Angle10; }
            set
            {
                this._a1_Angle10 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_11
        /// </summary>
        public float A1_Distance11
        {
            get { return this._a1_Distance11; }
            set
            {
                this._a1_Distance11 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_11
        /// </summary>
        public float A1_Angle11
        {
            get { return this._a1_Angle11; }
            set
            {
                this._a1_Angle11 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_12
        /// </summary>
        public float A1_Distance12
        {
            get { return this._a1_Distance12; }
            set
            {
                this._a1_Distance12 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_12
        /// </summary>
        public float A1_Angle12
        {
            get { return this._a1_Angle12; }
            set
            {
                this._a1_Angle12 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_13
        /// </summary>
        public float A1_Distance13
        {
            get { return this._a1_Distance13; }
            set
            {
                this._a1_Distance13 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_13
        /// </summary>
        public float A1_Angle13
        {
            get { return this._a1_Angle13; }
            set
            {
                this._a1_Angle13 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_14
        /// </summary>
        public float A1_Distance14
        {
            get { return this._a1_Distance14; }
            set
            {
                this._a1_Distance14 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_14
        /// </summary>
        public float A1_Angle14
        {
            get { return this._a1_Angle14; }
            set
            {
                this._a1_Angle14 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_15
        /// </summary>
        public float A1_Distance15
        {
            get { return this._a1_Distance15; }
            set
            {
                this._a1_Distance15 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_15
        /// </summary>
        public float A1_Angle15
        {
            get { return this._a1_Angle15; }
            set
            {
                this._a1_Angle15 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_16
        /// </summary>
        public float A1_Distance16
        {
            get { return this._a1_Distance16; }
            set
            {
                this._a1_Distance16 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_16
        /// </summary>
        public float A1_Angle16
        {
            get { return this._a1_Angle16; }
            set
            {
                this._a1_Angle16 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_17
        /// </summary>
        public float A1_Distance17
        {
            get { return this._a1_Distance17; }
            set
            {
                this._a1_Distance17 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_17
        /// </summary>
        public float A1_Angle17
        {
            get { return this._a1_Angle17; }
            set
            {
                this._a1_Angle17 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_18
        /// </summary>
        public float A1_Distance18
        {
            get { return this._a1_Distance18; }
            set
            {
                this._a1_Distance18 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_18
        /// </summary>
        public float A1_Angle18
        {
            get { return this._a1_Angle18; }
            set
            {
                this._a1_Angle18 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_19
        /// </summary>
        public float A1_Distance19
        {
            get { return this._a1_Distance19; }
            set
            {
                this._a1_Distance19 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_19
        /// </summary>
        public float A1_Angle19
        {
            get { return this._a1_Angle19; }
            set
            {
                this._a1_Angle19 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_20
        /// </summary>
        public float A1_Distance20
        {
            get { return this._a1_Distance20; }
            set
            {
                this._a1_Distance20 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_20
        /// </summary>
        public float A1_Angle20
        {
            get { return this._a1_Angle20; }
            set
            {
                this._a1_Angle20 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_21
        /// </summary>
        public float A1_Distance21
        {
            get { return this._a1_Distance21; }
            set
            {
                this._a1_Distance21 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_21
        /// </summary>
        public float A1_Angle21
        {
            get { return this._a1_Angle21; }
            set
            {
                this._a1_Angle21 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_22
        /// </summary>
        public float A1_Distance22
        {
            get { return this._a1_Distance22; }
            set
            {
                this._a1_Distance22 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_22
        /// </summary>
        public float A1_Angle22
        {
            get { return this._a1_Angle22; }
            set
            {
                this._a1_Angle22 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_23
        /// </summary>
        public float A1_Distance23
        {
            get { return this._a1_Distance23; }
            set
            {
                this._a1_Distance23 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_23
        /// </summary>
        public float A1_Angle23
        {
            get { return this._a1_Angle23; }
            set
            {
                this._a1_Angle23 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_24
        /// </summary>
        public float A1_Distance24
        {
            get { return this._a1_Distance24; }
            set
            {
                this._a1_Distance24 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_24
        /// </summary>
        public float A1_Angle24
        {
            get { return this._a1_Angle24; }
            set
            {
                this._a1_Angle24 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_25
        /// </summary>
        public float A1_Distance25
        {
            get { return this._a1_Distance25; }
            set
            {
                this._a1_Distance25 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_25
        /// </summary>
        public float A1_Angle25
        {
            get { return this._a1_Angle25; }
            set
            {
                this._a1_Angle25 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_26
        /// </summary>
        public float A1_Distance26
        {
            get { return this._a1_Distance26; }
            set
            {
                this._a1_Distance26 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_26
        /// </summary>
        public float A1_Angle26
        {
            get { return this._a1_Angle26; }
            set
            {
                this._a1_Angle26 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_27
        /// </summary>
        public float A1_Distance27
        {
            get { return this._a1_Distance27; }
            set
            {
                this._a1_Distance27 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_27
        /// </summary>
        public float A1_Angle27
        {
            get { return this._a1_Angle27; }
            set
            {
                this._a1_Angle27 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_28
        /// </summary>
        public float A1_Distance28
        {
            get { return this._a1_Distance28; }
            set
            {
                this._a1_Distance28 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_28
        /// </summary>
        public float A1_Angle28
        {
            get { return this._a1_Angle28; }
            set
            {
                this._a1_Angle28 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_29
        /// </summary>
        public float A1_Distance29
        {
            get { return this._a1_Distance29; }
            set
            {
                this._a1_Distance29 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_29
        /// </summary>
        public float A1_Angle29
        {
            get { return this._a1_Angle29; }
            set
            {
                this._a1_Angle29 = value;
            }
        }

        /// <summary>
        /// A角阴阳极错位距离_30
        /// </summary>
        public float A1_Distance30
        {
            get { return this._a1_Distance30; }
            set
            {
                this._a1_Distance30 = value;
            }
        }

        /// <summary>
        /// A角阳极轮廓与阴极的角度_30
        /// </summary>
        public float A1_Angle30
        {
            get { return this._a1_Angle30; }
            set
            {
                this._a1_Angle30 = value;
            }
        }

        /// <summary>
        /// B角拍照结果
        /// </summary>
        public int A2_PhotographResult
        {
            get { return this._a2_PhotographResult; }
            set
            {
                this._a2_PhotographResult = value;
            }
        }

        /// <summary>
        /// B角原始图Path
        /// </summary>
        public string A2_OriginalImagePath
        {
            get { return this._a2_OriginalImagePath; }
            set
            {
                this._a2_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// B角结果图Path
        /// </summary>
        public string A2_ResultImagePath
        {
            get { return this._a2_ResultImagePath; }
            set
            {
                this._a2_ResultImagePath = value;
            }
        }

        /// <summary>
        /// B角最小
        /// </summary>
        public float A2_Min
        {
            get { return this._a2_Min; }
            set
            {
                this._a2_Min = value;
            }
        }

        /// <summary>
        /// B角最大
        /// </summary>
        public float A2_Max
        {
            get { return this._a2_Max; }
            set
            {
                this._a2_Max = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_1
        /// </summary>
        public float A2_Distance1
        {
            get { return this._a2_Distance1; }
            set
            {
                this._a2_Distance1 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_1
        /// </summary>
        public float A2_Angle1
        {
            get { return this._a2_Angle1; }
            set
            {
                this._a2_Angle1 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_2
        /// </summary>
        public float A2_Distance2
        {
            get { return this._a2_Distance2; }
            set
            {
                this._a2_Distance2 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_2
        /// </summary>
        public float A2_Angle2
        {
            get { return this._a2_Angle2; }
            set
            {
                this._a2_Angle2 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_3
        /// </summary>
        public float A2_Distance3
        {
            get { return this._a2_Distance3; }
            set
            {
                this._a2_Distance3 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_3
        /// </summary>
        public float A2_Angle3
        {
            get { return this._a2_Angle3; }
            set
            {
                this._a2_Angle3 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_4
        /// </summary>
        public float A2_Distance4
        {
            get { return this._a2_Distance4; }
            set
            {
                this._a2_Distance4 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_4
        /// </summary>
        public float A2_Angle4
        {
            get { return this._a2_Angle4; }
            set
            {
                this._a2_Angle4 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_5
        /// </summary>
        public float A2_Distance5
        {
            get { return this._a2_Distance5; }
            set
            {
                this._a2_Distance5 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_5
        /// </summary>
        public float A2_Angle5
        {
            get { return this._a2_Angle5; }
            set
            {
                this._a2_Angle5 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_6
        /// </summary>
        public float A2_Distance6
        {
            get { return this._a2_Distance6; }
            set
            {
                this._a2_Distance6 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_6
        /// </summary>
        public float A2_Angle6
        {
            get { return this._a2_Angle6; }
            set
            {
                this._a2_Angle6 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_7
        /// </summary>
        public float A2_Distance7
        {
            get { return this._a2_Distance7; }
            set
            {
                this._a2_Distance7 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_7
        /// </summary>
        public float A2_Angle7
        {
            get { return this._a2_Angle7; }
            set
            {
                this._a2_Angle7 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_8
        /// </summary>
        public float A2_Distance8
        {
            get { return this._a2_Distance8; }
            set
            {
                this._a2_Distance8 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_8
        /// </summary>
        public float A2_Angle8
        {
            get { return this._a2_Angle8; }
            set
            {
                this._a2_Angle8 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_9
        /// </summary>
        public float A2_Distance9
        {
            get { return this._a2_Distance9; }
            set
            {
                this._a2_Distance9 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_9
        /// </summary>
        public float A2_Angle9
        {
            get { return this._a2_Angle9; }
            set
            {
                this._a2_Angle9 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_10
        /// </summary>
        public float A2_Distance10
        {
            get { return this._a2_Distance10; }
            set
            {
                this._a2_Distance10 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_10
        /// </summary>
        public float A2_Angle10
        {
            get { return this._a2_Angle10; }
            set
            {
                this._a2_Angle10 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_11
        /// </summary>
        public float A2_Distance11
        {
            get { return this._a2_Distance11; }
            set
            {
                this._a2_Distance11 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_11
        /// </summary>
        public float A2_Angle11
        {
            get { return this._a2_Angle11; }
            set
            {
                this._a2_Angle11 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_12
        /// </summary>
        public float A2_Distance12
        {
            get { return this._a2_Distance12; }
            set
            {
                this._a2_Distance12 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_12
        /// </summary>
        public float A2_Angle12
        {
            get { return this._a2_Angle12; }
            set
            {
                this._a2_Angle12 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_13
        /// </summary>
        public float A2_Distance13
        {
            get { return this._a2_Distance13; }
            set
            {
                this._a2_Distance13 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_13
        /// </summary>
        public float A2_Angle13
        {
            get { return this._a2_Angle13; }
            set
            {
                this._a2_Angle13 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_14
        /// </summary>
        public float A2_Distance14
        {
            get { return this._a2_Distance14; }
            set
            {
                this._a2_Distance14 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_14
        /// </summary>
        public float A2_Angle14
        {
            get { return this._a2_Angle14; }
            set
            {
                this._a2_Angle14 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_15
        /// </summary>
        public float A2_Distance15
        {
            get { return this._a2_Distance15; }
            set
            {
                this._a2_Distance15 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_15
        /// </summary>
        public float A2_Angle15
        {
            get { return this._a2_Angle15; }
            set
            {
                this._a2_Angle15 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_16
        /// </summary>
        public float A2_Distance16
        {
            get { return this._a2_Distance16; }
            set
            {
                this._a2_Distance16 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_16
        /// </summary>
        public float A2_Angle16
        {
            get { return this._a2_Angle16; }
            set
            {
                this._a2_Angle16 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_17
        /// </summary>
        public float A2_Distance17
        {
            get { return this._a2_Distance17; }
            set
            {
                this._a2_Distance17 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_17
        /// </summary>
        public float A2_Angle17
        {
            get { return this._a2_Angle17; }
            set
            {
                this._a2_Angle17 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_18
        /// </summary>
        public float A2_Distance18
        {
            get { return this._a2_Distance18; }
            set
            {
                this._a2_Distance18 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_18
        /// </summary>
        public float A2_Angle18
        {
            get { return this._a2_Angle18; }
            set
            {
                this._a2_Angle18 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_19
        /// </summary>
        public float A2_Distance19
        {
            get { return this._a2_Distance19; }
            set
            {
                this._a2_Distance19 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_19
        /// </summary>
        public float A2_Angle19
        {
            get { return this._a2_Angle19; }
            set
            {
                this._a2_Angle19 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_20
        /// </summary>
        public float A2_Distance20
        {
            get { return this._a2_Distance20; }
            set
            {
                this._a2_Distance20 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_20
        /// </summary>
        public float A2_Angle20
        {
            get { return this._a2_Angle20; }
            set
            {
                this._a2_Angle20 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_21
        /// </summary>
        public float A2_Distance21
        {
            get { return this._a2_Distance21; }
            set
            {
                this._a2_Distance21 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_21
        /// </summary>
        public float A2_Angle21
        {
            get { return this._a2_Angle21; }
            set
            {
                this._a2_Angle21 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_22
        /// </summary>
        public float A2_Distance22
        {
            get { return this._a2_Distance22; }
            set
            {
                this._a2_Distance22 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_22
        /// </summary>
        public float A2_Angle22
        {
            get { return this._a2_Angle22; }
            set
            {
                this._a2_Angle22 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_23
        /// </summary>
        public float A2_Distance23
        {
            get { return this._a2_Distance23; }
            set
            {
                this._a2_Distance23 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_23
        /// </summary>
        public float A2_Angle23
        {
            get { return this._a2_Angle23; }
            set
            {
                this._a2_Angle23 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_24
        /// </summary>
        public float A2_Distance24
        {
            get { return this._a2_Distance24; }
            set
            {
                this._a2_Distance24 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_24
        /// </summary>
        public float A2_Angle24
        {
            get { return this._a2_Angle24; }
            set
            {
                this._a2_Angle24 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_25
        /// </summary>
        public float A2_Distance25
        {
            get { return this._a2_Distance25; }
            set
            {
                this._a2_Distance25 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_25
        /// </summary>
        public float A2_Angle25
        {
            get { return this._a2_Angle25; }
            set
            {
                this._a2_Angle25 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_26
        /// </summary>
        public float A2_Distance26
        {
            get { return this._a2_Distance26; }
            set
            {
                this._a2_Distance26 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_26
        /// </summary>
        public float A2_Angle26
        {
            get { return this._a2_Angle26; }
            set
            {
                this._a2_Angle26 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_27
        /// </summary>
        public float A2_Distance27
        {
            get { return this._a2_Distance27; }
            set
            {
                this._a2_Distance27 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_27
        /// </summary>
        public float A2_Angle27
        {
            get { return this._a2_Angle27; }
            set
            {
                this._a2_Angle27 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_28
        /// </summary>
        public float A2_Distance28
        {
            get { return this._a2_Distance28; }
            set
            {
                this._a2_Distance28 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_28
        /// </summary>
        public float A2_Angle28
        {
            get { return this._a2_Angle28; }
            set
            {
                this._a2_Angle28 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_29
        /// </summary>
        public float A2_Distance29
        {
            get { return this._a2_Distance29; }
            set
            {
                this._a2_Distance29 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_29
        /// </summary>
        public float A2_Angle29
        {
            get { return this._a2_Angle29; }
            set
            {
                this._a2_Angle29 = value;
            }
        }

        /// <summary>
        /// B角阴阳极错位距离_30
        /// </summary>
        public float A2_Distance30
        {
            get { return this._a2_Distance30; }
            set
            {
                this._a2_Distance30 = value;
            }
        }

        /// <summary>
        /// B角阳极轮廓与阴极的角度_30
        /// </summary>
        public float A2_Angle30
        {
            get { return this._a2_Angle30; }
            set
            {
                this._a2_Angle30 = value;
            }
        }

        /// <summary>
        /// C角拍照结果
        /// </summary>
        public int A3_PhotographResult
        {
            get { return this._a3_PhotographResult; }
            set
            {
                this._a3_PhotographResult = value;
            }
        }

        /// <summary>
        /// C角原始图Path
        /// </summary>
        public string A3_OriginalImagePath
        {
            get { return this._a3_OriginalImagePath; }
            set
            {
                this._a3_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// C角结果图Path
        /// </summary>
        public string A3_ResultImagePath
        {
            get { return this._a3_ResultImagePath; }
            set
            {
                this._a3_ResultImagePath = value;
            }
        }

        /// <summary>
        /// C角最小
        /// </summary>
        public float A3_Min
        {
            get { return this._a3_Min; }
            set
            {
                this._a3_Min = value;
            }
        }

        /// <summary>
        /// C角最大
        /// </summary>
        public float A3_Max
        {
            get { return this._a3_Max; }
            set
            {
                this._a3_Max = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_1
        /// </summary>
        public float A3_Distance1
        {
            get { return this._a3_Distance1; }
            set
            {
                this._a3_Distance1 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_1
        /// </summary>
        public float A3_Angle1
        {
            get { return this._a3_Angle1; }
            set
            {
                this._a3_Angle1 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_2
        /// </summary>
        public float A3_Distance2
        {
            get { return this._a3_Distance2; }
            set
            {
                this._a3_Distance2 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_2
        /// </summary>
        public float A3_Angle2
        {
            get { return this._a3_Angle2; }
            set
            {
                this._a3_Angle2 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_3
        /// </summary>
        public float A3_Distance3
        {
            get { return this._a3_Distance3; }
            set
            {
                this._a3_Distance3 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_3
        /// </summary>
        public float A3_Angle3
        {
            get { return this._a3_Angle3; }
            set
            {
                this._a3_Angle3 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_4
        /// </summary>
        public float A3_Distance4
        {
            get { return this._a3_Distance4; }
            set
            {
                this._a3_Distance4 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_4
        /// </summary>
        public float A3_Angle4
        {
            get { return this._a3_Angle4; }
            set
            {
                this._a3_Angle4 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_5
        /// </summary>
        public float A3_Distance5
        {
            get { return this._a3_Distance5; }
            set
            {
                this._a3_Distance5 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_5
        /// </summary>
        public float A3_Angle5
        {
            get { return this._a3_Angle5; }
            set
            {
                this._a3_Angle5 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_6
        /// </summary>
        public float A3_Distance6
        {
            get { return this._a3_Distance6; }
            set
            {
                this._a3_Distance6 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_6
        /// </summary>
        public float A3_Angle6
        {
            get { return this._a3_Angle6; }
            set
            {
                this._a3_Angle6 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_7
        /// </summary>
        public float A3_Distance7
        {
            get { return this._a3_Distance7; }
            set
            {
                this._a3_Distance7 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_7
        /// </summary>
        public float A3_Angle7
        {
            get { return this._a3_Angle7; }
            set
            {
                this._a3_Angle7 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_8
        /// </summary>
        public float A3_Distance8
        {
            get { return this._a3_Distance8; }
            set
            {
                this._a3_Distance8 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_8
        /// </summary>
        public float A3_Angle8
        {
            get { return this._a3_Angle8; }
            set
            {
                this._a3_Angle8 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_9
        /// </summary>
        public float A3_Distance9
        {
            get { return this._a3_Distance9; }
            set
            {
                this._a3_Distance9 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_9
        /// </summary>
        public float A3_Angle9
        {
            get { return this._a3_Angle9; }
            set
            {
                this._a3_Angle9 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_10
        /// </summary>
        public float A3_Distance10
        {
            get { return this._a3_Distance10; }
            set
            {
                this._a3_Distance10 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_10
        /// </summary>
        public float A3_Angle10
        {
            get { return this._a3_Angle10; }
            set
            {
                this._a3_Angle10 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_11
        /// </summary>
        public float A3_Distance11
        {
            get { return this._a3_Distance11; }
            set
            {
                this._a3_Distance11 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_11
        /// </summary>
        public float A3_Angle11
        {
            get { return this._a3_Angle11; }
            set
            {
                this._a3_Angle11 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_12
        /// </summary>
        public float A3_Distance12
        {
            get { return this._a3_Distance12; }
            set
            {
                this._a3_Distance12 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_12
        /// </summary>
        public float A3_Angle12
        {
            get { return this._a3_Angle12; }
            set
            {
                this._a3_Angle12 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_13
        /// </summary>
        public float A3_Distance13
        {
            get { return this._a3_Distance13; }
            set
            {
                this._a3_Distance13 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_13
        /// </summary>
        public float A3_Angle13
        {
            get { return this._a3_Angle13; }
            set
            {
                this._a3_Angle13 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_14
        /// </summary>
        public float A3_Distance14
        {
            get { return this._a3_Distance14; }
            set
            {
                this._a3_Distance14 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_14
        /// </summary>
        public float A3_Angle14
        {
            get { return this._a3_Angle14; }
            set
            {
                this._a3_Angle14 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_15
        /// </summary>
        public float A3_Distance15
        {
            get { return this._a3_Distance15; }
            set
            {
                this._a3_Distance15 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_15
        /// </summary>
        public float A3_Angle15
        {
            get { return this._a3_Angle15; }
            set
            {
                this._a3_Angle15 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_16
        /// </summary>
        public float A3_Distance16
        {
            get { return this._a3_Distance16; }
            set
            {
                this._a3_Distance16 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_16
        /// </summary>
        public float A3_Angle16
        {
            get { return this._a3_Angle16; }
            set
            {
                this._a3_Angle16 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_17
        /// </summary>
        public float A3_Distance17
        {
            get { return this._a3_Distance17; }
            set
            {
                this._a3_Distance17 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_17
        /// </summary>
        public float A3_Angle17
        {
            get { return this._a3_Angle17; }
            set
            {
                this._a3_Angle17 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_18
        /// </summary>
        public float A3_Distance18
        {
            get { return this._a3_Distance18; }
            set
            {
                this._a3_Distance18 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_18
        /// </summary>
        public float A3_Angle18
        {
            get { return this._a3_Angle18; }
            set
            {
                this._a3_Angle18 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_19
        /// </summary>
        public float A3_Distance19
        {
            get { return this._a3_Distance19; }
            set
            {
                this._a3_Distance19 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_19
        /// </summary>
        public float A3_Angle19
        {
            get { return this._a3_Angle19; }
            set
            {
                this._a3_Angle19 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_20
        /// </summary>
        public float A3_Distance20
        {
            get { return this._a3_Distance20; }
            set
            {
                this._a3_Distance20 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_20
        /// </summary>
        public float A3_Angle20
        {
            get { return this._a3_Angle20; }
            set
            {
                this._a3_Angle20 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_21
        /// </summary>
        public float A3_Distance21
        {
            get { return this._a3_Distance21; }
            set
            {
                this._a3_Distance21 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_21
        /// </summary>
        public float A3_Angle21
        {
            get { return this._a3_Angle21; }
            set
            {
                this._a3_Angle21 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_22
        /// </summary>
        public float A3_Distance22
        {
            get { return this._a3_Distance22; }
            set
            {
                this._a3_Distance22 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_22
        /// </summary>
        public float A3_Angle22
        {
            get { return this._a3_Angle22; }
            set
            {
                this._a3_Angle22 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_23
        /// </summary>
        public float A3_Distance23
        {
            get { return this._a3_Distance23; }
            set
            {
                this._a3_Distance23 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_23
        /// </summary>
        public float A3_Angle23
        {
            get { return this._a3_Angle23; }
            set
            {
                this._a3_Angle23 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_24
        /// </summary>
        public float A3_Distance24
        {
            get { return this._a3_Distance24; }
            set
            {
                this._a3_Distance24 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_24
        /// </summary>
        public float A3_Angle24
        {
            get { return this._a3_Angle24; }
            set
            {
                this._a3_Angle24 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_25
        /// </summary>
        public float A3_Distance25
        {
            get { return this._a3_Distance25; }
            set
            {
                this._a3_Distance25 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_25
        /// </summary>
        public float A3_Angle25
        {
            get { return this._a3_Angle25; }
            set
            {
                this._a3_Angle25 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_26
        /// </summary>
        public float A3_Distance26
        {
            get { return this._a3_Distance26; }
            set
            {
                this._a3_Distance26 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_26
        /// </summary>
        public float A3_Angle26
        {
            get { return this._a3_Angle26; }
            set
            {
                this._a3_Angle26 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_27
        /// </summary>
        public float A3_Distance27
        {
            get { return this._a3_Distance27; }
            set
            {
                this._a3_Distance27 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_27
        /// </summary>
        public float A3_Angle27
        {
            get { return this._a3_Angle27; }
            set
            {
                this._a3_Angle27 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_28
        /// </summary>
        public float A3_Distance28
        {
            get { return this._a3_Distance28; }
            set
            {
                this._a3_Distance28 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_28
        /// </summary>
        public float A3_Angle28
        {
            get { return this._a3_Angle28; }
            set
            {
                this._a3_Angle28 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_29
        /// </summary>
        public float A3_Distance29
        {
            get { return this._a3_Distance29; }
            set
            {
                this._a3_Distance29 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_29
        /// </summary>
        public float A3_Angle29
        {
            get { return this._a3_Angle29; }
            set
            {
                this._a3_Angle29 = value;
            }
        }

        /// <summary>
        /// C角阴阳极错位距离_30
        /// </summary>
        public float A3_Distance30
        {
            get { return this._a3_Distance30; }
            set
            {
                this._a3_Distance30 = value;
            }
        }

        /// <summary>
        /// C角阳极轮廓与阴极的角度_30
        /// </summary>
        public float A3_Angle30
        {
            get { return this._a3_Angle30; }
            set
            {
                this._a3_Angle30 = value;
            }
        }

        /// <summary>
        /// D角拍照结果
        /// </summary>
        public int A4_PhotographResult
        {
            get { return this._a4_PhotographResult; }
            set
            {
                this._a4_PhotographResult = value;
            }
        }

        /// <summary>
        /// D角原始图Path
        /// </summary>
        public string A4_OriginalImagePath
        {
            get { return this._a4_OriginalImagePath; }
            set
            {
                this._a4_OriginalImagePath = value;
            }
        }

        /// <summary>
        /// D角结果图Path
        /// </summary>
        public string A4_ResultImagePath
        {
            get { return this._a4_ResultImagePath; }
            set
            {
                this._a4_ResultImagePath = value;
            }
        }

        /// <summary>
        /// D角最小
        /// </summary>
        public float A4_Min
        {
            get { return this._a4_Min; }
            set
            {
                this._a4_Min = value;
            }
        }

        /// <summary>
        /// D角最大
        /// </summary>
        public float A4_Max
        {
            get { return this._a4_Max; }
            set
            {
                this._a4_Max = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_1
        /// </summary>
        public float A4_Distance1
        {
            get { return this._a4_Distance1; }
            set
            {
                this._a4_Distance1 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_1
        /// </summary>
        public float A4_Angle1
        {
            get { return this._a4_Angle1; }
            set
            {
                this._a4_Angle1 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_2
        /// </summary>
        public float A4_Distance2
        {
            get { return this._a4_Distance2; }
            set
            {
                this._a4_Distance2 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_2
        /// </summary>
        public float A4_Angle2
        {
            get { return this._a4_Angle2; }
            set
            {
                this._a4_Angle2 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_3
        /// </summary>
        public float A4_Distance3
        {
            get { return this._a4_Distance3; }
            set
            {
                this._a4_Distance3 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_3
        /// </summary>
        public float A4_Angle3
        {
            get { return this._a4_Angle3; }
            set
            {
                this._a4_Angle3 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_4
        /// </summary>
        public float A4_Distance4
        {
            get { return this._a4_Distance4; }
            set
            {
                this._a4_Distance4 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_4
        /// </summary>
        public float A4_Angle4
        {
            get { return this._a4_Angle4; }
            set
            {
                this._a4_Angle4 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_5
        /// </summary>
        public float A4_Distance5
        {
            get { return this._a4_Distance5; }
            set
            {
                this._a4_Distance5 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_5
        /// </summary>
        public float A4_Angle5
        {
            get { return this._a4_Angle5; }
            set
            {
                this._a4_Angle5 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_6
        /// </summary>
        public float A4_Distance6
        {
            get { return this._a4_Distance6; }
            set
            {
                this._a4_Distance6 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_6
        /// </summary>
        public float A4_Angle6
        {
            get { return this._a4_Angle6; }
            set
            {
                this._a4_Angle6 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_7
        /// </summary>
        public float A4_Distance7
        {
            get { return this._a4_Distance7; }
            set
            {
                this._a4_Distance7 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_7
        /// </summary>
        public float A4_Angle7
        {
            get { return this._a4_Angle7; }
            set
            {
                this._a4_Angle7 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_8
        /// </summary>
        public float A4_Distance8
        {
            get { return this._a4_Distance8; }
            set
            {
                this._a4_Distance8 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_8
        /// </summary>
        public float A4_Angle8
        {
            get { return this._a4_Angle8; }
            set
            {
                this._a4_Angle8 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_9
        /// </summary>
        public float A4_Distance9
        {
            get { return this._a4_Distance9; }
            set
            {
                this._a4_Distance9 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_9
        /// </summary>
        public float A4_Angle9
        {
            get { return this._a4_Angle9; }
            set
            {
                this._a4_Angle9 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_10
        /// </summary>
        public float A4_Distance10
        {
            get { return this._a4_Distance10; }
            set
            {
                this._a4_Distance10 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_10
        /// </summary>
        public float A4_Angle10
        {
            get { return this._a4_Angle10; }
            set
            {
                this._a4_Angle10 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_11
        /// </summary>
        public float A4_Distance11
        {
            get { return this._a4_Distance11; }
            set
            {
                this._a4_Distance11 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_11
        /// </summary>
        public float A4_Angle11
        {
            get { return this._a4_Angle11; }
            set
            {
                this._a4_Angle11 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_12
        /// </summary>
        public float A4_Distance12
        {
            get { return this._a4_Distance12; }
            set
            {
                this._a4_Distance12 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_12
        /// </summary>
        public float A4_Angle12
        {
            get { return this._a4_Angle12; }
            set
            {
                this._a4_Angle12 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_13
        /// </summary>
        public float A4_Distance13
        {
            get { return this._a4_Distance13; }
            set
            {
                this._a4_Distance13 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_13
        /// </summary>
        public float A4_Angle13
        {
            get { return this._a4_Angle13; }
            set
            {
                this._a4_Angle13 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_14
        /// </summary>
        public float A4_Distance14
        {
            get { return this._a4_Distance14; }
            set
            {
                this._a4_Distance14 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_14
        /// </summary>
        public float A4_Angle14
        {
            get { return this._a4_Angle14; }
            set
            {
                this._a4_Angle14 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_15
        /// </summary>
        public float A4_Distance15
        {
            get { return this._a4_Distance15; }
            set
            {
                this._a4_Distance15 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_15
        /// </summary>
        public float A4_Angle15
        {
            get { return this._a4_Angle15; }
            set
            {
                this._a4_Angle15 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_16
        /// </summary>
        public float A4_Distance16
        {
            get { return this._a4_Distance16; }
            set
            {
                this._a4_Distance16 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_16
        /// </summary>
        public float A4_Angle16
        {
            get { return this._a4_Angle16; }
            set
            {
                this._a4_Angle16 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_17
        /// </summary>
        public float A4_Distance17
        {
            get { return this._a4_Distance17; }
            set
            {
                this._a4_Distance17 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_17
        /// </summary>
        public float A4_Angle17
        {
            get { return this._a4_Angle17; }
            set
            {
                this._a4_Angle17 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_18
        /// </summary>
        public float A4_Distance18
        {
            get { return this._a4_Distance18; }
            set
            {
                this._a4_Distance18 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_18
        /// </summary>
        public float A4_Angle18
        {
            get { return this._a4_Angle18; }
            set
            {
                this._a4_Angle18 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_19
        /// </summary>
        public float A4_Distance19
        {
            get { return this._a4_Distance19; }
            set
            {
                this._a4_Distance19 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_19
        /// </summary>
        public float A4_Angle19
        {
            get { return this._a4_Angle19; }
            set
            {
                this._a4_Angle19 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_20
        /// </summary>
        public float A4_Distance20
        {
            get { return this._a4_Distance20; }
            set
            {
                this._a4_Distance20 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_20
        /// </summary>
        public float A4_Angle20
        {
            get { return this._a4_Angle20; }
            set
            {
                this._a4_Angle20 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_21
        /// </summary>
        public float A4_Distance21
        {
            get { return this._a4_Distance21; }
            set
            {
                this._a4_Distance21 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_21
        /// </summary>
        public float A4_Angle21
        {
            get { return this._a4_Angle21; }
            set
            {
                this._a4_Angle21 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_22
        /// </summary>
        public float A4_Distance22
        {
            get { return this._a4_Distance22; }
            set
            {
                this._a4_Distance22 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_22
        /// </summary>
        public float A4_Angle22
        {
            get { return this._a4_Angle22; }
            set
            {
                this._a4_Angle22 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_23
        /// </summary>
        public float A4_Distance23
        {
            get { return this._a4_Distance23; }
            set
            {
                this._a4_Distance23 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_23
        /// </summary>
        public float A4_Angle23
        {
            get { return this._a4_Angle23; }
            set
            {
                this._a4_Angle23 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_24
        /// </summary>
        public float A4_Distance24
        {
            get { return this._a4_Distance24; }
            set
            {
                this._a4_Distance24 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_24
        /// </summary>
        public float A4_Angle24
        {
            get { return this._a4_Angle24; }
            set
            {
                this._a4_Angle24 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_25
        /// </summary>
        public float A4_Distance25
        {
            get { return this._a4_Distance25; }
            set
            {
                this._a4_Distance25 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_25
        /// </summary>
        public float A4_Angle25
        {
            get { return this._a4_Angle25; }
            set
            {
                this._a4_Angle25 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_26
        /// </summary>
        public float A4_Distance26
        {
            get { return this._a4_Distance26; }
            set
            {
                this._a4_Distance26 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_26
        /// </summary>
        public float A4_Angle26
        {
            get { return this._a4_Angle26; }
            set
            {
                this._a4_Angle26 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_27
        /// </summary>
        public float A4_Distance27
        {
            get { return this._a4_Distance27; }
            set
            {
                this._a4_Distance27 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_27
        /// </summary>
        public float A4_Angle27
        {
            get { return this._a4_Angle27; }
            set
            {
                this._a4_Angle27 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_28
        /// </summary>
        public float A4_Distance28
        {
            get { return this._a4_Distance28; }
            set
            {
                this._a4_Distance28 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_28
        /// </summary>
        public float A4_Angle28
        {
            get { return this._a4_Angle28; }
            set
            {
                this._a4_Angle28 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_29
        /// </summary>
        public float A4_Distance29
        {
            get { return this._a4_Distance29; }
            set
            {
                this._a4_Distance29 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_29
        /// </summary>
        public float A4_Angle29
        {
            get { return this._a4_Angle29; }
            set
            {
                this._a4_Angle29 = value;
            }
        }

        /// <summary>
        /// D角阴阳极错位距离_30
        /// </summary>
        public float A4_Distance30
        {
            get { return this._a4_Distance30; }
            set
            {
                this._a4_Distance30 = value;
            }
        }

        /// <summary>
        /// D角阳极轮廓与阴极的角度_30
        /// </summary>
        public float A4_Angle30
        {
            get { return this._a4_Angle30; }
            set
            {
                this._a4_Angle30 = value;
            }
        }

        public int RecheckState
        {
            get
            {
                return _recheckState;
            }

            set
            {
                _recheckState = value;
            }
        }

        public DateTime RecheckTime
        {
            get
            {
                return _recheckTime;
            }

            set
            {
                _recheckTime = value;
            }
        }

        public string RecheckUserID
        {
            get
            {
                return _recheckUserID;
            }

            set
            {
                _recheckUserID = value;
            }
        }

        public DateTime FQATime
        {
            get
            {
                return _FQATime;
            }

            set
            {
                _FQATime = value;
            }
        }

        public string FQAUser
        {
            get
            {
                return _FQAUser;
            }

            set
            {
                _FQAUser = value;
            }
        }

        public BatteryCheck DeepCopy()
        {
            BatteryCheck batteryCheck = new BatteryCheck();
            batteryCheck.Iden = _iden;
            batteryCheck.ProductSN= _productSN;
            batteryCheck.Model = _model;            
            batteryCheck.Quality=_quality;
            batteryCheck.NGreason=_ngReason;
            batteryCheck.ProductDate = _productDate;
            batteryCheck.OutTime = _outTime;
            batteryCheck.Operator = _operator;

            batteryCheck.ResultPath=_resultPath;
            batteryCheck.MesImagePath = _mesImagePath;
            batteryCheck.CheckMode = _checkMode;
            batteryCheck.Thickness = _thickness;

            batteryCheck.A1_PhotographResult=_a1_PhotographResult;
            batteryCheck.A1_OriginalImagePath=_a1_OriginalImagePath;
            batteryCheck.A1_ResultImagePath=_a1_ResultImagePath;
            batteryCheck.A1_Min=_a1_Min;
            batteryCheck.A1_Max=_a1_Max;
            batteryCheck.A1_Distance1=_a1_Distance1;
            batteryCheck.A1_Angle1=_a1_Angle1;
            batteryCheck.A1_Distance2 = _a1_Distance2;
            batteryCheck.A1_Angle2 = _a1_Angle2;
            batteryCheck.A1_Distance3 = _a1_Distance3;
            batteryCheck.A1_Angle3 = _a1_Angle3;
            batteryCheck.A1_Distance4 = _a1_Distance4;
            batteryCheck.A1_Angle4 = _a1_Angle4;
            batteryCheck.A1_Distance5 = _a1_Distance5;
            batteryCheck.A1_Angle5 = _a1_Angle5;
            batteryCheck.A1_Distance6 = _a1_Distance6;
            batteryCheck.A1_Angle6 = _a1_Angle6;
            batteryCheck.A1_Distance7 = _a1_Distance7;
            batteryCheck.A1_Angle7 = _a1_Angle7;
            batteryCheck.A1_Distance8 = _a1_Distance8;
            batteryCheck.A1_Angle8 = _a1_Angle8;
            batteryCheck.A1_Distance9 = _a1_Distance9;
            batteryCheck.A1_Angle9 = _a1_Angle9;
            batteryCheck.A1_Distance10 = _a1_Distance10;
            batteryCheck.A1_Angle10 = _a1_Angle10;
            batteryCheck.A1_Distance11 = _a1_Distance11;
            batteryCheck.A1_Angle11 = _a1_Angle11;
            batteryCheck.A1_Distance12 = _a1_Distance12;
            batteryCheck.A1_Angle12 = _a1_Angle12;
            batteryCheck.A1_Distance13 = _a1_Distance13;
            batteryCheck.A1_Angle13 = _a1_Angle13;
            batteryCheck.A1_Distance14 = _a1_Distance14;
            batteryCheck.A1_Angle14 = _a1_Angle14;
            batteryCheck.A1_Distance15 = _a1_Distance15;
            batteryCheck.A1_Angle15 = _a1_Angle15;
            batteryCheck.A1_Distance16 = _a1_Distance16;
            batteryCheck.A1_Angle16 = _a1_Angle16;
            batteryCheck.A1_Distance17 = _a1_Distance17;
            batteryCheck.A1_Angle17 = _a1_Angle17;
            batteryCheck.A1_Distance18 = _a1_Distance18;
            batteryCheck.A1_Angle18 = _a1_Angle18;
            batteryCheck.A1_Distance19 = _a1_Distance19;
            batteryCheck.A1_Angle19 = _a1_Angle19;
            batteryCheck.A1_Distance20 = _a1_Distance20;
            batteryCheck.A1_Angle20 = _a1_Angle20;
            batteryCheck.A1_Distance21 = _a1_Distance21;
            batteryCheck.A1_Angle21 = _a1_Angle21;
            batteryCheck.A1_Distance22 = _a1_Distance22;
            batteryCheck.A1_Angle22 = _a1_Angle22;
            batteryCheck.A1_Distance23 = _a1_Distance23;
            batteryCheck.A1_Angle23 = _a1_Angle23;
            batteryCheck.A1_Distance24 = _a1_Distance24;
            batteryCheck.A1_Angle24 = _a1_Angle24;
            batteryCheck.A1_Distance25 = _a1_Distance25;
            batteryCheck.A1_Angle25 = _a1_Angle25;
            batteryCheck.A1_Distance26 = _a1_Distance26;
            batteryCheck.A1_Angle26 = _a1_Angle26;
            batteryCheck.A1_Distance27 = _a1_Distance27;
            batteryCheck.A1_Angle27 = _a1_Angle27;
            batteryCheck.A1_Distance28 = _a1_Distance28;
            batteryCheck.A1_Angle28 = _a1_Angle28;
            batteryCheck.A1_Distance29 = _a1_Distance29;
            batteryCheck.A1_Angle29 = _a1_Angle29;
            batteryCheck.A1_Distance30 = _a1_Distance30;
            batteryCheck.A1_Angle30 = _a1_Angle30;


            batteryCheck.A2_PhotographResult = _a2_PhotographResult;
            batteryCheck.A2_OriginalImagePath = _a2_OriginalImagePath;
            batteryCheck.A2_ResultImagePath = _a2_ResultImagePath;
            batteryCheck.A2_Min = _a2_Min;
            batteryCheck.A2_Max = _a2_Max;
            batteryCheck.A2_Distance1 = _a2_Distance1;
            batteryCheck.A2_Angle1 = _a2_Angle1;
            batteryCheck.A2_Distance2 = _a2_Distance2;
            batteryCheck.A2_Angle2 = _a2_Angle2;
            batteryCheck.A2_Distance3 = _a2_Distance3;
            batteryCheck.A2_Angle3 = _a2_Angle3;
            batteryCheck.A2_Distance4 = _a2_Distance4;
            batteryCheck.A2_Angle4 = _a2_Angle4;
            batteryCheck.A2_Distance5 = _a2_Distance5;
            batteryCheck.A2_Angle5 = _a2_Angle5;
            batteryCheck.A2_Distance6 = _a2_Distance6;
            batteryCheck.A2_Angle6 = _a2_Angle6;
            batteryCheck.A2_Distance7 = _a2_Distance7;
            batteryCheck.A2_Angle7 = _a2_Angle7;
            batteryCheck.A2_Distance8 = _a2_Distance8;
            batteryCheck.A2_Angle8 = _a2_Angle8;
            batteryCheck.A2_Distance9 = _a2_Distance9;
            batteryCheck.A2_Angle9 = _a2_Angle9;
            batteryCheck.A2_Distance10 = _a2_Distance10;
            batteryCheck.A2_Angle10 = _a2_Angle10;
            batteryCheck.A2_Distance11 = _a2_Distance11;
            batteryCheck.A2_Angle11 = _a2_Angle11;
            batteryCheck.A2_Distance12 = _a2_Distance12;
            batteryCheck.A2_Angle12 = _a2_Angle12;
            batteryCheck.A2_Distance13 = _a2_Distance13;
            batteryCheck.A2_Angle13 = _a2_Angle13;
            batteryCheck.A2_Distance14 = _a2_Distance14;
            batteryCheck.A2_Angle14 = _a2_Angle14;
            batteryCheck.A2_Distance15 = _a2_Distance15;
            batteryCheck.A2_Angle15 = _a2_Angle15;
            batteryCheck.A2_Distance16 = _a2_Distance16;
            batteryCheck.A2_Angle16 = _a2_Angle16;
            batteryCheck.A2_Distance17 = _a2_Distance17;
            batteryCheck.A2_Angle17 = _a2_Angle17;
            batteryCheck.A2_Distance18 = _a2_Distance18;
            batteryCheck.A2_Angle18 = _a2_Angle18;
            batteryCheck.A2_Distance19 = _a2_Distance19;
            batteryCheck.A2_Angle19 = _a2_Angle19;
            batteryCheck.A2_Distance20 = _a2_Distance20;
            batteryCheck.A2_Angle20 = _a2_Angle20;
            batteryCheck.A2_Distance21 = _a2_Distance21;
            batteryCheck.A2_Angle21 = _a2_Angle21;
            batteryCheck.A2_Distance22 = _a2_Distance22;
            batteryCheck.A2_Angle22 = _a2_Angle22;
            batteryCheck.A2_Distance23 = _a2_Distance23;
            batteryCheck.A2_Angle23 = _a2_Angle23;
            batteryCheck.A2_Distance24 = _a2_Distance24;
            batteryCheck.A2_Angle24 = _a2_Angle24;
            batteryCheck.A2_Distance25 = _a2_Distance25;
            batteryCheck.A2_Angle25 = _a2_Angle25;
            batteryCheck.A2_Distance26 = _a2_Distance26;
            batteryCheck.A2_Angle26 = _a2_Angle26;
            batteryCheck.A2_Distance27 = _a2_Distance27;
            batteryCheck.A2_Angle27 = _a2_Angle27;
            batteryCheck.A2_Distance28 = _a2_Distance28;
            batteryCheck.A2_Angle28 = _a2_Angle28;
            batteryCheck.A2_Distance29 = _a2_Distance29;
            batteryCheck.A2_Angle29 = _a2_Angle29;
            batteryCheck.A2_Distance30 = _a2_Distance30;
            batteryCheck.A2_Angle30 = _a2_Angle30;


            batteryCheck.A3_PhotographResult = _a3_PhotographResult;
            batteryCheck.A3_OriginalImagePath = _a3_OriginalImagePath;
            batteryCheck.A3_ResultImagePath = _a3_ResultImagePath;
            batteryCheck.A3_Min = _a3_Min;
            batteryCheck.A3_Max = _a3_Max;
            batteryCheck.A3_Distance1 = _a3_Distance1;
            batteryCheck.A3_Angle1 = _a3_Angle1;
            batteryCheck.A3_Distance2 = _a3_Distance2;
            batteryCheck.A3_Angle2 = _a3_Angle2;
            batteryCheck.A3_Distance3 = _a3_Distance3;
            batteryCheck.A3_Angle3 = _a3_Angle3;
            batteryCheck.A3_Distance4 = _a3_Distance4;
            batteryCheck.A3_Angle4 = _a3_Angle4;
            batteryCheck.A3_Distance5 = _a3_Distance5;
            batteryCheck.A3_Angle5 = _a3_Angle5;
            batteryCheck.A3_Distance6 = _a3_Distance6;
            batteryCheck.A3_Angle6 = _a3_Angle6;
            batteryCheck.A3_Distance7 = _a3_Distance7;
            batteryCheck.A3_Angle7 = _a3_Angle7;
            batteryCheck.A3_Distance8 = _a3_Distance8;
            batteryCheck.A3_Angle8 = _a3_Angle8;
            batteryCheck.A3_Distance9 = _a3_Distance9;
            batteryCheck.A3_Angle9 = _a3_Angle9;
            batteryCheck.A3_Distance10 = _a3_Distance10;
            batteryCheck.A3_Angle10 = _a3_Angle10;
            batteryCheck.A3_Distance11 = _a3_Distance11;
            batteryCheck.A3_Angle11 = _a3_Angle11;
            batteryCheck.A3_Distance12 = _a3_Distance12;
            batteryCheck.A3_Angle12 = _a3_Angle12;
            batteryCheck.A3_Distance13 = _a3_Distance13;
            batteryCheck.A3_Angle13 = _a3_Angle13;
            batteryCheck.A3_Distance14 = _a3_Distance14;
            batteryCheck.A3_Angle14 = _a3_Angle14;
            batteryCheck.A3_Distance15 = _a3_Distance15;
            batteryCheck.A3_Angle15 = _a3_Angle15;
            batteryCheck.A3_Distance16 = _a3_Distance16;
            batteryCheck.A3_Angle16 = _a3_Angle16;
            batteryCheck.A3_Distance17 = _a3_Distance17;
            batteryCheck.A3_Angle17 = _a3_Angle17;
            batteryCheck.A3_Distance18 = _a3_Distance18;
            batteryCheck.A3_Angle18 = _a3_Angle18;
            batteryCheck.A3_Distance19 = _a3_Distance19;
            batteryCheck.A3_Angle19 = _a3_Angle19;
            batteryCheck.A3_Distance20 = _a3_Distance20;
            batteryCheck.A3_Angle20 = _a3_Angle20;
            batteryCheck.A3_Distance21 = _a3_Distance21;
            batteryCheck.A3_Angle21 = _a3_Angle21;
            batteryCheck.A3_Distance22 = _a3_Distance22;
            batteryCheck.A3_Angle22 = _a3_Angle22;
            batteryCheck.A3_Distance23 = _a3_Distance23;
            batteryCheck.A3_Angle23 = _a3_Angle23;
            batteryCheck.A3_Distance24 = _a3_Distance24;
            batteryCheck.A3_Angle24 = _a3_Angle24;
            batteryCheck.A3_Distance25 = _a3_Distance25;
            batteryCheck.A3_Angle25 = _a3_Angle25;
            batteryCheck.A3_Distance26 = _a3_Distance26;
            batteryCheck.A3_Angle26 = _a3_Angle26;
            batteryCheck.A3_Distance27 = _a3_Distance27;
            batteryCheck.A3_Angle27 = _a3_Angle27;
            batteryCheck.A3_Distance28 = _a3_Distance28;
            batteryCheck.A3_Angle28 = _a3_Angle28;
            batteryCheck.A3_Distance29 = _a3_Distance29;
            batteryCheck.A3_Angle29 = _a3_Angle29;
            batteryCheck.A3_Distance30 = _a3_Distance30;
            batteryCheck.A3_Angle30 = _a3_Angle30;

            batteryCheck.A4_PhotographResult = _a4_PhotographResult;
            batteryCheck.A4_OriginalImagePath = _a4_OriginalImagePath;
            batteryCheck.A4_ResultImagePath = _a4_ResultImagePath;
            batteryCheck.A4_Min = _a4_Min;
            batteryCheck.A4_Max = _a4_Max;
            batteryCheck.A4_Distance1 = _a4_Distance1;
            batteryCheck.A4_Angle1 = _a4_Angle1;
            batteryCheck.A4_Distance2 = _a4_Distance2;
            batteryCheck.A4_Angle2 = _a4_Angle2;
            batteryCheck.A4_Distance3 = _a4_Distance3;
            batteryCheck.A4_Angle3 = _a4_Angle3;
            batteryCheck.A4_Distance4 = _a4_Distance4;
            batteryCheck.A4_Angle4 = _a4_Angle4;
            batteryCheck.A4_Distance5 = _a4_Distance5;
            batteryCheck.A4_Angle5 = _a4_Angle5;
            batteryCheck.A4_Distance6 = _a4_Distance6;
            batteryCheck.A4_Angle6 = _a4_Angle6;
            batteryCheck.A4_Distance7 = _a4_Distance7;
            batteryCheck.A4_Angle7 = _a4_Angle7;
            batteryCheck.A4_Distance8 = _a4_Distance8;
            batteryCheck.A4_Angle8 = _a4_Angle8;
            batteryCheck.A4_Distance9 = _a4_Distance9;
            batteryCheck.A4_Angle9 = _a4_Angle9;
            batteryCheck.A4_Distance10 = _a4_Distance10;
            batteryCheck.A4_Angle10 = _a4_Angle10;
            batteryCheck.A4_Distance11 = _a4_Distance11;
            batteryCheck.A4_Angle11 = _a4_Angle11;
            batteryCheck.A4_Distance12 = _a4_Distance12;
            batteryCheck.A4_Angle12 = _a4_Angle12;
            batteryCheck.A4_Distance13 = _a4_Distance13;
            batteryCheck.A4_Angle13 = _a4_Angle13;
            batteryCheck.A4_Distance14 = _a4_Distance14;
            batteryCheck.A4_Angle14 = _a4_Angle14;
            batteryCheck.A4_Distance15 = _a4_Distance15;
            batteryCheck.A4_Angle15 = _a4_Angle15;
            batteryCheck.A4_Distance16 = _a4_Distance16;
            batteryCheck.A4_Angle16 = _a4_Angle16;
            batteryCheck.A4_Distance17 = _a4_Distance17;
            batteryCheck.A4_Angle17 = _a4_Angle17;
            batteryCheck.A4_Distance18 = _a4_Distance18;
            batteryCheck.A4_Angle18 = _a4_Angle18;
            batteryCheck.A4_Distance19 = _a4_Distance19;
            batteryCheck.A4_Angle19 = _a4_Angle19;
            batteryCheck.A4_Distance20 = _a4_Distance20;
            batteryCheck.A4_Angle20 = _a4_Angle20;
            batteryCheck.A4_Distance21 = _a4_Distance21;
            batteryCheck.A4_Angle21 = _a4_Angle21;
            batteryCheck.A4_Distance22 = _a4_Distance22;
            batteryCheck.A4_Angle22 = _a4_Angle22;
            batteryCheck.A4_Distance23 = _a4_Distance23;
            batteryCheck.A4_Angle23 = _a4_Angle23;
            batteryCheck.A4_Distance24 = _a4_Distance24;
            batteryCheck.A4_Angle24 = _a4_Angle24;
            batteryCheck.A4_Distance25 = _a4_Distance25;
            batteryCheck.A4_Angle25 = _a4_Angle25;
            batteryCheck.A4_Distance26 = _a4_Distance26;
            batteryCheck.A4_Angle26 = _a4_Angle26;
            batteryCheck.A4_Distance27 = _a4_Distance27;
            batteryCheck.A4_Angle27 = _a4_Angle27;
            batteryCheck.A4_Distance28 = _a4_Distance28;
            batteryCheck.A4_Angle28 = _a4_Angle28;
            batteryCheck.A4_Distance29 = _a4_Distance29;
            batteryCheck.A4_Angle29 = _a4_Angle29;
            batteryCheck.A4_Distance30 = _a4_Distance30;
            batteryCheck.A4_Angle30 = _a4_Angle30;

            batteryCheck.RecheckState = _recheckState;
            batteryCheck.RecheckTime = _recheckTime;
            batteryCheck.RecheckUserID = _recheckUserID;
            batteryCheck.FQATime = _FQATime;
            batteryCheck._FQAUser = _FQAUser;

            return batteryCheck;
        }


    }
}
