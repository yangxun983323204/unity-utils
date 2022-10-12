using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;
using System.Text;

namespace YX
{
    public class UGUIInvoker : EditorWindow
    {
        private Button _btn;
        private Toggle _tog;
        private InputField _input;
        private MonoBehaviour _mono;
        private string _monoFunc;

        [MenuItem("Debug/UGUI Invoker")]
        public static void Open()
        {
            var wnd = GetWindow<UGUIInvoker>("UGUI Invoker");
            wnd.Show();
            wnd.minSize = new Vector2(600, 400);
            wnd.maxSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            Btn();
            EditorGUILayout.Space();
            Tog();
            EditorGUILayout.Space();
            Input();
            EditorGUILayout.Space();
            Mono();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void Btn()
        {
            EditorGUILayout.BeginHorizontal();
            _btn = EditorGUILayout.ObjectField(_btn, typeof(Button), true) as Button;
            if (GUILayout.Button("Click") && _btn!=null)
            {
                _btn.onClick.Invoke();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Tog()
        {
            EditorGUILayout.BeginHorizontal();
            _tog = EditorGUILayout.ObjectField(_tog, typeof(Toggle), true) as Toggle;
            if (GUILayout.Button("Tog") && _tog != null)
            {
                _tog.isOn = !_tog.isOn;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Input()
        {
            EditorGUILayout.BeginHorizontal();
            _input = EditorGUILayout.ObjectField(_input, typeof(InputField), true) as InputField;
            if (GUILayout.Button("Submit") && _input != null)
            {
                _input.OnSubmit(null);
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool _preShowMonoMethods;
        private bool _showMonoMethods;
        private string _msCache;
        private Vector2 _scroll;

        private void Mono()
        {
            EditorGUILayout.BeginHorizontal();
            _mono = EditorGUILayout.ObjectField(_mono, typeof(MonoBehaviour), true) as MonoBehaviour;
            _monoFunc = EditorGUILayout.TextField(_monoFunc);
            if (GUILayout.Button("Call") && _mono != null && _monoFunc!=null)
            {
                var t = _mono.GetType();
                var m = t.GetMethod(_monoFunc.Trim(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                m.Invoke(_mono, new object[] { });
            }
            EditorGUILayout.EndHorizontal();

            _showMonoMethods = GUILayout.Toggle(_showMonoMethods,"ShowMethods");
            if (_showMonoMethods && _preShowMonoMethods != _showMonoMethods && _mono != null)
            {
                var t = _mono.GetType();
                var ms = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                StringBuilder sb = new StringBuilder(1000);
                foreach (var item in ms)
                {
                    sb.AppendLine(item.Name);
                }
                _msCache = sb.ToString();
            }

            if (_showMonoMethods)
            {
                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Width(580),GUILayout.Height(250));
                GUILayout.TextArea(_msCache);
                EditorGUILayout.EndScrollView();
            }
            _preShowMonoMethods = _showMonoMethods;
        }
    }
}
