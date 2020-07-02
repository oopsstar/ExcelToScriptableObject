using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : class, new()
{
    private static GameObject m_root = null;
    public static GameObject managerRoot
    {
        get
        {
            m_root = GameObject.Find("Managers");
            if (m_root == null)
                m_root = new GameObject("Managers");
            return m_root;
        }
    }

    private static T m_Instance = null;
    public static bool IsCreate()
    {
        return !(m_Instance == null);
    }

    private static T GetInstance()
    {
        GameObject rootObj = managerRoot;

        if(Application.isPlaying)
        {
            if (rootObj != null)
                DontDestroyOnLoad(rootObj);
        }

        lock (typeof(T))
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(T)) as T;
            }

            if (m_Instance == null)
            {
                m_Instance = rootObj.AddComponent(typeof(T)) as T;
            }
        }

        return m_Instance;
    }

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                GetInstance();
            }
            return m_Instance;
        }
        set
        {
            m_Instance = value;
        }
    }
}