using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEditor;

public class ExcelLoader : MonoBehaviour {
    

    public static void ReadXLS(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            Debug.Log("ReadXLS:" + path);

            ReadBook(new HSSFWorkbook(fs), Path.GetFileNameWithoutExtension(path), path);
        }
    }

    public static void ReadXLSX(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            Debug.Log("ReadXLSX:" + path);

            ReadBook(new XSSFWorkbook(fs), Path.GetFileNameWithoutExtension(path), path);
        }
    }

    static public void ReadBook(IWorkbook book, string FileName, string path)
    {
        new ScriptGenerator(book, FileName, path);
        AssetDatabase.Refresh();
    }
}
