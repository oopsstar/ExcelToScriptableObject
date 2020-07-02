using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ExcelDataClass;
///
/// !!! Machine generated code !!!
///
namespace ExcelDataClass
{
	[CustomEditor(typeof(TextTable))]
	public class TextTableEditor : BaseExcelEditor<TextTable>
	{	    
		SerializedProperty m_UIProperty;
	SerializedProperty m_ItemProperty;
	
		
		public override void OnEnable()
		{
			base.OnEnable();

			m_UIProperty = targetObject.FindProperty("ui");
	m_ItemProperty = targetObject.FindProperty("item");
	

		}
		public override bool Load()
		{
			TextTable targetData = target as TextTable;

			return targetData.Load();
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUIHelper.DrawSerializedProperty(m_UIProperty);
	GUIHelper.DrawSerializedProperty(m_ItemProperty);
			targetObject.ApplyModifiedProperties();

		}
	}
}
