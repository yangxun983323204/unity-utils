using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YX
{
    public static class AssetUtils
    {
        public static Object GetAssetFromId(string fileID, string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            long fid;

            for (int i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var s = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out fid);
                if (s && fileID == fid.ToString())
                {
                    return asset;
                }
            }

            return null;
        }

        public static string GetScriptName(Object monoScript)
        {
            var script = monoScript as MonoScript;
            string name;
            if (script != null)
                name = script.GetClass().FullName;
            else
                throw new System.InvalidCastException(monoScript.ToString());

            return name;
        }

        public static Object GetMonoScript(string name,string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);

            for (int i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var script = asset as MonoScript;
                if (script != null)
                {
                    var c = script.GetClass().FullName;
                    if (c == name)
                    {
                        return asset;
                    }
                }
            }

            return null;
        }

        public static bool TrySearchClass(string fullName,ref string fileID,ref string guid,ref Object asset)
        {
            if (string.IsNullOrEmpty(fullName))
                return false;

            long fid;
            string fileName = fullName;
            int i = fullName.LastIndexOf(".");
            if (i>=0)
            {
                fileName = fullName.Substring(i+1);
            }

            var guids = AssetDatabase.FindAssets($"{fileName} t:Script");
            foreach (var g in guids)
            {
                var o = GetMonoScript(fullName, g);
                if (o!=null)
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o,out guid, out fid))
                    {
                        fileID = fid.ToString();
                        asset = o;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
