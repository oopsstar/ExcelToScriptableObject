using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class BaseData<T>{

    public void SetField(string key, object data)
    {
        FieldInfo fieldInfo = this.GetType().GetField(key, BindingFlags.Public | BindingFlags.Instance);

        if (fieldInfo.FieldType == typeof(int))
        {
            fieldInfo.SetValue(this, (int)data);
        }
        else if (fieldInfo.FieldType == typeof(string))
        {
            fieldInfo.SetValue(this, (string)data);
        }
        else if (fieldInfo.FieldType == typeof(double))
        {
            fieldInfo.SetValue(this, (double)data);
        }
        else if(fieldInfo.FieldType == typeof(float))
        {
            fieldInfo.SetValue(this, (float)data);
        }


    }
	
}
