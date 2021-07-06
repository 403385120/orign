using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZYXray
{
    public class MotionEnum
    {
        public enum ScanGRR
        {
            获取条码,
            扫码完成,
        }

        public static ScanGRR EnumScanGrr = 0;

        public enum ScanGRRSingleScaner
        {
            获取条码1,
            扫码完成1,
            获取条码2,
            扫码完成2,
        }
        public static ScanGRRSingleScaner EnumScanGRRSingleScaner = 0;

        public enum ScanBarcode1
        {
            工位1触发扫码,
            工位1获取条码代号,
            工位1获取条码代号完成,
            工位1扫码完成,
        }

        public static ScanBarcode1 EnumScanBarcode1 = 0;

        public enum ScanBarcode2
        {
            工位2触发扫码,
            工位2获取条码代号,
            工位2获取条码代号完成,
            工位2扫码完成,
        }

        public static ScanBarcode2 EnumScanBarcode2 = 0;

        public enum UnLoadScan
        {
            下料扫码触发,
            下料扫码完成,
        }

        public static UnLoadScan EnumUnLoadScan = 0;

        public enum BarcodeBinding1
        {
            触发条码绑定,
            条码绑定完成,
        }

        public static BarcodeBinding1 EnumBarcodeBinding1 = 0;

        public enum BarcodeBinding2
        {
            触发条码绑定,
            条码绑定完成,
        }

        public static BarcodeBinding2 EnumBarcodeBinding2 = 0;


        public enum XRAYShot1
        {
            触发角一拍照,
            角一拍照,
            角一拍照完成,
            触发角二拍照,
            角二拍照完成,
        }

        public static XRAYShot1 EnumXrayShot1 = 0;

        public enum XRAYShot2
        {
            触发角三拍照,
            角三拍照,
            角三拍照完成,
            触发角四拍照,
            角四拍照完成,
            处理XRAY数据,
            XRAY完成,
        }

        public static XRAYShot2 EnumXrayShot2 = 0;

        public enum Sorting
        {
            触发分拣,
            分拣完成,
        }

        public static Sorting EnumSorting = 0;

        public enum IV
        {
            触发IV测试,
            判断工位一是否有料,
            判断工位二是否有料,
            判断工位三是否有料,
            判断工位四是否有料,
            获取IV数据,
            获取数据完成,
            处理IV数据,
            IV测试完成,
        }

        public static IV EnumIV = 0;

        public enum OCV
        {
            工位1触发OCV电压测试,
            工位2触发内阻测试,
            工位1触发内阻测试,
            工位2触发电压测试,
            OCV测试完成,
        }

        public static OCV EnumOCV = 0;

        public enum OCV1
        {
            工位1触发内阻测试,
            工位1内阻测试完成,
            工位1触发OCV电压测试,
            工位1OCV电压测试完成,
        }

        public static OCV1 EnumOCV1 = 0;

        public enum OCV2
        {
            工位2触发OCV电压测试,
            工位2OCV电压测试完成,
            工位2触发内阻测试,
            工位2内阻测试完成,
        }

        public static OCV2 EnumOCV2 = 0;

        public enum MDI1
        {
            工位1触发FQI检测,
            工位1发送拍照指令,
            工位1拍照完成,
            工位1获取尺寸数据,
            工位1FQI完成,
        }

        public static MDI1 EnumMDI1 = 0;

        public enum MDI2
        {
            工位2触发FQI检测,
            工位2发送拍照指令,
            工位2拍照完成,
            工位2获取尺寸数据,
            工位2FQI完成,
        }

        public static MDI2 EnumMDI2 = 0;

        public enum Thickness1
        {
            工位1触发测厚,
            工位1测厚完成
        }

        public static Thickness1 EnumThickness1 = 0;

        public enum Thickness2
        {
            工位2触发测厚,
            工位2测厚完成
        }

        public static Thickness2 EnumThickness2 = 0;

        public enum MDIThickness1
        {
            平台1工位1触发FQI检测,
            平台1工位1FQI完成,
            平台1工位2触发FQI检测,
            平台1工位2FQI完成,
            平台1触发测厚,
            平台1测厚完成,
        }

        public static MDIThickness1 EnumMdiThickness1 = 0;

        public enum MDIThickness2
        {
            平台2工位1触发FQI检测,
            平台2工位1FQI完成,
            平台2工位2触发FQI检测,
            平台2工位2FQI完成,
            平台2触发测厚,
            平台2测厚完成,
        }

        public static MDIThickness2 EnumMdiThickness2 = 0;

        public enum RemoveProduct
        {
            触发移除电池,
            移除电池完成,
        }

        public static RemoveProduct EnumRemoveProduct = 0;

        public enum IVGRR
        {
            获取IV数据,
            获取数据完成,
            处理数据,
            IVGRR完成,
        }

        public static IVGRR EnumIVGRR = 0;

        public enum ThicknessAndMDIGRR
        {
            获取前测厚数据,
            前测厚完成,
            获取后测厚数据,
            测厚完成,
            前FQI拍照,
            前FQI拍照完成,
            后FQI拍照,
            后FQI拍照完成,
            获取FQI数据,
            FQIGRR完成,
        }

        public static ThicknessAndMDIGRR EnumThicknessAndMdiGRR = 0;

        public enum XRAYGRRShot1
        {
            触发角一拍照,
            角一拍照完成,
            触发角二拍照,
            角二拍照完成,
        }

        public static XRAYGRRShot1 EnumXrayGRRShot1 = 0;

        public enum XRAYGRRShot2
        {
            触发角三拍照,
            角三拍照完成,
            触发角四拍照,
            角四拍照完成,
            发送拍照结果,
            XRAY完成,
        }

        public static XRAYGRRShot2 EnumXrayGRRShot2 = 0;

        public enum OCVGRR
        {
            工位1触发电压测试,
            工位2触发内阻测试,
            工位1触发内阻测试,
            工位2触发电压测试,
            OCV测试完成,
        }

        public static OCVGRR EnumOCVGRR = 0;

        public enum MDIAndPPGGRR
        {
            准备拍照,
            工位1触发拍照,
            工位1处理数据,
            工位1拍照完成,
            工位2触发拍照,
            工位2处理数据,
            工位2拍照完成,
            工位3触发拍照,
            工位3处理数据,
            工位3拍照完成,
            工位4触发拍照,
            工位4处理数据,
            工位4拍照完成,
            一组FQIGGR完成,
        }

        public enum PPGGRR
        {
            准备测厚,
            工位1触发测厚,
            工位1测厚完成,
            工位2触发测厚,
            工位2测厚完成,
            工位3触发测厚,
            工位3测厚完成,
            工位4触发测厚,
            工位4测厚完成,
            一组PPGGGR完成,
        }
        public static PPGGRR EnumPpgGrr = 0;

        public static MDIAndPPGGRR EnumMdiAndPpggrr = 0;

        public enum XRAYGRR
        {
            触发角1拍照,
            角1拍照完成,
            触发角2拍照,
            角2拍照完成,
            触发角3拍照,
            角3拍照完成,
            触发角4拍照,
            角4拍照完成,
            触发结果发送,
            发送结果完成,
        }
        public static XRAYGRR EnumXraygrr = 0;

        public enum XRARYGRRMDI
        {
            工位1触发FQI拍照,
            工位2触发FQI拍照,
            工位3触发FQI拍照,
            工位4触发FQI拍照,
            工位1拍照完成,
            工位2拍照完成,
            工位3拍照完成,
            工位4拍照完成,
        }
        public static XRARYGRRMDI EnumXraygrrMdi = 0;
    }
}
