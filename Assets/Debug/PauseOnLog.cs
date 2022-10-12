#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        [Serializable]
        public class PausePoint
        {
            public string Name;
            public string Key;
            public KeyWordsType Type;
            public bool Enable;
        }

        internal class JsonWrap
        {
            public PausePoint[] KeyWords;
        }

        public PausePoint[] KeyWords = new PausePoint[0];

        private string _cacheFile = "YXPausePoint.config";

        private void Start()
        {
            if (File.Exists(_cacheFile))
            {
                try
                {
                    KeyWords = JsonUtility.FromJson<JsonWrap>(File.ReadAllText(_cacheFile,Encoding.UTF8)).KeyWords;
                }
                catch
                {
                    KeyWords = new PausePoint[0];
                }
            }
            Application.logMessageReceived += OnRecLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnRecLog;
            File.WriteAllText(_cacheFile, JsonUtility.ToJson(new JsonWrap() { KeyWords = KeyWords },true),Encoding.UTF8);
        }

        private void OnRecLog(string condition, string stackTrace, LogType type)
        {
            if (KeyWords.Length<=0)
                return;

            for (int i = 0; i < KeyWords.Length; i++)
            {
                var p = KeyWords[i];
                if (p == null || !p.Enable)
                    continue;

                MatchKeyWord(condition, p);
            }
        }

        private void MatchKeyWord(string log,PausePoint pp)
        {
            if (pp==null || string.IsNullOrEmpty(pp.Key))
                return;

            switch (pp.Type)
            {
                case KeyWordsType.Start:
                    if (log.StartsWith(pp.Key))
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
                case KeyWordsType.Contain:
                    if (log.Contains(pp.Key))
                    {
                        EditorApplication.isPaused = true;
                    }
                    break;
                case KeyWordsType.End:
                    if (log.EndsWith(pp.Key))
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