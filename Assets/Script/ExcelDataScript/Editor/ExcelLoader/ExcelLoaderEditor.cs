using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
#endif

#if UNITY_EDITOR
public class ExcelLoaderEditor : EditorWindow
{   
	enum eValueType
	{
		VALUE_TYPE = 0,
		VALUE_NAME,
		MAX,
	}

	IWorkbook m_workbook = null;
	string FilePath = string.Empty;
	string FileName = string.Empty;

	public class Cell
	{
		public string FieldType;
		public string FieldName;
	}

	List<ISheet> m_Sheets = new List<ISheet>();
	Dictionary<ISheet, List<Cell>> m_Cells = new Dictionary<ISheet, List<Cell>>();

	Vector2 ScorollValue = Vector2.zero;


	[MenuItem("Utility/ExcelLoader/ExcelLoaderTool")]
	static void OnShow()
	{
		ExcelLoaderEditor editor = (ExcelLoaderEditor)EditorWindow.GetWindow(typeof(ExcelLoaderEditor));
		editor.Show();
	}

	void OnInspectorUpdate()
	{
		if (string.IsNullOrEmpty(FilePath) == false && m_Sheets.Count == 0)
		{
			FileName = Path.GetFileNameWithoutExtension(FilePath);
			LoadExcel();
		}
		Repaint();
	}

	const string ExcelDataRootFolder = "Assets/Script/ExcelDataScript/Editor/ExcelLoader/ExcelData/";
	
	public const string scriptableObjectDir = "Assets/Resources/ScriptableObject";
	
	public const string scriptableObjectScriptDir = "Assets/Scripts/ExcelDataScript/ExcelData";
	
	public const string scriptableObjectScriptEditorDir = "Assets/Scripts/ExcelDataScript/Editor/ExcelLoader/ExcelScriptableEditor";
	
	private void OnGUI()
	{
		ScorollValue = EditorGUILayout.BeginScrollView(ScorollValue);

		if (GUILayout.Button("All Excel SourceCode Generator"))
		{
			AllSourceCodeGenerator();
		}
		if (GUILayout.Button("All ScriptableObject Generator"))
        {
			AllScriptableObjectGenerator();
        }
        if (GUILayout.Button("All ScriptableObject Update"))
        {
            AllScriptableObjectUpdate();
        }
      
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("FileName : ", GUILayout.Width(60));
		EditorGUILayout.LabelField(FileName);
		if (GUILayout.Button("FileOpen"))
		{
			FilePath = EditorUtility.OpenFilePanel("ExcelFile Open", "Assets/Editor/ExcelLoader", "xlsx,xls");
			if (string.IsNullOrEmpty(FilePath) == false)
			{
				FileName = Path.GetFileNameWithoutExtension(FilePath);
				LoadExcel();
			}
			else
			{
				FilePath = string.Empty;
				FileName = string.Empty;
			}

		}
		EditorGUILayout.EndHorizontal();


		if (string.IsNullOrEmpty(FilePath) == false)
		{
			DrawSheet();

			if (GUILayout.Button("SourceCode Generator"))
			{
				try
				{
					new ScriptGenerator(m_workbook, FileName, FilePath);

				}
				catch(Exception e)
				{
					EditorUtility.DisplayDialog("ValueType Error", string.Format("ValueType : {0}", e.ToString()), "OK");
				}            
			}

			if (GUILayout.Button("ScriptableObject Generator"))
			{
                if (Directory.Exists(scriptableObjectDir) == false)
                {
                    Directory.CreateDirectory(scriptableObjectDir);
                }

				OnScriptableObjectGenerator(FileName, FilePath);
			}

		}
		EditorGUILayout.EndScrollView();
	}

    void AllSourceCodeGenerator()
	{      
		string[] Files = Directory.GetFiles(ExcelDataRootFolder);

		try
		{
			foreach (string file in Files)
            {
                string extension = Path.GetExtension(file);

                IWorkbook workbook = null;

                if (extension == ".xls")
                {
                    workbook = GetReadXLS(file);
                }
                else if (extension == ".xlsx")
                {
                    workbook = GetReadXLSX(file);
                }

                if (workbook == null)
                    continue;

                string filename = Path.GetFileNameWithoutExtension(file);

                new ScriptGenerator(workbook, filename, file);

                Debug.LogError("File Extension: " + Path.GetExtension(file));

                Debug.LogError("File : " + file);
            }	

			EditorUtility.DisplayDialog("Success", "AllSourceCodeGenerator", "OK");
		}
		catch(Exception e)
		{
			EditorUtility.DisplayDialog("Failed", "AllSourceCodeGenerator", "OK");
		}      
	}

    void AllScriptableObjectGenerator()
	{
		string[] Files = Directory.GetFiles(ExcelDataRootFolder);

		try
		{         
            if(Directory.Exists(scriptableObjectDir) == false)
            {
                Directory.CreateDirectory(scriptableObjectDir);
            }

			foreach (string file in Files)
            {
				string extension = Path.GetExtension(file);

				if (extension != ".xlsx" && extension != ".xls")
					continue;

                string filename = Path.GetFileNameWithoutExtension(file);            
                OnScriptableObjectGenerator(filename, file, false);
            }

			EditorUtility.DisplayDialog("Success", "AllScriptableObjectGenerator", "OK");
		}
		catch(Exception e)
		{
			EditorUtility.DisplayDialog("Failed", "AllScriptableObjectGenerator", "OK");
		}
        
	}

    void AllScriptableObjectUpdate()
    {
        string[] Files = Directory.GetFiles(scriptableObjectDir);

        try
        {
            for (int i = 0; i < Files.Length; ++i)
            {
                string extension = Path.GetExtension(Files[i]);

                if (extension == ".meta")
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(Files[i]);
                ScriptableObject scriptableObject = ResourceLoader.Load<ScriptableObject>(string.Format("ScriptableObject/{0}.asset", fileName));
                Type scriptableType = scriptableObject.GetType();

                if (scriptableType != null && scriptableType.FullName.Contains("ExcelDataClass"))
                {
                    MethodInfo methodInfo = scriptableType.GetMethod("Load");
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(scriptableObject, null);
                        Debug.LogError("Load!!: " + scriptableType.Name);
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("e : " + e);
        }
    }


	void LoadExcel()
	{
		m_workbook = null;
		if (FilePath.EndsWith(".xls"))
		{
			m_workbook = GetReadXLS(FilePath);
		}
		else if (FilePath.EndsWith(".xlsx"))
		{
			m_workbook = GetReadXLSX(FilePath);
		}

		if (m_workbook != null)
		{
			m_Sheets.Clear();
			m_Cells.Clear();

			int numberOfSheets = m_workbook.NumberOfSheets;

			for (int sheetIndex = 0; sheetIndex < numberOfSheets; ++sheetIndex)
			{
				ISheet sheet = m_workbook.GetSheetAt(sheetIndex);
				IRow row = sheet.GetRow(sheet.FirstRowNum);

				List<Cell> CellTypeValue = new List<Cell>();

				if (row != null)
				{
					for (int i = 0; i < row.Cells.Count; ++i)
					{
						string[] cellValue = GetTypeValue(row.Cells[i].StringCellValue);

						if (cellValue.Length <= (int)eValueType.VALUE_TYPE || string.IsNullOrEmpty(cellValue[(int)eValueType.VALUE_TYPE]))
						{
							Debug.LogError("Not Value Type :" + row.Cells[i].StringCellValue);
							continue;
						}

						Cell cell = new Cell();
						cell.FieldType = cellValue[(int)eValueType.VALUE_TYPE];
						cell.FieldName = cellValue[(int)eValueType.VALUE_NAME];

						CellTypeValue.Add(cell);
					}
				}
				m_Cells.Add(sheet, CellTypeValue);
				m_Sheets.Add(sheet);
			}
		}
	}

	IWorkbook GetReadXLS(string path)
	{
		using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			return new HSSFWorkbook(fs);
		}
	}

	IWorkbook GetReadXLSX(string path)
	{
		using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			return new XSSFWorkbook(fs);
		}
	}

	void DrawSheet()
	{      
		for (int i = 0; i < m_Sheets.Count; ++i)
		{
			EditorGUILayout.LabelField(m_Sheets[i].SheetName);

			if (m_Cells.ContainsKey(m_Sheets[i]))
			{
				List<Cell> cells = m_Cells[m_Sheets[i]];


				if (cells.Count == 0)
					continue;

				for (int j = 0; j < cells.Count; ++j)
				{
                    

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(cells[j].FieldType, GUILayout.Width(60));
					EditorGUILayout.LabelField(cells[j].FieldName);
					EditorGUILayout.EndHorizontal();
				}
			}
		}
	}

	public static string[] GetTypeValue(string Cell)
	{
		string[] CellValue = Cell.Split('_');
      
		return CellValue;
	}

	void OnScriptableObjectGenerator(string file, string excelFilePath, bool ShowError = true)
	{
        string strType = string.Format("ExcelDataClass.{0}, Assembly-Csharp", file);

        Type type = Type.GetType(strType); 
		if (type == null)
		{
			if(ShowError)
			    EditorUtility.DisplayDialog("Failed", "ScriptableObject Generator Failed", "OK");
			return;
		}

        string filePath = string.Format("{0}/{1}.asset", scriptableObjectDir, file);

        if(File.Exists(filePath))
        {
            ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath(filePath, type) as ScriptableObject;
            FieldInfo[] fields = scriptableObject.GetType().GetFields();

            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].Name == "SheetName")
                {
                    fields[i].SetValue(scriptableObject, excelFilePath);
                }
            }
            AssetDatabase.ImportAsset(filePath);
        }
        else
        {
            ScriptableObject scriptableObject = ScriptableObject.CreateInstance(string.Format("ExcelDataClass.{0}", file));
            FieldInfo[] fields = scriptableObject.GetType().GetFields();

            for (int i = 0; i < fields.Length; ++i)
            {
                if (fields[i].Name == "SheetName")
                {
                    fields[i].SetValue(scriptableObject, excelFilePath);
                }
            }

            AssetDatabase.CreateAsset(scriptableObject, filePath);
        }


	}   
}
#endif