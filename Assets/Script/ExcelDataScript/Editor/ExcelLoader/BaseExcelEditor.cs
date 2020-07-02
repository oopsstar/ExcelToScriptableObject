using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseExcelEditor<T> : BaseEditor<T> where T : ScriptableObject
{
    public override void OnEnable()
    {
        base.OnEnable();
    }

    /// <summary>
    /// Draw Inspector view.
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (target == null)
            return;

        // Update SerializedObject
        targetObject.Update();

        if (GUILayout.Button("Update"))
        {
            if (!Load())
            {
                const string error1 = "\n- Check the path of the 'Sheet Name' and the file is exist at the path.";
                const string error2 = "\n- Also check the excel file has the sheet which matches with 'Worksheet Name'.";
                EditorUtility.DisplayDialog(
                    "Error",
                    "Failed to import and update the excel file." + error1 + error2,
                    "OK"
                );
            }
        }

        EditorGUILayout.Separator();

        DrawInspector();
 
        targetObject.ApplyModifiedProperties();
    }


}