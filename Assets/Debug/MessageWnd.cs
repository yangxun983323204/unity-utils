using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class MessageWnd : MonoBehaviour
    {
        public bool Expand = false;
        public bool ListenLog = true;
        //public int MsgShowCount = 10;
        private List<MsgGroup> _msgGroups = new List<MsgGroup>();

        public const string TYPE_DEBUG = "Debug";
        public const string TYPE_WARNING = "Warning";
        public const string TYPE_ERROR = "Error";
        public const string TYPE_CUSTOM = "Custom";

        private MsgGroup _currGroup;
        private Msg _currMsg;

        public struct Msg
        {
            public string Title;
            public string Desc;
        }

        public class MsgGroup
        {
            public string Name;
            public int MaxCount { get; private set; }
            public LinkedList<Msg> MsgList = new LinkedList<Msg>();

            public MsgGroup(string name, int max)
            {
                Name = name;
                MaxCount = max;
            }

            public void AddMsg(string title, string desc)
            {
                if (MsgList.Count >= MaxCount)
                {
                    MsgList.RemoveFirst();
                }

                MsgList.AddLast(new Msg() { Title = title, Desc = desc });
            }

            public void Clear()
            {
                MsgList.Clear();
            }
        }

        Rect _foldRect;
        Rect _expandRect;

        Vector2 _expand_msgListPos;
        Vector2 _expand_msgDescPos;

        GUIStyle _btnStyle, _msgStyle, _textStyle;

        int _foldWndId;
        int _expendWndId;

        private void Awake()
        {
            _msgGroups.Add(new MsgGroup(TYPE_DEBUG, 999));
            _msgGroups.Add(new MsgGroup(TYPE_WARNING, 999));
            _msgGroups.Add(new MsgGroup(TYPE_ERROR, 999));
            _msgGroups.Add(new MsgGroup(TYPE_CUSTOM, 999));

            Application.logMessageReceived += OnRecUnityLog;
            _currGroup = GetGroup(TYPE_DEBUG);

            _foldWndId = Rand.Next(10000);
            _expendWndId = Rand.Next(10000);
        }

        private void Start()
        {
            _foldRect = new Rect(0, 0, Mathf.Min(W(0.1f), H(0.1f)), Mathf.Min(W(0.1f), H(0.1f)));
            _expandRect = new Rect(0, 0, _sw, _sh);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnRecUnityLog;
        }

        MsgGroup GetGroup(string name)
        {
            switch (name)
            {
                case TYPE_DEBUG:
                    return _msgGroups[0];
                case TYPE_WARNING:
                    return _msgGroups[1];
                case TYPE_ERROR:
                    return _msgGroups[2];
                case TYPE_CUSTOM:
                    return _msgGroups[3];
                default:
                    return null;
            }
        }

        public void AddMsg(string type,string title,string desc)
        {
            var g = GetGroup(type);
            if (g!=null)
            {
                g.AddMsg(title, desc);
            }
            else
            {
                g = GetGroup(TYPE_ERROR);
                g.AddMsg($"找不到名为[{type}]的消息分组", "标题:"+title + "\n内容:" + desc);
            }
        }

        void OnRecUnityLog(string str, string trace, LogType type)
        {
            if (!ListenLog)
                return;

            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    GetGroup(TYPE_ERROR).AddMsg(str, trace);
                    break;
                case LogType.Warning:
                    GetGroup(TYPE_WARNING).AddMsg(str, trace);
                    break;
                case LogType.Log:
                    GetGroup(TYPE_DEBUG).AddMsg(str, trace);
                    break;
                default:
                    GetGroup(TYPE_DEBUG).AddMsg(str, trace);
                    break;
            }
        }

        private float _sw { get { return Screen.width; } }
        private float _sh { get { return Screen.height; } }
        private float W(float scale) { return Screen.width * scale; }
        private float H(float scale) { return Screen.height * scale; }

        private void OnGUI()
        {
            if (_btnStyle == null)
            {
                _btnStyle = new GUIStyle(GUI.skin.button);
                _btnStyle.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
            }

            if (_msgStyle == null)
            {
                _msgStyle = new GUIStyle(GUI.skin.button);
                _msgStyle.alignment = TextAnchor.MiddleLeft;
                _msgStyle.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
            }

            if (_textStyle == null)
            {
                _textStyle = new GUIStyle(GUI.skin.textArea);
                _textStyle.fontSize = Mathf.Min(Screen.width, Screen.height) / 50;
            }

            if (!Expand)
                _foldRect = GUILayout.Window(_foldWndId, _foldRect, DrawFold, "MsgWnd");
            else
                _expandRect = GUILayout.Window(_expendWndId, _expandRect, DrawExpand, "MsgWnd");
        }

        void DrawFold(int id)
        {
            if (GUILayout.Button("展开", _btnStyle))
            {
                Expand = true;
            }

            GUI.DragWindow();
            FoldWndDragClip();
        }

        void DrawExpand(int id)
        {
            // table切换
            float tableH = H(0.05f);
            GUILayout.BeginHorizontal(GUILayout.Height(tableH));
            {
                if (GUILayout.Button("清空", _btnStyle, GUILayout.Width(W(0.05f)), GUILayout.Height(tableH)))
                {
                    foreach (var g in _msgGroups)
                    {
                        g.Clear();
                    }
                }
                for (int i = 0; i < _msgGroups.Count; i++)
                {
                    var g = _msgGroups[i];
                    if (GUILayout.Button(g.Name+$"({g.MsgList.Count})", _btnStyle, GUILayout.Height(tableH)))
                    {
                        _currGroup = g;
                        _currMsg = default;
                    }
                }

                if (GUILayout.Button("收起", _btnStyle, GUILayout.Width(W(0.05f)), GUILayout.Height(tableH)))
                {
                    Expand = false;
                }
            }
            GUILayout.EndHorizontal();
            // 消息列表
            float scrollH = H(0.5f);
            var lH = scrollH / 10;
            GUILayout.BeginVertical(GUILayout.Height(scrollH));
            {
                _expand_msgListPos = GUILayout.BeginScrollView(_expand_msgListPos);
                {
                    var p = _currGroup.MsgList.First;
                    while (p != null)
                    {
                        if (GUILayout.Button(p.Value.Title, _msgStyle, GUILayout.Height(lH)))
                        {
                            _currMsg = p.Value;
                        }
                        p = p.Next;
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            // 消息详情
            _expand_msgDescPos = GUILayout.BeginScrollView(_expand_msgDescPos);
            {
                if(!string.IsNullOrEmpty(_currMsg.Desc))
                    GUILayout.TextArea(_currMsg.Desc, _textStyle, GUILayout.Width(_expandRect.width), GUILayout.MinHeight(H(0.45F)));
            }
            GUILayout.EndScrollView();
            //
            GUI.DragWindow();
            ExpandWndDragClip();
        }

        void FoldWndDragClip()
        {
            if (_foldRect.xMin < 0)
                _foldRect.MoveXMin(0);
            else if (_foldRect.xMax > _sw)
                _foldRect.MoveXMax(_sw);

            if (_foldRect.yMin < 0)
                _foldRect.MoveYMin(0);
            else if (_foldRect.yMax > _sh)
                _foldRect.MoveYMax(_sh);
        }

        void ExpandWndDragClip()
        {
            if (_expandRect.xMin > W(0.8f))
                _expandRect.MoveXMin(W(0.8f));
            else if (_expandRect.xMax < W(0.2f))
                _expandRect.MoveXMax(W(0.2f));

            if (_expandRect.yMin > H(0.8f))
                _expandRect.MoveYMin(H(0.8f));
            else if (_expandRect.yMax < H(0.2f))
                _expandRect.MoveYMax(H(0.2f));
        }
    }
}
