using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class TipQueue : MonoBehaviour
    {
        public struct Tip
        {
            public string Msg;
            public float Life;
            public float CreateTime;

            public bool IsExpired()
            {
                if (Life <= 0)
                    return false;
                else
                    return CreateTime + Life < Time.time;
            }
        }

        LinkedList<Tip> _tips = new LinkedList<Tip>();
        Rect _showRect = new Rect();
        GUIStyle _tipStyle;
        int _wndId;

        private void Start()
        {
            if (!_rectIsSet)
            {
                var w = W(0.2f);
                var h = _sh;
                SetRect(_sw - w, 0, w, h);
            }

            _wndId = Rand.Next(10000);
        }

        public void AddTip(string msg,float life=3)
        {
            _tips.AddLast(new Tip() { Msg = msg, Life = life });
        }

        bool _rectIsSet = false;
        public void SetRect(float x, float y, float w, float h)
        {
            _showRect.x = x;
            _showRect.y = y;
            _showRect.width = w;
            _showRect.height = h;
            _rectIsSet = true;
        }

        private void Update()
        {
            var p = _tips.First;
            while (p!=null)
            {
                var n = p.Next;
                if (p.Value.IsExpired())
                    _tips.Remove(p);

                p = n;
            }
        }

        private float _sw { get { return Screen.width; } }
        private float _sh { get { return Screen.height; } }
        private float W(float scale) { return Screen.width * scale; }
        private float H(float scale) { return Screen.height * scale; }

        private void OnGUI()
        {
            _showRect = GUILayout.Window(_wndId, _showRect, DrawTips, "Tips");
        }

        void DrawTips(int id)
        {
            if (_tipStyle == null)
            {
                _tipStyle = new GUIStyle(GUI.skin.button);
                _tipStyle.alignment = TextAnchor.MiddleLeft;
                _tipStyle.wordWrap = true;
            }

            var p = _tips.First;
            while (p != null)
            {
                var t = p.Value;
                GUILayout.Box(t.Msg,_tipStyle);
                p = p.Next;
            }
        }
    }
}
