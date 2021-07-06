using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ATL.WebService
{
    public class MTWInterface
    {
        public MTWInterface()
        {
            url = "http://CNNDM1VTAPP/Apriso/httpServices/operations/MTWInterfaceServer";  //Rest接口地址
            ////测试环境接口地址
            //url = "http://aprisomom.atlbattery.com/Apriso/httpServices/operations/MTWInterfaceServer";
            ////生产环境接口地址
            //url = "http://atlmes-w.atlbattery.com/Apriso/httpServices/operations/MTWInterfaceServer";
        }

        public MTWInterface(string _url)
        {
            url = _url;
        }

        /// <summary>
        /// Rest接口地址
        /// </summary>
        public string url;

        /// <summary>
        /// post
        /// </summary>
        /// <param name="RequestJson">RequestJson = "{\"Header\":{\"InterfaceCode\":\"TransformaionInfo\",\"RequestTime\":\"2020-03-15 15:12:12\"},\"RequestInfo\":{\"WipOrderNo\":\"003010050100\",\"OperationType\":\"Mixing\",\"OrderNo\":\"\",\"Equipment\":\"APAC017Q\",\"OperationCode\":\"\"}}";</param>
        /// <returns></returns>
        public string RequestForMES(string RequestJson)
        {
            //套入Apriso 输入参数的标准格式，输入参数层级 Inputs
            object InputsJson = new
            {
                Inputs = new
                {
                    PostJson = RequestJson
                }
            };
            string jsonParam = Newtonsoft.Json.JsonConvert.SerializeObject(InputsJson);
            //创建rest的请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "post";
            request.ContentType = "application/json";
            //创建参数
            string data = jsonParam;
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            request.ContentLength = byteData.Length;
            //以流的形式附加参数
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }
            //获取输出结果
            var responseValue = string.Empty;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response == null)
                {
                    //失败;
                }
                //判断接口状态
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //失败;
                }
                //读取输出内容
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                //解析Apriso 输出的Json
                var ResponseJson = Newtonsoft.Json.Linq.JObject.Parse(responseValue);
                //Apriso 会给输出套一层 OutPuts，实际的输出内容在 Output层级之内
                var Outputs = ResponseJson["Outputs"];
                var ResponseResult = Newtonsoft.Json.Linq.JObject.Parse(Outputs["Result"].ToString());
                return ResponseResult.ToString();
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    //失败;
                    throw ex;
                }
                if (((System.Net.HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.OK)
                {
                    //失败;
                }
                StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream(), true);
                responseValue = streamReader.ReadToEnd();
                streamReader.Close();
                ex.Response.Close();
                //return responseValue.ToString();

                //解析Apriso 输出的Json
                var ResponseJson = Newtonsoft.Json.Linq.JObject.Parse(responseValue);
                //Apriso 会给输出套一层 OutPuts，实际的输出内容在 Output层级之内
                var ResponseResult = ResponseJson["Message"].ToString();

                //MessageBox.Show(ResponseResult);
                return ResponseResult.ToString();
            }
        }
    }
}
