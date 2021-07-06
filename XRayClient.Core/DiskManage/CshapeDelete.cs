using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XRayClient.Core
{
    class CshapeDelete
    {
        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public long GetDirectoryLength(string dirPath)
        {
            long len = 0;
            //判断该路径是否存在（是否为文件夹）
            if (!Directory.Exists(dirPath))
            {
                //查询文件的大小
                len = FileSize(dirPath);
            }
            else
            {
                //定义一个DirectoryInfo对象
                DirectoryInfo di = new DirectoryInfo(dirPath);

                //通过GetFiles方法，获取di目录中的所有文件的大小
                foreach (FileInfo fi in di.GetFiles())
                {
                    len += fi.Length;
                }
                //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                DirectoryInfo[] dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    for (int i = 0; i < dis.Length; i++)
                    {
                        len += GetDirectoryLength(dis[i].FullName);
                    }
                }
            }
            return len;
        }

        /// <summary>
        /// 获取路径文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public long FileSize(string filePath)
        {
            //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// 删除路径上所有文件
        /// </summary>
        /// <param name="srcPath">路径</param>
        public  void DelectDir(string srcPath)
        {
            if (Directory.Exists(srcPath))
            {
            }
            else
            {
                return;
            }

            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }


        /// add by fjy 2018.09.06  
        /// <summary>
        /// 定期清理电池图片，只删除文件名格式为"yyyyMMddHH"且过期的文件夹
        /// </summary>
        /// <param name="dirPath">文件夹根路径</param>  
        /// <param name="CleanDay">数据保存时间</param> 
        /// <returns></returns>
        public bool CleanDir(string dirPath, int CleanDay)
        {
            DateTime PreDatetime = DateTime.Now.AddDays(0 - CleanDay);
            int PreYear = int.Parse(PreDatetime.ToString("yyyy"));
            int PreMonth = int.Parse(PreDatetime.ToString("MM"));
            int PreDay = int.Parse(PreDatetime.ToString("dd"));
            int PreHour = int.Parse(PreDatetime.ToString("HH"));

            if (Directory.Exists(dirPath))
            {
            }
            else
            {
                return false;
            }

            try
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    string DirectoryName = i.Name;
                    int Len = DirectoryName.Length;
                    int CurYear = 0;
                    int CurMonth = 0;
                    int CurDay = 0;
                    int CurHour = 0;
                    bool res = true;
                    if (Len == 10)
                    {
                        res = int.TryParse(DirectoryName.Substring(0, 4), out CurYear);
                        res = int.TryParse(DirectoryName.Substring(4, 2), out CurMonth);
                        res = int.TryParse(DirectoryName.Substring(6, 2), out CurDay);
                        res = int.TryParse(DirectoryName.Substring(8, 2), out CurHour);
                    }
                    else
                    {
                        res = false;
                    }

                    if ((CurYear < PreYear
                        || (CurYear <= PreYear && CurMonth < PreMonth)
                        || (CurYear <= PreYear && CurMonth <= PreMonth && CurDay < PreDay)
                        || (CurYear <= PreYear && CurMonth <= PreMonth && CurDay <= PreDay && CurHour < PreHour)) && res)
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

    }
}
