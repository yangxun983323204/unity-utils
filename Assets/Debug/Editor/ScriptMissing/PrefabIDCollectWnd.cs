using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YX.Yaml
{
    /// <summary>
    /// 收集prefab脚本ID的工具窗口
    /// </summary>
    public class PrefabIDCollectWnd : EditorWindow
    {
        public List<GameObject> Prefabs { get { return _prefabs; } set { _prefabs = value; } }
        [SerializeField]
        private List<GameObject> _prefabs = new List<GameObject>();
        private SerializedObject _serializedObj;
        private SerializedProperty _prefabsSP;

        private ScriptIDDatabase _db = new ScriptIDDatabase();

        [MenuItem("Debug/PrefabIDCollectWnd")]
        public static void Open()
        {
            var wnd = GetWindow<PrefabIDCollectWnd>("PrefabIDCollectWnd");
            wnd.Show();
            wnd.minSize = new Vector2(600, 400);
            wnd.maxSize = new Vector2(600, 400);
        }

        #region UI
        private void OnEnable()
        {
            _serializedObj = new SerializedObject(this);
            _prefabsSP = _serializedObj.FindProperty("_prefabs");
            _db.Log = true;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            DrawPrefabList();
            DrawSave();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private Vector2 _scroll;
        void DrawPrefabList()
        {
            GUILayout.Label("把要收集脚本ID的prefab拖放到下面的列表中");
            _serializedObj.Update();
            EditorGUI.BeginChangeCheck();

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MaxWidth(100000), GUILayout.MaxWidth(100000));
            EditorGUILayout.PropertyField(_prefabsSP, true);
            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObj.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        void DrawSave()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("加载记录"))
            {
                var p = EditorUtility.OpenFilePanel("加载记录", Path.GetDirectoryName(Application.dataPath), "yaml");
                if (File.Exists(p))
                {
                    _db.Load(p);
                }
            }
            if (GUILayout.Button("附加主记录"))
            {
                var p = EditorUtility.OpenFilePanel("附加主记录", Path.GetDirectoryName(Application.dataPath), "yaml");
                AddDatabase(p, true);
            }
            if (GUILayout.Button("附加从记录"))
            {
                var p = EditorUtility.OpenFilePanel("附加从记录", Path.GetDirectoryName(Application.dataPath), "yaml");
                AddDatabase(p, false);
            }
            if (GUILayout.Button("清空内存记录"))
            {
                ClearDatabase();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            bool newSave = false;
            if (GUILayout.Button("保存"))
            {
                if (!Save())
                    newSave = true;
            }

            if (newSave || GUILayout.Button("另存为"))
            {
                var savePath = EditorUtility.SaveFilePanel(newSave? "保存" : "另存为", Path.GetDirectoryName(Application.dataPath), "yx_script_id", "yaml");
                if (!string.IsNullOrEmpty(savePath))
                {
                    if (File.Exists(savePath))
                    {
                        int i = EditorUtility.DisplayDialogComplex("警告", "已存在同名文件，如何处理？", "覆盖", "合并", "取消");
                        Debug.Log(i);
                        if (i == 0)// 覆盖
                            SaveAs(savePath);
                        else if(i == 1)// 合并
                        {
                            AddDatabase(savePath, false);
                            SaveAs(savePath);
                        }
                    }
                    else
                        SaveAs(savePath);

                    if (newSave)
                        _db.Load(savePath);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        public void ClearDatabase()
        {
            _db.Clear();
        }

        public void AddDatabase(string p,bool addMaster)
        {
            if (File.Exists(p))
            {
                var tmpDb = new ScriptIDDatabase();
                tmpDb.Load(p);
                foreach (var item in tmpDb.All)
                {
                    if (addMaster)
                        _db.AddOrUpdate(item);
                    else
                        _db.Add(item);
                }
            }
            else
                Debug.LogError("不存在文件:" + p);
        }

        public void ParseList()
        {
            var parser = new PrefabParser();
            foreach (var go in _prefabs)
            {
                if (go!=null)
                {
                    var path = AssetDatabase.GetAssetPath(go);
                    parser.Load(path);
                    var monos = parser.GetMonoBehaviourNodes();
                    foreach (var item in monos)
                    {
                        var asset = AssetUtils.GetAssetFromId(item.ScriptFileID, item.ScriptGUID);
                        if (asset == null)
                            continue;

                        _db.AddOrUpdate(new ScriptIDDatabase.ScriptID() {
                            Name = AssetUtils.GetScriptName(asset),
                            FileID = item.ScriptFileID,
                            GUID = item.ScriptGUID
                        });
                    }
                }
            }
        }

        public bool Save()
        {
            if (!_db.Loaded)
                return false;
            else
            {
                ParseList();
                _db.Save();
                return true;
            }
        }

        public void SaveAs(string savePath)
        {
            var dir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            ParseList();
            _db.SaveAs(savePath);
        }
    }
}
