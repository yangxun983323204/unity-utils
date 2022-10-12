using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YX
{
    public class RectTransformCollider : UIBehaviour
    {
        public BoxCollider Collider;
        public float Z;

        private RectTransform _rectTran { get { return transform as RectTransform; } }

        public void Init()
        {
            if (Collider == null)
            {
                Collider = gameObject.GetComponent<BoxCollider>();
                if (Collider == null)
                    Collider = gameObject.AddComponent<BoxCollider>();

                UpdateColliderSize();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateColliderSize();
        }

        private void UpdateColliderSize()
        {
            if (Collider == null)
                return;

            var rect = _rectTran.rect;
            Collider.size = new Vector3(rect.width, rect.height, Z);
            var cx = rect.width / 2f - _rectTran.pivot.x * rect.width;
            var cy = rect.height / 2f - _rectTran.pivot.y * rect.height;
            Collider.center = new Vector3(cx, cy, 0);
        }
    }
}