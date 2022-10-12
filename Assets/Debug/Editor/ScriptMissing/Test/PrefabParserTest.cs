using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using YX.Yaml;

namespace ScriptMissingTests
{
    public class PrefabParserTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Load()
        {
            var path = "Assets/Debug/Editor/ScriptMissing/Test/Main Camera.prefab";
            var parser = new PrefabParser();
            parser.Load(path);
            var list = parser.GetMonoBehaviourNodes();
            foreach (var i in list)
            {
                Debug.LogFormat("fileID: {0}, guid: {1}", i.ScriptFileID, i.ScriptGUID);
            }
        }
    }
}
