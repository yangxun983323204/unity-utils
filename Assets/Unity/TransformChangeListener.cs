using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace YX {
    public class TransformChangeListener : MonoBehaviour
    {
        public static event UnityAction<Transform> onChange;

        public static TransformChangeListener Attach(Transform to)
        {
            if (to == null)
                return null;

            var i = to.gameObject.GetComponent<TransformChangeListener>();
            if (i == null)
                i = to.gameObject.AddComponent<TransformChangeListener>();

            return i;
        }

        private Transform _tran;

        void Awake()
        {
            _tran = transform;
        }

        private void LateUpdate()
        {
            if (_tran.hasChanged)
            {
                _tran.hasChanged = false;
                onChange?.Invoke(_tran);
            }
        }
    }
}
