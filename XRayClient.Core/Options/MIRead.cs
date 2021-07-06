using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XRayClient.Core.Options
{

    public struct MIConfig                 //MI配置属性结构体
    {
        private string _strMINum;           //MI号
        private string _strLayerNum;        //对应检测层数
        private string _strLayerNum_BD;     //双边测试使用
        private string _strSpecMin;         //规格小
        private string _strSpecMax;         //规格大

        public string StrMINum
        {
            get { return this._strMINum; }
            set { this._strMINum = value; }
        }

        public string StrLayerNum
        {
            get { return this._strLayerNum; }
            set { this._strLayerNum = value; }
        }

        public string StrLayerNum_BD
        {
            get { return this._strLayerNum_BD; }
            set { this._strLayerNum_BD = value; }
        }

        public string StrSpecMin
        {
            get { return this._strSpecMin; }
            set { this._strSpecMin = value; }
        }

        public string StrSpecMax
        {
            get { return this._strSpecMax; }
            set { this._strSpecMax = value; }
        }

    }; 

    public class MIRead
    {
        //分割字符串
        static public string _miFiledir = @"\\nd-app01\\XRaySpec\\MISpec.csv";
        static public List<MIConfig> listconfig = new List<MIConfig>();

        static public int changedir(string dir)
        {
            _miFiledir = dir;

            return 0;
        }

        static public int GetMiConfig(string keyword, ref MIConfig result)
        {
            if(0 == listconfig.Count)
            {
                ReadMIConfigData();
            }

            for(int n = 0; n < listconfig.Count; n++)
            {
                if(keyword == listconfig[n].StrMINum)
                {
                    result.StrLayerNum = listconfig[n].StrLayerNum;
                    result.StrLayerNum_BD = listconfig[n].StrLayerNum_BD;
                    result.StrMINum = listconfig[n].StrMINum;
                    result.StrSpecMax = listconfig[n].StrSpecMax;
                    result.StrSpecMin = listconfig[n].StrSpecMin;

                    return 0;
                }
            }


            return -1;
        }

        static public string SpiltStr(ref string strSrc, string strDims)
        {
            int iPos = strSrc.IndexOf(strDims);
            int iLength = strSrc.Count();
            string strGetStr = "";

            if (iPos > 0)
            {
                strGetStr = strSrc.Substring(0, iPos);
                strSrc = strSrc.Substring(iPos + 1);
            }
            else
            {
                return strSrc;
            }

            return strGetStr;

        }

        //读取MI配置数据
        static public int ReadMIConfigData()
        {
            int iRet = 0;
            string strTemp = string.Empty;
            string strData = string.Empty;
            string strDims = ",";
            listconfig.Clear();

            try
            {
                FileStream fs = new FileStream(_miFiledir, FileMode.Open, FileAccess.Read);
                StreamReader streamReader = new StreamReader(fs);
                string line = "";
                while ((line = streamReader.ReadLine()) != null)
                {

                    MIConfig MIConfig = new MIConfig();
                    MIConfig.StrMINum = SpiltStr(ref line, strDims);
                    MIConfig.StrLayerNum = SpiltStr(ref line, strDims);
                    MIConfig.StrLayerNum_BD = SpiltStr(ref line, strDims);
                    MIConfig.StrSpecMin = SpiltStr(ref line, strDims);
                    MIConfig.StrSpecMax = SpiltStr(ref line, strDims);

                    if (3 == MIConfig.StrMINum.Count())
                    {
                        listconfig.Add(MIConfig);
                    }

                }
            }
            catch(Exception e)
            {
                return -1;
            }
            

            return iRet;
        }



    }
}
