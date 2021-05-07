using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace YX
{
    public class AssetBundleExplorer : EditorWindow
    {
        [MenuItem("Debug/AssetBundleExplorer")]
        public static void Open()
        {
            var wnd = GetWindow<AssetBundleExplorer>("AssetBundleExplorer");
            wnd.Show();
            wnd.minSize = new Vector2(600, 400);
            wnd.maxSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            SetBundle();
            EditorGUILayout.Space();
            ListAsset();
            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            if (_bundle != null)
            {
                _bundle.Unload(true);
                _bundle = null;
            }
        }

        private DefaultAsset _assets;
        private AssetBundle _bundle;
        private string[] _list;

        void SetBundle()
        {
            EditorGUILayout.BeginHorizontal();
            if (Selection.activeObject != null)
            {
                EditorGUILayout.LabelField(Selection.activeObject.GetType().ToString());
            }
            _assets = EditorGUILayout.ObjectField(_assets, typeof(DefaultAsset), true) as DefaultAsset;
            if (GUILayout.Button("Open") && _assets != null)
            {
                if (_bundle!=null)
                {
                    _bundle.Unload(true);
                    _bundle = null;
                }

                var path = AssetDatabase.GetAssetPath(_assets);
                _bundle = AssetBundle.LoadFromFile(path);
                _list = _bundle.GetAllAssetNames();

            }
            EditorGUILayout.EndHorizontal();
        }


        private float _scrollValueX = 0;
        private float _scrollValueY = 0;
        private Vector2 _scroll;
        void ListAsset()
        {
            if (_list != null && _list.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(580), GUILayout.Height(250));
                foreach (var item in _list)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(item);
                    if (GUILayout.Button("open"))
                    {
                        OpenAsset(item);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
            }
        }

        void OpenAsset(string name)
        {
            if (_bundle == null)
                return;

            if (name.EndsWith(".bytes"))
            {
                var content = _bundle.LoadAsset<TextAsset>(name).text;
                var wnd = GetWindow<TextWindow>(name);
                wnd.Content = content;
                wnd.Show();
            }
        }
    }

    public class TextWindow : EditorWindow
    {
        public string Content { get; set; }

        private float _scrollValueX = 0;
        private float _scrollValueY = 0;
        private Vector2 _scroll;

        private void Awake()
        {
            minSize = new Vector2(1200, 400);
            maxSize = new Vector2(1200, 400);
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(1200), GUILayout.Height(400));
            GUILayout.TextArea(Content);
            EditorGUILayout.EndScrollView();
        }
    }
}
