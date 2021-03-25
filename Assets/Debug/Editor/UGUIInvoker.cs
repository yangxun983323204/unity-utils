using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;

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
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            Btn();
            Tog();
            Input();
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

        private void Mono()
        {
            EditorGUILayout.BeginHorizontal();
            _mono = EditorGUILayout.ObjectField(_mono, typeof(MonoBehaviour), true) as MonoBehaviour;
            _monoFunc = EditorGUILayout.TextField(_monoFunc);
            if (GUILayout.Button("Call") && _mono != null && _monoFunc!=null)
            {
                var t = _mono.GetType();
                var m = t.GetMethod(_monoFunc, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                m.Invoke(_mono, new object[] { });
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
