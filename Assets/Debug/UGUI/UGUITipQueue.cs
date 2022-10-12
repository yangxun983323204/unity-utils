using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX
{
    public class UGUITipQueue : MonoBehaviour
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

        private int _fontSize = 30;
        public int FontSize {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                _hasChange = true;
            }
        }

        LinkedList<Tip> _tips = new LinkedList<Tip>();
        Pool<GameObject> _tipItemPool = new Pool<GameObject>();

        Canvas _canvas;
        RectTransform _tipRect;

        private void Start()
        {
            _canvas = UGUICreator.CreateCanvas(gameObject, 1920, 1080, RenderMode.ScreenSpaceCamera, Camera.main);
            _tipRect = UGUICreator.CreateNode(_canvas.GetComponent<RectTransform>(), 1920 / 4, 1080);

            var layout = _tipRect.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.spacing = 20;

            var item = UGUICreator.CreateImage();
            item.color = new Color(0, 0, 0, 0.5f);
            var itemLayout = item.gameObject.AddComponent<HorizontalLayoutGroup>();
            itemLayout.padding = new RectOffset(20, 20, 20, 20);
            itemLayout.childAlignment = TextAnchor.MiddleCenter;
            itemLayout.childControlWidth = true;
            itemLayout.childControlHeight = true;
            itemLayout.childForceExpandWidth = true;
            itemLayout.childForceExpandHeight = true;
            var fitter = item.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var itemTxt = UGUICreator.CreateText();
            itemTxt.fontSize = 30;
            itemTxt.transform.SetParent(item.transform, false);
            itemTxt.transform.localScale = Vector3.one;
            itemTxt.transform.localEulerAngles = Vector3.zero;
            itemTxt.transform.localPosition = Vector3.zero;

            var allocator = new GameObjectAllocator();
            allocator.CacheRoot.transform.SetParent(transform);
            allocator.SetTemplate(item.gameObject);
            _tipItemPool.SetAllocator(allocator);
        }

        bool _hasChange = false;
        public void AddTip(string msg, float life = 3)
        {
            _tips.AddLast(new Tip() { Msg = msg, Life = life });
            _hasChange = true;
        }

        private void Update()
        {
            var p = _tips.First;
            while (p != null)
            {
                var n = p.Next;
                if (p.Value.IsExpired())
                {
                    _tips.Remove(p);
                    _hasChange = true;
                }

                p = n;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay && _canvas.worldCamera == null)
                _canvas.worldCamera = Camera.main;

            if (!_hasChange)
                return;

            _hasChange = false;
            for (int i = _tipRect.childCount-1; i >=0; i--)
            {
                _tipItemPool.Recycle(_tipRect.GetChild(i).gameObject);
            }

            foreach (var tip in _tips)
            {
                var v = _tipItemPool.Spawn();
                v.transform.SetParent(_tipRect);
                v.transform.localScale = Vector3.one;
                v.transform.localEulerAngles = Vector3.zero;
                v.transform.localPosition = Vector3.zero;
                var txt = v.GetComponentInChildren<Text>();
                txt.text = tip.Msg;
                txt.fontSize = _fontSize;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_tipRect);
        }
    }
}
