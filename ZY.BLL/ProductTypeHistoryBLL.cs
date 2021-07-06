using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.DAL;
using ZY.Model;

namespace ZY.BLL
{
    public partial class ProductTypeHistoryBLL
    {
        private static readonly ProductTypeHistoryDAL dal = new ProductTypeHistoryDAL();

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
        public static IList<ProductTypeHistory> GetList(string filters, string orderBy, int pageSize, int pageIndex, ref string errMsg)
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
        public static IList<ProductTypeHistory> GetList(string filters, int pageSize, int pageIndex, ref string errMsg)
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
        public static IList<ProductTypeHistory> GetList(string filters, string orderBy, int TopNo, ref string errMsg)
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
        public static IList<ProductTypeHistory> GetList(string filters, string orderBy, ref string errMsg)
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
        public static IList<ProductTypeHistory> GetList(string filters, int TopNo, ref string errMsg)
        {
            return dal.GetList(filters, string.Empty, TopNo, ref errMsg);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="filters">查询条件</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static IList<ProductTypeHistory> GetList(string filters, ref string errMsg)
        {
            return dal.GetList(filters, -1, ref errMsg);
        }


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static long Add(ProductTypeHistory model, ref string errMsg)
        {
            return dal.Add(model, ref errMsg);
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool Update(ProductTypeHistory model, ref string errMsg)
        {
            return dal.Update(model, ref errMsg);
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool Delete(ProductTypeHistory model, ref string errMsg)
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
    }
}
