using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
	public partial class ParametricDataArray : EsqDbBusinessEntity
	{
		public ParametricDataArray()
		{
			selectTable = "ParametricDataArray";
			saveTable = "ParametricDataArray";
			backupTable = "bak_ParametricDataArray";
		}
		#region Model
		/// <summary>
		/// 
		/// </summary>
		public int? Iden
		{
			set; get;
		}
		/// <summary>
		/// MES名称
		/// </summary>
		public string name
		{
			set; get;
		}
		/// <summary>
		/// 数据类型
		/// </summary>
		public string dataType
		{
			set; get;
		}
		/// <summary>
		/// 描述
		/// </summary>
		public string description
		{
			set; get;
		}
		/// <summary>
		/// 数据名称
		/// </summary>
		public string argCode
		{
			set; get;
		}
		/// <summary>
		/// 是否NC
		/// </summary>
		public bool? isNc
		{
			set; get;
		}
		/// <summary>
		/// 数据字段
		/// </summary>
		public string dataField
		{
			set; get;
		}
		/// <summary>
		/// 数据大类
		/// </summary>
		public string categroy
		{
			set; get;
		}
		/// <summary>
		/// 大类
		/// </summary>
		public string setion
		{
			set; get;
		}
		/// <summary>
		/// 接口名称
		/// </summary>
		public string func
		{
			set; get;
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string remark
		{
			set; get;
		}
		/// <summary>
		/// 参数ID
		/// </summary>
		public string argID
		{
			set; get;
		}
		/// <summary>
		/// 参数结果
		/// </summary>
		public string argResult
		{
			set; get;
		}
		/// <summary>
		/// 参数类型
		/// </summary>
		public string argType
		{
			set; get;
		}

		public string argValue { get; set; }

		public string barCode { get; set; }
		#endregion Model

	}
}
