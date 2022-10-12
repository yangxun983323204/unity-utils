using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YX {
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformChangeListener : UIBehaviour
    {
        public static event UnityAction<RectTransform> onChange;

        public static RectTransformChangeListener Attach(RectTransform to)
        {
            if (to == null)
                return null;

            var i = to.gameObject.GetComponent<RectTransformChangeListener>();
            if (i == null)
                i = to.gameObject.AddComponent<RectTransformChangeListener>();

            return i;
        }

        private RectTransform _rectTran;

        protected override void OnRectTransformDimensionsChange()
        {
            onChange?.Invoke(_rectTran);
        }

        protected override void Awake()
        {
            _rectTran = transform as RectTransform;
        }

        private void LateUpdate()
        {
            if (_rectTran.hasChanged)
            {
                _rectTran.hasChanged = false;
                onChange?.Invoke(_rectTran);
            }
        }
    }
}
