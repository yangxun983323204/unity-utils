using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace YX
{
    public class GraphicDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform Limit { get { return _limit; } set { _limit = value; } }
        [SerializeField]
        [FormerlySerializedAs("Limit")]
        RectTransform _limit;

        RectTransform _rectTran;
        Graphic _graphic;

        void Start()
        {
            _rectTran = transform as RectTransform;
            _graphic = GetComponent<Graphic>();
            if (_graphic == null)
            {
                Debug.LogError("GraphicDrag组件需要UnityEngine.UI.Graphic组件或其子类");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _debt = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var delta = eventData.delta;
            Screen2Canvas(ref delta);
            PosClamp(ref delta);
            _rectTran.anchoredPosition += (delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _debt = Vector2.zero;
        }
        /// <summary>
        /// 债务,该值代表在x，y方向上有多少应该移动而未移动的量
        /// </summary>
        Vector2 _debt = Vector2.zero;// 当Rect靠边后继续加深移动将产生债务，移动增量优先用于偿还债务
        void PosClamp(ref Vector2 delta)
        {
            if (_limit == null)
                return;
            // repay a debt 
            int n = 0;
            var dirx = _debt.x * delta.x;
            var diry = _debt.y * delta.y;

            if (dirx > 0)
            {
                n++;
                _debt.x += delta.x;
                delta.x = 0;
            }
            else if (dirx < 0)
            {
                var x = _debt.x + delta.x;
                if (_debt.x * x > 0)
                {
                    _debt.x = x;
                    delta.x = 0;
                }
                else
                {
                    _debt.x = 0;
                    delta.x = x;
                }
            }

            if (diry>0)
            {
                n++;
                _debt.y += delta.y;
                delta.y = 0;
            }
            else if (diry < 0)
            {
                var y = _debt.y + delta.y;
                if (_debt.y * y > 0)
                {
                    _debt.y = y;
                    delta.y = 0;
                }
                else
                {
                    _debt.y = 0;
                    delta.y = y;
                }
            }

            if (n==2)// 两个方向都靠边了，且移动还在向靠边的方向加深
            {
                return;
            }
            //

            var rawDelta = delta;
            var limit2w = _limit.localToWorldMatrix;
            var w2self = _rectTran.worldToLocalMatrix;

            var tRect = _limit.rect;
            var tMin = new Vector2(tRect.xMin, tRect.yMin);
            var tMax = new Vector2(tRect.xMax, tRect.yMax);
            var wMin = limit2w.MultiplyPoint(tMin);
            var wMax = limit2w.MultiplyPoint(tMax);
            var min = w2self.MultiplyPoint(wMin);
            var max = w2self.MultiplyPoint(wMax);

            var rect = _rectTran.rect;

            if (delta.x > 0)
            {
                var d = max.x - rect.xMax;
                delta.x = Mathf.Min(d, delta.x);
            }
            else if(delta.x < 0)
            {
                var d = min.x - rect.xMin;
                delta.x = Mathf.Max(d, delta.x);
            }

            if (delta.y > 0)
            {
                var d = max.y - rect.yMax;
                delta.y = Mathf.Min(d, delta.y);
            }
            else if (delta.y < 0)
            {
                var d = min.y - rect.yMin;
                delta.y = Mathf.Max(d, delta.y);
            }

            _debt -= (delta - rawDelta);
        }

        void Screen2Canvas(ref Vector2 delta)
        {
            var canvsRect = (_graphic.canvas.transform as RectTransform).rect;
            var cx = (float)canvsRect.width / Screen.width;
            var cy = (float)canvsRect.height / Screen.height;

            var lx = _rectTran.lossyScale.x / _graphic.canvas.transform.lossyScale.x;
            var ly = _rectTran.lossyScale.y / _graphic.canvas.transform.lossyScale.y;

            var sx = cx / lx * _rectTran.localScale.x;
            var sy = cy / ly * _rectTran.localScale.y;

            delta.x = delta.x * sx;
            delta.y = delta.y * sy;
        }
    }
}
