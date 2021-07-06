using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayClient.Core
{
    public class CshapeDeleteIF
    {
        static private CshapeDelete _Shapedelete = new CshapeDelete();

        /// <summary>
        /// 检测指定路径中文件大小大于最大值时删除路径内所有的文件  CshapeDeleteIF.DetectDelete(@"d:\\Test", 77000); 检测d:\\Test文件夹内如果容量大于77KB将删除路径内的所有文件
        /// </summary>
        /// <param name="filedirname">指定的文件路径</param>
        /// <param name="filedirsizemax">容量上限（B）</param>
        /// <returns></returns>
        static public int DetectDelete(string filedirname, long filedirsizemax)
        {
            long DirectorySize = _Shapedelete.GetDirectoryLength(filedirname);
            if (DirectorySize > filedirsizemax)
            {
                _Shapedelete.DelectDir(filedirname);
            }

            return 0;
        }

        /// <summary>
        /// 删除filedirname路径上ClearDaynum天前的所有文件夹
        /// </summary>
        /// <param name="filedirname"></param>
        /// <param name="ClearDaynum"></param>
        /// <returns></returns>
        static public bool PeriodicallyDeleteDir(string filedirname, int ClearDaynum)
        {
            _Shapedelete.CleanDir(filedirname, ClearDaynum);

            return false;
        }
    }
}
