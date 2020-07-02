using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScriptableObject<T>: SingletonScriptableObject<T> where T : ScriptableObject {

    [SerializeField]
    public string SheetName;

    public virtual bool Load()
    {
        return false;
    }
}
