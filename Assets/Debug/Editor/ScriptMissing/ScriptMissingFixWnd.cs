using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static YX.Yaml.ScriptIDDatabase;

namespace YX.Yaml
{
    /// <summary>
    /// 修复prefab上的脚本丢失的工具窗口
    /// </summary>
    public class ScriptMissingFixWnd : EditorWindow
    {
        public List<GameObject> Prefabs { get { return _prefabs; } set { _prefabs = value; } }
        [SerializeField]
        private List<GameObject> _prefabs = new List<GameObject>();
        private SerializedObject _serializedObj;
        private SerializedProperty _prefabsSP;

        private ScriptIDDatabase _db = new ScriptIDDatabase();
        private ScriptIDDatabase _refDb = new ScriptIDDatabase();

        public List<ScriptID> MissingList = new List<ScriptID>();
        public List<ScriptID> FixList = new List<ScriptID>();
        public List<ScriptID> NofixList = new List<ScriptID>();

        [MenuItem("Debug/ScriptMissingFixWnd")]
        public static void Open()
        {
            var wnd = GetWindow<ScriptMissingFixWnd>("ScriptMissingFixWnd");
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
            DrawTool();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private Vector2 _scroll;
        void DrawPrefabList()
        {
            GUILayout.Label("把要修复脚本丢失的prefab拖放到下面的列表中");
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

        void DrawTool()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("加载ID参考(主)"))
            {
                var p = EditorUtility.OpenFilePanel("加载ID参考(主)", Path.GetDirectoryName(Application.dataPath), "yaml");
                AddRefDatabase(p, true);
            }
            if (GUILayout.Button("加载ID参考(从)"))
            {
                var p = EditorUtility.OpenFilePanel("加载ID参考(从)", Path.GetDirectoryName(Application.dataPath), "yaml");
                AddRefDatabase(p, false);
            }
            if (GUILayout.Button("清除参考数据"))
            {
                ClearRefDB();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("分析"))
            {
                AnalyzePrefabs();
            }
            if (GUILayout.Button("修复"))
            {
                AnalyzePrefabs();
                Fix();
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        // 公开内部方法，以便直接脚本调用

        public void ClearRefDB()
        {
            _refDb.Clear();
        }

        public void AddRefDatabase(string p, bool addMaster)
        {
            if (File.Exists(p))
            {
                var tmpDb = new ScriptIDDatabase();
                tmpDb.Load(p);
                foreach (var item in tmpDb.All)
                {
                    if (addMaster)
                        _refDb.AddOrUpdate(item);
                    else
                        _refDb.Add(item);
                }
            }
            else
                Debug.LogError("不存在文件:" + p);
        }

        public void AnalyzePrefabs()
        {
            MissingList.Clear();
            FixList.Clear();
            NofixList.Clear();
            var parser = new PrefabParser();
            foreach (var go in _prefabs)
            {
                if (go != null)
                {
                    var path = AssetDatabase.GetAssetPath(go);
                    parser.Load(path);
                    var monos = parser.GetMonoBehaviourNodes();
                    foreach (var item in monos)
                    {
                        var asset = AssetUtils.GetAssetFromId(item.ScriptFileID, item.ScriptGUID);
                        if (asset == null) // missing
                        {
                            if (MissingList.Find(n=> { return n.FileID == item.ScriptFileID && n.GUID == item.ScriptGUID; })==null)
                            {
                                MissingList.Add(new ScriptID() { FileID = item.ScriptFileID, GUID = item.ScriptGUID });
                            }
                        }
                    }
                }
            }

            // 从参考数据库查询missing id的名字
            foreach (var item in MissingList)
            {
                var r = _refDb.Search(item.FileID, item.GUID);
                if (r != null)
                    item.Name = r.Name;
            }
            // 通过名字在项目中寻找其正确id
            foreach (var item in MissingList)
            {
                string fileID = null;
                string guid = null;
                Object _ = null;
                if (AssetUtils.TrySearchClass(item.Name, ref fileID, ref guid, ref _))
                {
                    FixList.Add(new ScriptID() { Name = item.Name,  FileID = fileID, GUID = guid });
                }
                else
                    NofixList.Add(item);
            }
            // log
            var sb = new StringBuilder(100);
            sb.AppendLine("分析结果如下:");
            sb.AppendLine("--------");
            sb.AppendLine($"丢失{MissingList.Count}:");
            foreach (var item in MissingList)
            {
                sb.AppendLine($"类名:{item.Name},fileID:{item.FileID},guid:{item.GUID}");
            }
            sb.AppendLine("--------");

            sb.AppendLine($"可修复{FixList.Count}:");
            foreach (var item in FixList)
            {
                sb.AppendLine($"类名:{item.Name},fileID:{item.FileID},guid:{item.GUID}");
            }
            sb.AppendLine("--------");

            sb.AppendLine($"不可修复{NofixList.Count}:");
            foreach (var item in NofixList)
            {
                sb.AppendLine($"类名:{item.Name},fileID:{item.FileID},guid:{item.GUID}");
            }
            sb.AppendLine("--------");
            Debug.Log(sb.ToString());
        }

        public void Fix()
        {
            StringBuilder sb = new StringBuilder(1024 * 1024);
            var logStr = new StringBuilder(100);
            logStr.AppendLine("修复结果如下:");
            logStr.AppendLine("--------");
            foreach (var go in _prefabs)
            {
                if (go != null)
                {
                    sb.Clear();
                    var path = AssetDatabase.GetAssetPath(go);
                    var fix = GetNeedFixList(path);
                    if (fix == null || fix.Count <= 0)
                        continue;

                    logStr.AppendLine("修复:" + path);
                    sb.Append(File.ReadAllText(path, Encoding.UTF8));
                    foreach (var item in fix)
                    {
                        var old = MissingList.Find(n => n.Name == item.Name);
                        string fmt = "fileID: {0}, guid: {1}";
                        sb.Replace(string.Format(fmt,old.FileID,old.GUID), string.Format(fmt, item.FileID, item.GUID));
                        logStr.AppendLine($"类名:{old.Name},fileID:{old.FileID},guid:{old.GUID}->类名:{item.Name},fileID:{item.FileID},guid:{item.GUID}");
                    }

                    File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
                    logStr.AppendLine("--------");
                }
            }

            logStr.AppendLine("--------");
            Debug.Log(logStr.ToString());
        }

        private List<ScriptID> GetNeedFixList(string prefabPath)
        {
            var needfix = new List<ScriptID>();
            var parser = new PrefabParser();
            parser.Load(prefabPath);
            var monos = parser.GetMonoBehaviourNodes();
            foreach (var item in monos)
            {
                var missing = MissingList.Find(n => n.FileID == item.ScriptFileID && n.GUID == item.ScriptGUID);
                if (missing != null)
                {
                    var fix = FixList.Find(n=>n.Name == missing.Name);
                    if (fix!=null)
                    {
                        needfix.Add(fix);
                    }
                }
            }

            return needfix;
        }
    }
}
