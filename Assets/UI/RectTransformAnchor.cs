using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YX
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformAnchor : MonoBehaviour
    {
        public enum RectAnchorH
        {
            None,
            Left,
            Right
        }

        public enum RectAnchorV
        {
            None,
            Top,
            Bottom
        }

        public enum UnitType
        {
            Self,
            Other,
            World
        }

        [Serializable]
        public struct EdgeH
        {
            public RectTransform Ref;
            public RectAnchorH Anchor;
            public UnitType OffsetUnit;
            public float Offset;
            public bool Dirty { get; private set; }
            public void SetDirty(bool d) { Dirty = d; }
            public void SetIfDirty(bool d)
            {
                if (d)
                    Dirty = d;
            }
        }

        [Serializable]
        public struct EdgeV
        {
            public RectTransform Ref;
            public RectAnchorV Anchor;
            public UnitType OffsetUnit;
            public float Offset;
            public bool Dirty { get; private set; }
            public void SetDirty(bool d) { Dirty = d; }
            public void SetIfDirty(bool d)
            {
                if (d)
                    Dirty = d;
            }
        }

        [SerializeField] EdgeH _leftRef;
        [SerializeField] EdgeH _rightRef;
        [SerializeField] EdgeV _topRef;
        [SerializeField] EdgeV _bottomRef;

        RectTransform _rectTran;
        public RectTransform RectTran
        {
            get
            {
                if (_rectTran == null)
                    _rectTran = transform as RectTransform;

                return _rectTran;
            }
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                RectTransformChangeListener.Attach(RectTran);
                RectTransformChangeListener.Attach(_leftRef.Ref);
                RectTransformChangeListener.Attach(_rightRef.Ref);
                RectTransformChangeListener.Attach(_topRef.Ref);
                RectTransformChangeListener.Attach(_bottomRef.Ref);

                _leftRef.SetDirty(true);
                _rightRef.SetDirty(true);
                _topRef.SetDirty(true);
                _bottomRef.SetDirty(true);
                RectTransformChangeListener.onChange += OnHasRectTransformChange;
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                RectTransformChangeListener.onChange -= OnHasRectTransformChange;
            }
        }

        public void LateUpdate()
        {
            if (Application.isPlaying)
                return;

            UpdateRect();
        }

        void UpdateRect()
        {
            if (Application.isPlaying &&
                !_leftRef.Dirty && !_rightRef.Dirty && !_topRef.Dirty && !_bottomRef.Dirty)
            {
                return;
            }

            ApplyTarNewWorldRect(RectTran, CalcTarNewWorldRect());
            if (Application.isPlaying)
            {
                _leftRef.SetDirty(false);
                _rightRef.SetDirty(false);
                _topRef.SetDirty(false);
                _bottomRef.SetDirty(false);
            }
        }

        public static Rect GetWorldRect(RectTransform tran)
        {
            var rect = tran.rect;
            var min = new Vector2(rect.xMin, rect.yMin);
            var max = new Vector2(rect.xMax, rect.yMax);

            var matrix = tran.localToWorldMatrix;
            var wMin = matrix.MultiplyPoint(min);
            var wMax = matrix.MultiplyPoint(max);

            return new Rect(wMin.x, wMin.y, wMax.x - wMin.x, wMax.y - wMin.y);
        }

        public static Rect GetLocalRect(RectTransform tran, Rect wRect)
        {
            var wMin = new Vector2(wRect.xMin, wRect.yMin);
            var wMax = new Vector2(wRect.xMax, wRect.yMax);

            var matrix = tran.worldToLocalMatrix;
            var min = matrix.MultiplyPoint(wMin);
            var max = matrix.MultiplyPoint(wMax);

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        public static void ApplyTarNewWorldRect(RectTransform tar, Rect wRect)
        {
            var lRect = GetLocalRect(tar.parent as RectTransform, wRect);
            var pivot = tar.pivot;
            float xOffset = pivot.x - 0.5f;
            float yOffset = pivot.y - 0.5f;
            lRect.Set(lRect.x + xOffset * lRect.width, lRect.y + yOffset * lRect.height, lRect.width, lRect.height);
            //
            var lPos = lRect.center;
            var lSize = lRect.size;

            tar.localPosition = lPos;
            tar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lSize.x / tar.localScale.x);
            tar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lSize.y / tar.localScale.y);
            tar.ForceUpdateRectTransforms();
        }

        public void ForceUpdate()
        {
            ApplyTarNewWorldRect(RectTran, CalcTarNewWorldRect());
        }

        public void SetDirty()
        {
            _leftRef.SetDirty(true);
            _rightRef.SetDirty(true);
            _topRef.SetDirty(true);
            _bottomRef.SetDirty(true);
        }

        private Rect CalcTarNewWorldRect()
        {
            var tarRect = GetWorldRect(RectTran);
            var oldRect = tarRect;
            // 根据四条边的anchor计算新的world rect

            bool leftNeed = _leftRef.Anchor != RectAnchorH.None && _leftRef.Ref != null;
            bool rightNeed = _rightRef.Anchor != RectAnchorH.None && _rightRef.Ref != null;

            if (leftNeed || rightNeed)
            {
                bool hStretch = leftNeed && rightNeed;
                // left
                if (leftNeed)
                    tarRect.xMin = GetWBaseH(_leftRef);
                // right
                if (rightNeed)
                    tarRect.xMax = GetWBaseH(_rightRef);

                if (!hStretch)
                {
                    if (!leftNeed)
                        tarRect.xMin = tarRect.xMax - oldRect.width;
                    else
                        tarRect.xMax = tarRect.xMin + oldRect.width;
                }
            }


            bool topNeed = _topRef.Anchor != RectAnchorV.None && _topRef.Ref != null;
            bool bottomNeed = _bottomRef.Anchor != RectAnchorV.None && _bottomRef.Ref != null;

            if (topNeed || bottomNeed)
            {
                bool vStretch = topNeed && bottomNeed;
                // top
                if (topNeed)
                    tarRect.yMax = GetWBaseV(_topRef);
                // bottom
                if (bottomNeed)
                    tarRect.yMin = GetWBaseV(_bottomRef);

                if (!vStretch)
                {
                    if (!topNeed)
                        tarRect.yMax = tarRect.yMin + oldRect.height;
                    else
                        tarRect.yMin = tarRect.yMax - oldRect.height;
                }
            }

            return tarRect;
        }

        private float GetWBaseH(EdgeH edge)
        {
            if (edge.Anchor == RectAnchorH.None)
                return GetWorldRect(edge.Ref).xMin + GetWorldOffset(edge);
            else if (edge.Anchor == RectAnchorH.Left)
                return GetWorldRect(edge.Ref).xMin + GetWorldOffset(edge);
            else
                return GetWorldRect(edge.Ref).xMax + GetWorldOffset(edge);
        }

        private float GetWBaseV(EdgeV edge)
        {
            if (edge.Anchor == RectAnchorV.None)
                return GetWorldRect(edge.Ref).yMin + GetWorldOffset(edge);
            else if (edge.Anchor == RectAnchorV.Bottom)
                return GetWorldRect(edge.Ref).yMin + GetWorldOffset(edge);
            else
                return GetWorldRect(edge.Ref).yMax + GetWorldOffset(edge);
        }

        private void OnHasRectTransformChange(RectTransform t)
        {
            if (t == RectTran)
            {
                _leftRef.SetDirty(true);
                _rightRef.SetDirty(true);
                _topRef.SetDirty(true);
                _bottomRef.SetDirty(true);
            }
            else
            {
                _leftRef.SetIfDirty(_leftRef.Ref == t);
                _rightRef.SetIfDirty(_rightRef.Ref == t);
                _topRef.SetIfDirty(_topRef.Ref == t);
                _bottomRef.SetIfDirty(_bottomRef.Ref == t);
            }

            UpdateRect();
        }

        private float GetWorldOffset(EdgeH edgeH)
        {
            if (edgeH.OffsetUnit == UnitType.World)
                return edgeH.Offset;
            else if (edgeH.OffsetUnit == UnitType.Self)
                return edgeH.Offset * _rectTran.lossyScale.x;
            else
                return edgeH.Offset * edgeH.Ref.lossyScale.x;
        }

        private float GetWorldOffset(EdgeV edgeV)
        {
            if (edgeV.OffsetUnit == UnitType.World)
                return edgeV.Offset;
            else if (edgeV.OffsetUnit == UnitType.Self)
                return edgeV.Offset * _rectTran.lossyScale.y;
            else
                return edgeV.Offset * edgeV.Ref.lossyScale.y;
        }
    }
}
