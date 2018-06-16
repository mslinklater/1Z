using UnityEngine;
using UnityEditor;
using System.IO;

namespace MSL
{
    public static class CustomAssetUtility
    {
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if(path == "")
            {
                path = "Assets";
            }
            else if(Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}