using ATL.WebService;
using ATL.WebService.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATL.Common;

namespace WebServiceDemo
{
    class Program
    {
        //设备上位机调用MES的WS接口
        static void Main(string[] args)
        {
            //Rest接口地址
            string url = "http://CNNDM1VTAPP/Apriso/httpServices/operations/MTWInterfaceServer";
            ////测试环境接口地址
            //string url = "http://aprisomom.atlbattery.com/Apriso/httpServices/operations/MTWInterfaceServer";
            ////生产环境接口地址
            //string url = "http://atlmes-w.atlbattery.com/Apriso/httpServices/operations/MTWInterfaceServer";

            MTWInterface mes = new MTWInterface(url);
            //MTW转型信息查询（分条前调用）
            try
            {
                TransformaionInfoRequest.Root request = new TransformaionInfoRequest.Root();
                request.Header = new TransformaionInfoRequest.Header();
                request.Header.InterfaceCode = "TransformaionInfo";
                request.Header.RequestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                request.RequestInfo = new TransformaionInfoRequest.RequestInfo();
                request.RequestInfo.Equipment = "APAC017Q";
                request.RequestInfo.OperationCode = "";
                request.RequestInfo.OperationType = "Mixing";
                request.RequestInfo.OrderNo = "";
                request.RequestInfo.WipOrderNo = "003010050100";

                string response = mes.RequestForMES(request.ToJSON());
                TransformaionInfoResponse.Root root = JsonNewtonsoft.FromJSON<TransformaionInfoResponse.Root>(response);
                if (root != null && root.Header.IsSuccess == "1" && root.ResponseInfo != null)
                {
                    string OrderStatus = root.ResponseInfo.OrderStatus;
                    Console.WriteLine($"OrderStatus: {OrderStatus}");
                }
                else
                {
                    Console.WriteLine($"Error: {response}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
    }
}
