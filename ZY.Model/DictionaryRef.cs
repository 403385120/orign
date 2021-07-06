using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZY.Systems;

namespace ZY.Model
{
	/// <summary>
	/// DictionaryRef
	/// </summary>
	public partial class DictionaryRef : EsqDbBusinessEntity
	{
		public DictionaryRef()
		{
			selectTable = "DictionaryRef";
			saveTable = "DictionaryRef";
			backupTable = "bak_DictionaryRef";
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
		/// 
		/// </summary>
		public string RefKey
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public string RefCode
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public string RefCode2
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public string RefValue
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public string RefValue2
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public bool? RefSystem
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public string RefRemark
		{
			set; get;
		}
		/// <summary>
		/// 
		/// </summary>
		public bool? RefIsUse
		{
			set; get;
		}
		public bool IsSample { get; set; }

		#endregion Model

	}
}
