using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
namespace ExcelDataClass
{
	[System.Serializable]
	public partial class TextTable : BaseScriptableObject<TextTable>
	{	
		[SerializeField]
		public List<UI> ui = new List<UI>();
	[SerializeField]
		public List<Item> item = new List<Item>();
	
		
				[System.Serializable]
		public class UI : BaseData<UI>
		{	
			[SerializeField]
			private string m_Key;
			/// <summary>
			///키값
			/// </summary>
			public string Key
			{ get { return m_Key; } set { m_Key = value; } }
			[SerializeField]
			private string m_KOR;
			/// <summary>
			///한글
			/// </summary>
			public string KOR
			{ get { return m_KOR; } set { m_KOR = value; } }
			
		}

		[System.Serializable]
		public class Item : BaseData<Item>
		{	
			[SerializeField]
			private string m_Key;
			/// <summary>
			///키값
			/// </summary>
			public string Key
			{ get { return m_Key; } set { m_Key = value; } }
			[SerializeField]
			private string m_KOR;
			/// <summary>
			///한글
			/// </summary>
			public string KOR
			{ get { return m_KOR; } set { m_KOR = value; } }
			
		}


		
#if UNITY_EDITOR

		public override bool Load()
		{
			string path = this.SheetName;
			
			if (!File.Exists(path))
				return false;

			ExcelQuery query = null;
			query = new ExcelQuery(path, "UI");
			if (query != null && query.IsValid())
				ui = query.Deserialize<UI>();
			else
				return false;
			query = new ExcelQuery(path, "Item");
			if (query != null && query.IsValid())
				item = query.Deserialize<Item>();
			else
				return false;

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			return true;
		}
#endif
		
	}
}