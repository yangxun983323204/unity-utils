using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class StyleBuilder
    {
        public static void Fill(GameObject obj, Vector2 min, Vector2 max)
        {
            if (obj == null || !(obj.transform.parent is RectTransform))
                return;

            var parent = obj.transform.parent as RectTransform;
            var rectTransform = obj.GetComponent<RectTransform>();

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector3.zero;
        }

        public static void Size(GameObject obj, float w, float h)
        {
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector3(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector3(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector3(w, h);
        }

        public static Color ColorInvert(Color c)
        {
            c.r = 1 - c.r;
            c.g = 1 - c.g;
            c.b = 1 - c.b;
            return c;
        }

        public static void ColorInvert(GameObject root)
        {
            var hashset = new HashSet<Graphic>();
            var selectables = root.GetComponentsInChildren<Selectable>(true);
            foreach (var s in selectables)
            {
                var cols = s.colors;
                cols.disabledColor = ColorInvert(cols.disabledColor);
                cols.pressedColor = ColorInvert(cols.pressedColor);
                cols.normalColor = ColorInvert(cols.normalColor);
                cols.selectedColor = ColorInvert(cols.selectedColor);
                cols.highlightedColor = ColorInvert(cols.highlightedColor);
                s.colors = cols;
                BlackFix(s);
                if (s.targetGraphic != null && !hashset.Contains(s.targetGraphic))
                    hashset.Add(s.targetGraphic);
            }

            var graphics = root.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                if (hashset.Contains(g))
                    continue;

                g.color = ColorInvert(g.color);
            }
        }

        public static void BlackFix(Selectable selectable)
        {
            if (selectable.colors.normalColor == Color.black)
            {
                var cols = selectable.colors;
                if (selectable is Scrollbar)
                    cols.normalColor = cols.pressedColor;

                cols.highlightedColor = cols.pressedColor;
                cols.selectedColor = cols.pressedColor;
                selectable.colors = cols;
            }
        }

        public class AnchorSequenceBuilder
        {
            List<RectTransform> _rectTransforms = new List<RectTransform>(5);
            List<int> _weights = new List<int>();

            public AnchorSequenceBuilder Add(RectTransform rectTransform, int weight)
            {
                _rectTransforms.Add(rectTransform);
                _weights.Add(weight);
                return this;
            }

            public AnchorSequenceBuilder Add(GameObject go, int weight)
            {
                _rectTransforms.Add(go.GetComponent<RectTransform>());
                _weights.Add(weight);
                return this;
            }

            public AnchorSequenceBuilder AddSpan(int weight)
            {
                _rectTransforms.Add(null);
                _weights.Add(weight);
                return this;
            }

            public void Clear()
            {
                _rectTransforms.Clear();
                _weights.Clear();
            }

            public void BuildHorizontal()
            {
                int sum = 0;
                foreach (var i in _weights)
                {
                    sum += i;
                }

                float _curr = 0;
                for (int i = 0; i < _weights.Count; i++)
                {
                    var w = _weights[i];
                    var t = _rectTransforms[i];
                    var n = _curr + (float)w / sum;
                    if (t!=null)
                        Fill(t.gameObject, new Vector2(_curr, 0), new Vector2(n, 1));

                    _curr = n;
                }
            }

            public void BuildVertical()
            {
                int sum = 0;
                foreach (var i in _weights)
                {
                    sum += i;
                }

                float _curr = 0;
                for (int i = 0; i < _weights.Count; i++)
                {
                    var w = _weights[i];
                    var t = _rectTransforms[i];
                    var n = _curr + (float)w / sum;
                    if (t != null)
                        Fill(t.gameObject, new Vector2(0, _curr), new Vector2(1, n));

                    _curr = n;
                }
            }
        }
    }
}
