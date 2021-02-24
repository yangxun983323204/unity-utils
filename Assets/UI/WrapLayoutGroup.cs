using UnityEngine;
using UnityEngine.UI;

namespace YX
{
    [AddComponentMenu("Layout/Wrap Layout Group", 153)]
    public class WrapLayoutGroup : LayoutGroup
    {
        private Vector2[] childPos = new Vector2[0];
        private Bounds _childrenBounds;

        protected WrapLayoutGroup()
        { }

        public override void CalculateLayoutInputVertical()
        {
            SetLayoutInputForAxis(_childrenBounds.size.y, _childrenBounds.size.y, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            CalcRequiredSize();
            SetLayoutInputForAxis(_childrenBounds.size.x, _childrenBounds.size.x, -1, 0);
        }

        public override void SetLayoutVertical()
        {
            SetChildren();
        }

        private void SetChildren()
        {
            Vector3 offset = _childrenBounds.center;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var c = rectChildren[i];
                c.transform.localPosition -= offset;
            }
        }

        private void CalcRequiredSize()
        {
            _childrenBounds = default(Bounds);
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var c = rectChildren[i];
                if (i == 0)
                    _childrenBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform, c);
                else
                    _childrenBounds.Encapsulate(RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform, c));
            }
        }
    }
}
