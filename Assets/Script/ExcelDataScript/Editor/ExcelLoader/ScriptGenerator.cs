using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Reflection;


public class ScriptGenerator {

	enum eType
    {
        INT,
        DOUBLE,
        FLOAT,
        STRING,
        LONG,
        BOOL,
        MAX,
    }
       

    IWorkbook m_Book = null;
    string m_FileName = string.Empty;

    string ScriptRootPath
    {
        get
        {
            string RootPath = string.Format("{0}/{1}", ExcelLoaderEditor.scriptableObjectScriptDir, m_FileName);

            if (Directory.Exists(RootPath) == false)
            {
                Directory.CreateDirectory(RootPath);
            }

            return RootPath;
        }
    }

    string ScriptEditorRootPath
    {
        get
        {
            string RootPath = string.Format("{0}/{1}", ExcelLoaderEditor.scriptableObjectScriptEditorDir, m_FileName);

            if (Directory.Exists(RootPath) == false)
            {
                Directory.CreateDirectory(RootPath);
            }

            return RootPath;
        }
    }


    string SheetTemplateFilePath = "Assets/Script/ExcelDataScript/Editor/ExcelLoader/ScriptTemplete/SheetDataTemplate.txt";
    string ScriptableObjectDataTemplateFilePath = "Assets/Script/ExcelDataScript/Editor/ExcelLoader/ScriptTemplete/ScriptableObjectDataTemplate.txt";
    string ScriptableObjectDataTemplateUtilFilePath = "Assets/Script/ExcelDataScript/Editor/ExcelLoader/ScriptTemplete/ScriptableObjectDataTemplate_Util.txt";
    string ScrtipableObjectEditorTemplateFilePath = "Assets/Script/ExcelDataScript/Editor/ExcelLoader/ScriptTemplete/ScriptableObjectEditorClass.txt";

    enum eValueType
    {
        VALUE_TYPE = 0,
        VALUE_NAME,
        MAX,
    }

    List<ISheet> sheets = new List<ISheet>();
    string m_Path = string.Empty;

	StringBuilder SheetClass = new StringBuilder();

    public ScriptGenerator(IWorkbook book, string FileName, string path)
    {
        m_Book = book;
        m_FileName = FileName;
        m_Path = path;

        int numberOfSheets = m_Book.NumberOfSheets;

		SheetClass = new StringBuilder();

        for (int sheetIndex = 0; sheetIndex < numberOfSheets; ++sheetIndex)
        {
            ISheet sheet = m_Book.GetSheetAt(sheetIndex);
            sheets.Add(sheet);
			string DataClass = SheetDataScriptGenerator(sheet);

			if(string.IsNullOrEmpty(DataClass) == false)
			{
				SheetClass.Append(DataClass);
				SheetClass.AppendLine();            
			}         
        }
        ScriptableObjectGenerator();
        ScriptableObjectEditorGenerator();
        AssetDatabase.Refresh();

    }

    string SheetDataScriptGenerator(ISheet sheet)
    {
        string SheetTemplate = File.ReadAllText(SheetTemplateFilePath);

        int firstRowNum = sheet.FirstRowNum;
        int lastRowNum = sheet.LastRowNum;

        SheetTemplate = SheetTemplate.Replace("$ExcelSheetData$", sheet.SheetName);
        SheetTemplate = SheetTemplate.Replace("$Templete$", sheet.SheetName);

        StringBuilder strBuilder = new StringBuilder();

        IRow row = sheet.GetRow(firstRowNum);
        if (row == null)
			return string.Empty;

        int firstCellNum = row.FirstCellNum;
        int lastCellNum = row.LastCellNum;

        int valueCount = lastCellNum - firstCellNum;

        for (int i = row.FirstCellNum; i < lastCellNum; ++i)
        {
            ICell cell = row.GetCell(i);
         
			strBuilder.Append(GetProperty(cell.StringCellValue, cell.CellComment));

            string[] CellValue = cell.StringCellValue.Split('_');

            strBuilder.AppendLine();
            strBuilder.Append("\t"); 
        }
        
        string csFileName = string.Format("{0}/{1}.cs", ScriptRootPath, sheet.SheetName);
        SheetTemplate = SheetTemplate.Replace("$Types$", strBuilder.ToString());
              
		string[] sheetLines = SheetTemplate.ToString().Split('\n');

		StringBuilder builder = new StringBuilder();

		foreach(string line in sheetLines)
		{
			builder.Append(string.Format("\t\t{0}", line));
			builder.AppendLine();
		}
      
		return builder.ToString();
    }

	string GetProperty(string Cell, IComment comment)
    {
		string[] CellValue = ExcelLoaderEditor.GetTypeValue(Cell);

        if (CellValue.Length < (int)eValueType.MAX)
            return string.Empty;
      
        StringBuilder strBuilder = new StringBuilder();

        strBuilder.Append("[SerializeField]");
		strBuilder.AppendLine();  
		strBuilder.Append(string.Format("\tprivate {0} m_{1}", CellValue[(int)eValueType.VALUE_TYPE], CellValue[(int)eValueType.VALUE_NAME]));

		string TypeValue = CellValue[(int)eValueType.VALUE_TYPE];

		if(CellValue[(int)eValueType.VALUE_TYPE].Contains("[]"))
		{
			string type = CellValue[(int)eValueType.VALUE_TYPE];

			type = type.Replace("[]", "[0]");         
			strBuilder.Append(string.Format(" = new {0}", type));

			TypeValue = TypeValue.Replace("[]", "");
             
		}
      
		strBuilder.Append(";");      
        strBuilder.AppendLine();
              
		if(comment != null){         
			strBuilder.Append("\t/// <summary>\n");
			strBuilder.Append(string.Format("\t///{0}\n", comment.String.String));
			strBuilder.Append("\t/// </summary>\n");
		}
      
        strBuilder.Append(string.Format("\tpublic {0} {1}", CellValue[(int)eValueType.VALUE_TYPE], CellValue[(int)eValueType.VALUE_NAME]));
        strBuilder.AppendLine();
        strBuilder.Append("\t{ get { return " + "m_" + CellValue[(int)eValueType.VALUE_NAME] + "; } set { " + "m_" + CellValue[(int)eValueType.VALUE_NAME] + " = value; } }");

        return strBuilder.ToString();
    }


    void ScriptableObjectGenerator()
    {
        string ScriptableObjectTemplate = File.ReadAllText(ScriptableObjectDataTemplateFilePath);
        string ScriptableObjectDateTemplateUtilFile = File.ReadAllText(ScriptableObjectDataTemplateUtilFilePath);


        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$ExcelData$", m_FileName);
        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$Path$", m_Path);

        StringBuilder strBuilder = new StringBuilder();

        StringBuilder LoadStringBuilder = new StringBuilder();

        int numberOfSheets = m_Book.NumberOfSheets;

        LoadStringBuilder.Append("ExcelQuery query = null;");
        LoadStringBuilder.AppendLine();

        for (int sheetIndex = 0; sheetIndex < numberOfSheets; ++sheetIndex)
        {
            ISheet sheet = m_Book.GetSheetAt(sheetIndex);

            strBuilder.Append("[SerializeField]");
            strBuilder.AppendLine();
            strBuilder.Append("\t\t");
            strBuilder.Append("public");
            strBuilder.Append(' ');
            strBuilder.Append(string.Format("List<{0}>", sheet.SheetName));
            strBuilder.Append(' ');
            strBuilder.Append(string.Format("{0}", sheet.SheetName.ToLower()));
            strBuilder.Append(' ');
            strBuilder.Append(string.Format("= new List<{0}>();", sheet.SheetName));
            strBuilder.AppendLine();
            strBuilder.Append("\t");


            LoadStringBuilder.Append(string.Format("\t\t\tquery = new ExcelQuery(path, #{0}#);", sheet.SheetName));
            LoadStringBuilder.AppendLine();
            LoadStringBuilder.Append("\t\t\tif (query != null && query.IsValid())");
            LoadStringBuilder.AppendLine();
            LoadStringBuilder.Append(string.Format("\t\t\t\t{0} = query.Deserialize<{1}>();", sheet.SheetName.ToLower(), sheet.SheetName));
            LoadStringBuilder.AppendLine();
            LoadStringBuilder.Append("\t\t\telse");
            LoadStringBuilder.AppendLine();
            LoadStringBuilder.Append("\t\t\t\treturn false;");
            LoadStringBuilder.AppendLine();

            LoadStringBuilder.Replace('#', '"');
        }


        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$DataDeserialize$", LoadStringBuilder.ToString());

        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$Types$", strBuilder.ToString());      
		ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$DataClass$", SheetClass.ToString());

        string csFileName = string.Format("{0}/{1}.cs", ScriptRootPath, m_FileName);
        File.WriteAllText(csFileName, ScriptableObjectTemplate, Encoding.UTF8); 

        csFileName = string.Format("{0}/{1}_Util.cs", ScriptRootPath, m_FileName);

        if (File.Exists(csFileName) == false)
        {
            ScriptableObjectDateTemplateUtilFile = ScriptableObjectDateTemplateUtilFile.Replace("[System.Serializable]", "");
            ScriptableObjectDateTemplateUtilFile = ScriptableObjectDateTemplateUtilFile.Replace("$ExcelData$", m_FileName);

            File.WriteAllText(csFileName, ScriptableObjectDateTemplateUtilFile, Encoding.UTF8);     
        }
    }

    void ScriptableObjectEditorGenerator()
    {
        string ScriptableObjectTemplate = File.ReadAllText(ScrtipableObjectEditorTemplateFilePath);

        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$ClassName$", string.Format("{0}Editor", m_FileName));
        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$WorkSheetClassName$", string.Format("{0}", m_FileName));
              
        StringBuilder DataPropertyBuilder = new StringBuilder();
        StringBuilder DataPropertyInitalizeBuilder = new StringBuilder();

        StringBuilder DataPropertyInspectorBuilder = new StringBuilder();
        
        for (int i = 0; i < sheets.Count; ++i)
        {
            string PropertyName = string.Format("m_{0}Property", sheets[i].SheetName);
            string Variable = sheets[i].SheetName.ToLower();

            DataPropertyBuilder.Append(string.Format("SerializedProperty {0};", PropertyName));
            DataPropertyBuilder.AppendLine();
            DataPropertyBuilder.Append("\t");

            DataPropertyInitalizeBuilder.Append(string.Format("{0} = targetObject.FindProperty(#{1}#);", PropertyName, Variable));
            DataPropertyInitalizeBuilder.AppendLine();
            DataPropertyInitalizeBuilder.Append('\t');

            DataPropertyInspectorBuilder.Append(string.Format("GUIHelper.DrawSerializedProperty({0});", PropertyName));
            DataPropertyInspectorBuilder.AppendLine();
            DataPropertyInspectorBuilder.Append('\t');

        }

        DataPropertyInspectorBuilder.Append(string.Format("\t\ttargetObject.ApplyModifiedProperties();"));

        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$DataProperty$", DataPropertyBuilder.ToString());
        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$DataPropertyInitalize$", DataPropertyInitalizeBuilder.ToString());

        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace("$DataPropertyInspector$", DataPropertyInspectorBuilder.ToString());
      
        ScriptableObjectTemplate = ScriptableObjectTemplate.Replace('#', '"');
                    
        string csFileName = string.Format("{0}/{1}Editor.cs", ScriptEditorRootPath, m_FileName);
        File.WriteAllText(csFileName, ScriptableObjectTemplate, Encoding.UTF8);
    }


    void CreateScriptableObject()
    {
      
        BaseScriptableObject<ScriptableObject> obj = ScriptableObject.CreateInstance(m_FileName) as BaseScriptableObject<ScriptableObject>;

        obj.SheetName = m_Path;

        AssetDatabase.CreateAsset(obj, string.Format("{0}/{1}.asset", ExcelLoaderEditor.scriptableObjectDir, m_FileName));
    }
}
#endif