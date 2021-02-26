#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YX
{
    /// <summary>
    /// 在log输出包含指定关键字的时候暂停编辑器，方便单步执行
    /// </summary>
    public class PauseOnLog : MonoBehaviour
    {
        public enum KeyWordsType
        {
            Start,
            Contain,
            End,
        }

        public KeyWordsType Type = KeyWordsType.Start;
        public string KeyWords;

        private void Start()
        {
            Application.logMessageReceived += OnRecLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnRecLog;
        }

        private void OnRecLog(string condition, string stackTrace, LogType type)
        {
            if (string.IsNullOrEmpty(KeyWords))
                return;

            switch (Type)
            {
                case KeyWordsType.Start:
                    if (condition.StartsWith(KeyWords))
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
                case KeyWordsType.Contain:
                    if (condition.Contains(KeyWords))
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
                case KeyWordsType.End:
                    if (condition.EndsWith(KeyWords))
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
#endif