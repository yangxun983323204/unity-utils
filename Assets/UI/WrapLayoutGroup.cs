using UnityEngine;
using UnityEngine.UI;

namespace YX
{
    [AddComponentMenu("Layout/Wrap Layout Group", 153)]
    public class WrapLayoutGroup : LayoutGroup
    {
        public bool RecursiveChildren = true;
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
            _childrenBounds = default;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var c = rectChildren[i];
                var b = CalcBounds(rectTransform, c, RecursiveChildren);
                if (i == 0)
                    _childrenBounds = b;
                else
                    _childrenBounds.Encapsulate(b);
            }
        }

        private Bounds CalcBounds(RectTransform self,RectTransform tar,bool recursive)
        {
            Bounds bounds = default;
            if (recursive)
            {
                var trans = tar.GetComponentsInChildren<RectTransform>();
                for (int i = 0; i < trans.Length; i++)
                {
                    var t = trans[i];
                    var b = RectTransformUtility.CalculateRelativeRectTransformBounds(self, t);
                    if (i == 0)
                        bounds = b;
                    else
                        bounds.Encapsulate(b);
                }
            }
            else
            {
                bounds = GetRectTransformSelfBounds(self, tar);
            }

            return bounds;
        }

        private static readonly Vector3[] s_Corners = new Vector3[4];

        private Bounds GetRectTransformSelfBounds(Transform root,Transform child)
        {
            Bounds result;
            RectTransform c = child as RectTransform;

            Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;

            c.GetWorldCorners(s_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[j]);
                vector = Vector3.Min(lhs, vector);
                vector2 = Vector3.Max(lhs, vector2);
            }

            Bounds bounds = new Bounds(vector, Vector3.zero);
            bounds.Encapsulate(vector2);
            result = bounds;

            return result;
        }
    }
}
