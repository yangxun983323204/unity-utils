// 用来实现直接使用UI人员基于相对位置的标注
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YX
{
    public class RefPlaceTool:EditorWindow
    {
        [MenuItem("GameObject/RefPlace",priority = 0)]
        public static void Call()
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<RectTransform>()==null)
            {
                Debug.Log("必须选中场景中UGUI的对象");
                return;
            }

            var wnd = GetWindow<RefPlaceTool>("RefPlaceTool");
            wnd._tar = Selection.activeGameObject.GetComponent<RectTransform>();
            wnd._ref = null;
            wnd._keepPos = false;
            wnd._tarEdge = Edge.Left;
            wnd._refEdge0 = EdgeH.Right;
            wnd._refEdge1 = EdgeV.Bottom;
            wnd._val = 0;
            wnd.Show();
            wnd.minSize = new Vector2(400, 270);
            wnd.maxSize = new Vector2(400, 270);
        }

        public enum Edge
        {
            Left=0,Top,Right,Bottom
        }

        public enum EdgeH { Left=0,Right=2 }
        public enum EdgeV { Top=1,Bottom=3}

        private RectTransform _tar;
        private RectTransform _ref;
        private bool _keepPos;
        private Edge _tarEdge;
        private EdgeH _refEdge0;
        private EdgeV _refEdge1;
        private float _val;

        private Edge _refEdge;
        private RectTransform _layoutRoot;

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            _layoutRoot = (RectTransform)EditorGUILayout.ObjectField("根布局（非2D UI需要）", _layoutRoot, typeof(RectTransform), true);
            GUILayout.Space(20);
            _tar = (RectTransform)EditorGUILayout.ObjectField("目标", _tar, typeof(RectTransform),true);
            _tarEdge = (Edge)EditorGUILayout.EnumPopup("目标边:",_tarEdge);
            GUILayout.Space(20);
            _ref = (RectTransform)EditorGUILayout.ObjectField("参考", _ref, typeof(RectTransform), true);
            if (_tarEdge== Edge.Left || _tarEdge== Edge.Right)
                _refEdge0 = (EdgeH)EditorGUILayout.EnumPopup("参考边:", _refEdge0);
            else
                _refEdge1 = (EdgeV)EditorGUILayout.EnumPopup("参考边:", _refEdge1);

            GUILayout.Space(20);
            _keepPos = GUILayout.Toggle(_keepPos, "保持目标位置");
            GUILayout.Space(20);
            _val = EditorGUILayout.FloatField("相对值", _val);
            GUILayout.Space(20);
            GUILayout.EndVertical();
            if (_tar != null && _ref != null)
            {
                if (_layoutRoot == null && _tar.eulerAngles != Vector3.zero)
                    GUILayout.Label("！非2D UI，请设置共面的根布局，否则无法正确计算");
                else
                {
                    if (GUILayout.Button("应用"))
                        Apply();
                }
            }
            else
            {
                GUILayout.Label("！请设置好目标、参考、以及相对值");
            }
        }

        private void Apply()
        {
            if (_ref.IsChildOf(_tar))
            {
                EditorUtility.DisplayDialog("警告", "不支持父节点参考子节点", "好的");
                return;
            }

            if (_tarEdge == Edge.Left || _tarEdge == Edge.Right)
                _refEdge = (Edge)_refEdge0;
            else
                _refEdge = (Edge)_refEdge1;

            var tarNewWRect = CalcTarNewWorldRect();
            ApplyTarNewWorldRect(tarNewWRect);
        }

        private Rect GetWorldRect(RectTransform tran)
        {
            var rect = tran.rect;
            var min = new Vector2(rect.xMin, rect.yMin);
            var max = new Vector2(rect.xMax, rect.yMax);

            var matrix = tran.localToWorldMatrix;
            if (_layoutRoot!=null)
                matrix = _layoutRoot.worldToLocalMatrix * matrix;

            var wMin = matrix.MultiplyPoint(min);
            var wMax = matrix.MultiplyPoint(max);

            return new Rect(wMin.x, wMin.y, wMax.x - wMin.x, wMax.y - wMin.y);
        }

        private Rect GetLocalRect(RectTransform tran,Rect wRect)
        {
            var wMin = new Vector2(wRect.xMin, wRect.yMin);
            var wMax = new Vector2(wRect.xMax, wRect.yMax);

            var matrix = tran.worldToLocalMatrix;
            if (_layoutRoot != null)
                matrix = matrix * _layoutRoot.localToWorldMatrix;

            var min = matrix.MultiplyPoint(wMin);
            var max = matrix.MultiplyPoint(wMax);

            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        private Rect CalcTarNewWorldRect()
        {
            var tarRect = GetWorldRect(_tar);
            var refRect = GetWorldRect(_ref);
            float refVal;

            switch (_refEdge)
            {
                case Edge.Left:
                    refVal = refRect.xMin;
                    break;
                case Edge.Top:
                    refVal = refRect.yMax;
                    break;
                case Edge.Right:
                    refVal = refRect.xMax;
                    break;
                case Edge.Bottom:
                    refVal = refRect.yMin;
                    break;
                default:
                    throw new System.NotSupportedException();
            }

            refVal += _val;

            if (!_keepPos)
            {
                float x = tarRect.x;
                float y = tarRect.y;
                float w = tarRect.width;
                float h = tarRect.height;
                switch (_tarEdge)
                {
                    case Edge.Left:
                        return new Rect(refVal, y, w, h);
                    case Edge.Top:
                        return new Rect(x, refVal - h, w, h);
                    case Edge.Right:
                        return new Rect(refVal - w, y, w, h);
                    case Edge.Bottom:
                        return new Rect(x, refVal, w, h);
                    default:
                        throw new System.NotSupportedException();
                }
            }
            else
            {
                float x = tarRect.x;
                float y = tarRect.y;
                float w = tarRect.width;
                float h = tarRect.height;
                switch (_tarEdge)
                {
                    case Edge.Left:
                        w += x - refVal;
                        return new Rect(refVal, y, w, h);
                    case Edge.Top:
                        h += (refVal - h) - y;
                        return new Rect(x, refVal - h, w, h);
                    case Edge.Right:
                        w += refVal - (x + w);
                        return new Rect(refVal - w, y, w, h);
                    case Edge.Bottom:
                        h += y - refVal;
                        return new Rect(x, refVal, w, h);
                    default:
                        throw new System.NotSupportedException();
                }
            }
        }

        private void ApplyTarNewWorldRect(Rect wRect)
        {
            var lRect = GetLocalRect(_tar.parent as RectTransform, wRect);
            var pivot = _tar.pivot;
            float xOffset = pivot.x - 0.5f;
            float yOffset = pivot.y - 0.5f;
            lRect.Set(lRect.x + xOffset * lRect.width, lRect.y + yOffset * lRect.height, lRect.width, lRect.height);
            //
            var lPos = lRect.center;
            var lSize = lRect.size;

            _tar.localPosition = lPos;
            _tar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lSize.x / _tar.localScale.x);
            _tar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lSize.y / _tar.localScale.y);
        }
    }
}
