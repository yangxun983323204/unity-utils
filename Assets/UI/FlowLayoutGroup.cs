using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YX
{
    [AddComponentMenu("Layout/Flow Layout Group", 154)]
    public class FlowLayoutGroup : LayoutGroup
    {
        public Vector2 Padding = Vector2.zero;
        public float RowHeight = 100;

        private float _width;
        private float _height;

        private float _currWidth;
        private float _currHeight;
        private Vector2 _offset;

        public override void CalculateLayoutInputVertical()
        {
            SetLayoutInputForAxis(rectTransform.rect.height, rectTransform.rect.height, -1, 1);
        }

        public override void CalculateLayoutInputHorizontal()
        {
            SetLayoutInputForAxis(rectTransform.rect.width, rectTransform.rect.width, -1, 0);
        }

        public override void SetLayoutHorizontal()
        {
            var rectTran = transform as RectTransform;
            var rect = rectTran.rect;
            _width = rect.width;
            _height = rect.height;
            _offset = new Vector2(-_width * rectTran.pivot.x, _height * (1 - rectTran.pivot.y));
            _currWidth = 0;
            _currHeight = 0;
        }

        public override void SetLayoutVertical()
        {
            foreach (RectTransform c in transform)
            {
                if (!c.gameObject.activeSelf)
                    continue;

                var w = c.rect.width;
                var h = c.rect.height;
                if (_currWidth!=0 && _currWidth + Padding.x + w > _width)
                {
                    _currWidth = 0;
                    _currHeight = _currHeight - Padding.y - RowHeight;
                }

                float x = _currWidth + c.pivot.x * w + _offset.x;
                float y = _currHeight - (1 - c.pivot.y) * h + _offset.y;
                c.localPosition = new Vector2(x, y);
                _currWidth += Padding.x * 2 + w;
            }
        }
    }
}
