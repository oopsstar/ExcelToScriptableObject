using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceLoader : SingletonMonoBehaviour<ResourceLoader>
{
    public static string SecondResourceName = string.Empty;

    public const string BundleRootFolder = "Assets/AssetBundleResources";
    const string ScriptableObjectRootFolderName = "ScriptableObject";

    public static T Load<T>(string resPath, string Root = BundleRootFolder) where T : UnityEngine.Object
	{
		T obj = null;
		string path = string.Empty;

        if(resPath.Contains(Root))
        {
            resPath = resPath.Replace(string.Format("{0}/", Root), "");
        }

        if(Debug.isDebugBuild)
        {
            if (string.IsNullOrEmpty(SecondResourceName) == false)
            {
                string SecondPath = string.Empty;
                if(Path.HasExtension(resPath))
                {
                    SecondPath = resPath.Insert(resPath.IndexOf('.'), string.Format("_{0}",SecondResourceName));
                }
                else
                {
                    SecondPath = string.Format("{0}_{1}", resPath, SecondResourceName);
                }

                Debug.LogError("SecondResPath : " + SecondPath);

                obj = GetObject<T>(SecondPath, Root);
                if (obj != null)
                    return obj;
            }
        }

        return GetObject<T>(resPath, Root);
	}

    public static T GetObject<T>(string resPath, string Root) where T : UnityEngine.Object
    {
        T obj = null;
        string path = string.Empty;
        string FileName = Path.GetFileNameWithoutExtension(resPath);

#if UNITY_EDITOR
        string DirectoryName = Path.GetDirectoryName(resPath);

        //데이터만 먼저 폴더 읽도록
        if (DirectoryName == ScriptableObjectRootFolderName)
        {
            if (obj == null)
            {
                obj = Resources.Load<T>(resPath);
            }

            if (obj != null)
                return obj;
        }

        path = GetFindFileWithExtensionPath(resPath, Root);

        if (obj == null)
            obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#else
        AssetBundle bundle = AssetBundleDownLoader.Instance.LoadAssetBundle(Path.Combine(BundleRootFolder, resPath));

        if(bundle != null)
            obj = bundle.LoadAsset<T>(FileName);
#endif

        if (obj == null)
        {
            if (Path.HasExtension(resPath))
            {
                resPath = resPath.Remove(resPath.IndexOf('.'));
            }

            obj = Resources.Load<T>(resPath);
        }

        return obj;
    }


#if UNITY_EDITOR
    public static string GetFindFileWithExtensionPath(string resPath, string Root)
    {
        string path = string.Empty;
        string DirectoryName = Path.GetDirectoryName(resPath);
        string RootFolder = string.Empty;

        if (resPath.Contains(Root))
            RootFolder = DirectoryName;
        else
            RootFolder = Path.Combine(Root, DirectoryName);

        string FileName = Path.GetFileNameWithoutExtension(resPath);

        string[] strGUID = UnityEditor.AssetDatabase.FindAssets(string.Format("{0}", FileName), new string[] { RootFolder });

        List<string> resourcesPaths = new List<string>();

        for (int i = 0; i < strGUID.Length; ++i)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(strGUID[i]);
            resourcesPaths.Add(assetPath);
        }

        path = resourcesPaths.FindLast(delegate (string strPath)
        {
            string Directory = Path.GetDirectoryName(strPath);

            Directory = Directory.Replace("\\", "/");
            RootFolder = RootFolder.Replace("\\", "/");

            string strFileName = Path.GetFileNameWithoutExtension(strPath);


            if (Directory.Equals(RootFolder) && strFileName.Equals(FileName))
                return true;

            return false;
        });

        return path;
    }

#endif

    public const string TimelineResourceRoot = "Assets/EditorTimelineResources";

}
