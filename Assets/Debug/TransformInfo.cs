#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class TransformInfo : MonoBehaviour
    {
        public bool UseFixedUpdate = true;
        [Space()]
        public float WorldX;
        public float WorldY;
        public float WorldZ;
        public float LocalX, LocalY, LocalZ;
        [Space()]
        public float WorldRX;
        public float WorldRY;
        public float WorldRZ;
        public float LocalRX, LocalRY, LocalRZ;
        [Space()]
        public float WorldSX;
        public float WorldSY;
        public float WorldSZ;
        public float LocalSX, LocalSY, LocalSZ;

        private Transform _tran;

        private void Awake()
        {
            _tran = transform;
            SyncInfo();
        }

        private void Update()
        {
            if (!UseFixedUpdate)
            {
                SyncInfo();
            }
        }

        private void FixedUpdate()
        {
            if (UseFixedUpdate)
            {
                SyncInfo();
            }
        }

        private void SyncInfo()
        {
            WorldX = _tran.position.x;
            WorldY = _tran.position.y;
            WorldZ = _tran.position.z;

            LocalX = _tran.localPosition.x;
            LocalY = _tran.localPosition.y;
            LocalZ = _tran.localPosition.z;

            WorldRX = _tran.eulerAngles.x;
            WorldRY = _tran.eulerAngles.y;
            WorldRZ = _tran.eulerAngles.z;

            LocalRX = _tran.localEulerAngles.x;
            LocalRY = _tran.localEulerAngles.y;
            LocalRZ = _tran.localEulerAngles.z;

            WorldSX = _tran.lossyScale.x;
            WorldSY = _tran.lossyScale.y;
            WorldSZ = _tran.lossyScale.z;

            LocalSX = _tran.localScale.x;
            LocalSY = _tran.localScale.y;
            LocalSZ = _tran.localScale.z;
        }
    }
}
#endif
