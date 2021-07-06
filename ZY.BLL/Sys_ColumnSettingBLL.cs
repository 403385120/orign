using Esquel.Utility;
using Permission.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.DAL;
using ZY.Model;
using ZY.Systems;

namespace ZY.BLL
{
    public class Sys_ColumnSettingBLL
    {
        private static readonly Sys_ColumnSettingDAL dal = new Sys_ColumnSettingDAL();

        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool IsExists(string filters, ref string errMsg)
        {
            return dal.IsExists(filters, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="rCount">总页数</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string filters, int pageSize, int pageIndex, ref int rCount, ref string errMsg)
        {
            return dal.GetDataTable(filters, pageSize, pageIndex, ref rCount, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string filters, ref string errMsg)
        {
            return dal.GetDataTable(filters, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<Sys_ColumnSetting> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
        {
            return dal.GetList(filters, orderBy, -1, pageSize, pageIndex, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="rCount">总页数</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<Sys_ColumnSetting> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
        {
            return dal.GetList(filters, string.Empty, -1, pageSize, pageIndex, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="TopNo">前几行</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<Sys_ColumnSetting> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
        {
            return dal.GetList(filters, orderBy, TopNo, -1, -1, ref errMsg);
        }

        /// <summary>
        /// 自定义排序字段进行刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="orderBy">字段排序</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<Sys_ColumnSetting> GetList(string filters, string orderBy, ref string errMsg)
        {
            return dal.GetList(filters, orderBy, ref errMsg);
        }

        /// <summary>
        /// 刷新前几行所需要数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="TopNo">前几行</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<Sys_ColumnSetting> GetList(string filters, int TopNo, ref string errMsg)
        {
            return dal.GetList(filters, string.Empty, TopNo, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<Sys_ColumnSetting> GetList(string filters, ref string errMsg)
        {
            return dal.GetList(filters, -1, ref errMsg);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static int Add(Sys_ColumnSetting model, ref string errMsg)
        {
            return dal.Add(model, ref errMsg);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool Update(Sys_ColumnSetting model, ref string errMsg)
        {
            return dal.Update(model, ref errMsg);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool Delete(Sys_ColumnSetting model, ref string errMsg)
        {
            return dal.Delete(model, ref errMsg);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="sql">一整条更新SQL</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool UpdateSQL(string sql, ref string errMsg)
        {
            return dal.UpdateSQL(sql, ref errMsg);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="sql">一整条更新SQL</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool ExecuteSQL(string sql, ref string errMsg)
        {
            return dal.ExecuteSQL(sql, ref errMsg);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool UpdateFiledsByFilters(string fileds, string filters, ref string errMsg)
        {
            return dal.UpdateFiledsByFilters(fileds, filters, ref errMsg);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="fileds">需要更新的表达式</param>
        /// <param name="filters">更新条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static string SelectFiledsByFilters(string fileds, string filters, ref string errMsg)
        {
            return dal.SelectFiledsByFilters(fileds, filters, ref errMsg);
        }

        public static bool Insert(List<Sys_ColumnSetting> list, List<Sys_ColumnSetting> lstDel, ref string errMsg)
        {
            DBHelper dBHelper = DBHelperFactory.CreateHelperFactory(Common.ConnectionString);
            dBHelper.SQLTransBegin();
            try
            {
                Sys_ColumnSettingDAL sys_ColumnSettingDAL = new Sys_ColumnSettingDAL();
                foreach (Sys_ColumnSetting item in list.FindAll((Sys_ColumnSetting o) => o.IsEdit.Equals(Esquel.BaseManager.Enums.EditorStatus.AddNew)))
                {
                    sys_ColumnSettingDAL.Add(item, dBHelper, true, ref errMsg);
                }
                foreach (Sys_ColumnSetting item2 in list.FindAll((Sys_ColumnSetting o) => o.IsEdit.Equals(Esquel.BaseManager.Enums.EditorStatus.Modify)))
                {
                    sys_ColumnSettingDAL.Update(item2, dBHelper, true, ref errMsg);
                }
                foreach (Sys_ColumnSetting item3 in lstDel)
                {
                    sys_ColumnSettingDAL.Delete(item3, dBHelper, true, ref errMsg);
                }
                dBHelper.SQLTransCommit();
                return true;
            }
            catch (Exception ex)
            {
                dBHelper.SQLRollback();
                errMsg = ex.Message;
            }
            return false;
        }
    }
}
