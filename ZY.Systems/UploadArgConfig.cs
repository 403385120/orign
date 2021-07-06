using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZY.Systems
{
	public partial class UploadArgConfig : EsqDbBusinessEntity
	{
		public UploadArgConfig()
		{
			selectTable = "UploadArgConfig";
			saveTable = "UploadArgConfig";
			backupTable = "bak_UploadArgConfig";
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
		/// 参数代码
		/// </summary>
		public string ArgCode
		{
			set; get;
		}
		/// <summary>
		/// 参数描述
		/// </summary>
		public string ArgDesc
		{
			set; get;
		}
		/// <summary>
		/// 参数值
		/// </summary>
		public string ArgVal
		{
			set; get;
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark
		{
			set; get;
		}
		#endregion Model

	}
}
