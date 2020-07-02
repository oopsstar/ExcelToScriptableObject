using UnityEngine;
using System.Collections;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T m_Instance = null;
    public static T Instance 
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = ResourceLoader.Load<T>(string.Format("ScriptableObject/{0}.asset", typeof(T).Name));
            }

            return m_Instance;
        }
    }
}
