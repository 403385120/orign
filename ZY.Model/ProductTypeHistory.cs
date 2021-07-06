using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Systems;

namespace ZY.Model
{
    public partial class ProductTypeHistory : EsqDbBusinessEntity
    {
        public ProductTypeHistory()
        {
            selectTable = "ProductTypeHistory";
            saveTable = "ProductTypeHistory";
            backupTable = "bak_ProductTypeHistory";
        }
        public int? Iden { get; set; }
        /// <summary>
        /// 产品型号
        /// </summary>
        public string Product_type { get; set; }
        /// <summary>
        /// 总长度最小值
        /// </summary>
        public double? Define1 { get; set; }
        /// <summary>
        /// 总长度最大值
        /// </summary>
        public double? Define2 { get; set; }
        /// <summary>
        /// 主体长度最小值
        /// </summary>
        public double? Define3 { get; set; }
        /// <summary>
        /// 主体长度最大值
        /// </summary>
        public double? Define4 { get; set; }
        /// <summary>
        /// 主体宽度最小值
        /// </summary>
        public double? Define5 { get; set; }
        /// <summary>
        /// 主体宽度最大值
        /// </summary>
        public double? Define6 { get; set; }

        /// <summary>
        ///右极耳边距最小值         
        /// </summary>
        public double? Define7 { get; set; }
        /// <summary>
        ///右极耳边距最小值         
        /// </summary>
        public double? Define8 { get; set; }

        /// <summary>
        ///左极耳边距最小值         
        /// </summary>
        public double? Define9 { get; set; }
        /// <summary>
        ///左极耳边距最大值         
        /// </summary>
        public double? Define10 { get; set; }
        /// <summary>
        ///右1小白胶最小值         
        /// </summary>
        public double? Define11 { get; set; }

        /// <summary>
        ///右1小白胶最大值         
        /// </summary>
        public double? Define12 { get; set; }
        /// <summary>
        ///右2小白胶最小值         
        /// </summary>
        public double? Define13 { get; set; }

        /// <summary>
        ///右2小白胶最大值         
        /// </summary>
        public double? Define14 { get; set; }

        /// <summary>
        ///左1小白胶最小值         
        /// </summary>
        public double? Define15 { get; set; }
        /// <summary>
        ///左1小白胶最大值         
        /// </summary>
        public double? Define16 { get; set; }
        /// <summary>
        ///左2小白胶最小值         
        /// </summary>
        public double? Define17 { get; set; }
        /// <summary>
        ///左2小白胶最大值         
        /// </summary>
        public double? Define18 { get; set; }
        /// <summary>
        ///右极耳长度最小值         
        /// </summary>
        public double? Define19 { get; set; }
        /// <summary>
        ///右极耳长度最大值         
        /// </summary>
        public double? Define20 { get; set; }

        /// <summary>
        ///左极耳长度最小值         
        /// </summary>
        public double? Define21 { get; set; }
        /// <summary>
        ///左极耳长度最大值         
        /// </summary>
        public double? Define22 { get; set; }
        /// <summary>
        ///中间极耳边距最小值         
        /// </summary>
        public double? Define23 { get; set; }
        /// <summary>
        ///中间极耳边距最小值         
        /// </summary>
        public double? Define24 { get; set; }
        /// <summary>
        ///中间1小白胶最小值         
        /// </summary>
        public double? Define25 { get; set; }
        /// <summary>
        ///中间1小白胶最小值         
        /// </summary>
        public double? Define26 { get; set; }
        /// <summary>
        ///中间2小白胶最小值         
        /// </summary>
        public double? Define27 { get; set; }
        /// <summary>
        ///中间2小白胶最小值         
        /// </summary>
        public double? Define28 { get; set; }
        /// <summary>
        ///中间极耳长度最小值         
        /// </summary>
        public double? Define29 { get; set; }
        /// <summary>
        ///中间极耳长度最大值         
        /// </summary>
        public double? Define30 { get; set; }

        /// <summary>
        ///厚度最小值         
        /// </summary>
        public double? Define31 { get; set; }
        /// <summary>
        ///厚度最大值         
        /// </summary>
        public double? Define32 { get; set; }
        /// <summary>
        ///厚度标定值 
        /// </summary>
        public double? Define33 { get; set; }

        /// <summary>
        ///厚度K值1 
        /// </summary>
        public double? Define34 { get; set; }
        /// <summary>
        ///厚度B值1 
        /// </summary>
        public double? Define35 { get; set; }
        /// <summary>
        ///相关性K值1 
        /// </summary>
        public double? Define36 { get; set; }
        /// <summary>
        ///相关性K值2 
        /// </summary>
        public double? Define37 { get; set; }
        /// <summary>
        ///相关性K值3 
        /// </summary>
        public double? Define38 { get; set; }
        /// <summary>
        ///相关性K值4 
        /// </summary>

        public double? Define39 { get; set; }
        /// <summary>
        ///相关性B值1 
        /// </summary>

        public double? Define40 { get; set; }
        /// <summary>
        ///相关性B值2 
        /// </summary>

        public double? Define41 { get; set; }
        /// <summary>
        ///相关性B值3 
        /// </summary>

        public double? Define42 { get; set; }
        /// <summary>
        ///相关性B值4
        /// </summary>

        public double? Define43 { get; set; }
        /// <summary>
        ///各工位测厚极差
        /// </summary>

        public double? Define44 { get; set; }
        /// <summary>
        ///极差检测个数
        /// </summary>

        public int? Define45 { get; set; }
        /// <summary>
        ///工位均值报警
        /// </summary>

        public double? Define46 { get; set; }

        /// <summary>
        ///工位均值报警公差
        /// </summary>
        public double? Define47 { get; set; }
        /// <summary>
        ///电压最小值
        /// </summary>
        public double? Define48 { get; set; }
        /// <summary>
        ///电压最大值
        /// </summary>
        public double? Define49 { get; set; }

        /// <summary>
        ///内阻最小值
        /// </summary>
        public double? Define50 { get; set; }

        /// <summary>
        ///内阻最大值
        /// </summary>
        public double? Define51 { get; set; }

        /// <summary>
        ///温度最小值
        /// </summary>
        public double? Define52 { get; set; }

        /// <summary>
        ///温度最大值
        /// </summary>
        public double? Define53 { get; set; }

        /// <summary>
        ///电压补偿
        /// </summary>
        public double? Define54 { get; set; }

        /// <summary>
        ///内阻补偿
        /// </summary>
        public double? Define55 { get; set; }

        /// <summary>
        ///工位1温度补偿
        /// </summary>
        public double? Define56 { get; set; }

        /// <summary>
        ///工位2温度补偿
        /// </summary>
        public double? Define57 { get; set; }

        /// <summary>
        ///电池温度和环境温度差
        /// </summary>
        public double? Define58 { get; set; }

        /// <summary>
        ///不测内阻时内阻设定值
        /// </summary>
        public double? Define59 { get; set; }

        /// <summary>
        ///不测温度时温度设定值
        /// </summary>
        public double? Define60 { get; set; }
        /// <summary>
        ///IV初始值(判断依据4)
        /// </summary>
        public double? Define61 { get; set; }

        /// <summary>
        ///IV跳变值(判断一句5)
        /// </summary>
        public double? Define62 { get; set; }

        /// <summary>
        ///IV上限
        /// </summary>
        public double? Define63 { get; set; }

        /// <summary>
        ///IV下限
        /// </summary>
        public double? Define64 { get; set; }

        /// <summary>
        ///IV异常值1
        /// </summary>
        public double? Define65 { get; set; }

        /// <summary>
        ///IV异常值2
        /// </summary>
        public double? Define66 { get; set; }

        /// <summary>
        ///IV测试时间
        /// </summary>
        public double? Define67 { get; set; }

        /// <summary>
        ///工位1对应通道
        /// </summary>
        public double? Define68 { get; set; }

        /// <summary>
        ///工位2对应通道
        /// </summary>
        public double? Define69 { get; set; }

        /// <summary>
        ///工位4对应通道
        /// </summary>
        public double? Define70 { get; set; }

        /// <summary>
        ///工位4对应通道
        /// </summary>
        public double? Define71 { get; set; }
        /// <summary>
        ///头部最小值
        /// </summary>
        public double? Define72 { get; set; }
        /// <summary>
        ///头部最大值
        /// </summary>
        public double? Define73 { get; set; }
        /// <summary>
        ///尾部最小值
        /// </summary>
        public double? Define74 { get; set; }
        /// <summary>
        ///尾部最大值
        /// </summary>
        public double? Define75 { get; set; }

        /// <summary>
        ///AC电池层数
        /// </summary>
        public int? Define76 { get; set; }
        /// <summary>
        ///AC Width
        /// </summary>
        public int? Define77 { get; set; }
        /// <summary>
        ///AC Height
        /// </summary>
        public int? Define78 { get; set; }
        /// <summary>
        ///BD角层数
        /// </summary>
        public int? Define79 { get; set; }
        /// <summary>
        ///Width
        /// </summary>
        public int? Define80 { get; set; }
        /// <summary>
        ///Height
        /// </summary>
        public int? Define81 { get; set; }
        /// <summary>
        ///MI
        /// </summary>
        public string MI { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        public string BarcodeLenth { get; set; }
        

        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }
    }
}
