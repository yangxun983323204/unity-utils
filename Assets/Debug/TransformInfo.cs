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

        public Transform TargetTran;

        private void Awake()
        {
            TargetTran = transform;
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
            WorldX = TargetTran.position.x;
            WorldY = TargetTran.position.y;
            WorldZ = TargetTran.position.z;

            LocalX = TargetTran.localPosition.x;
            LocalY = TargetTran.localPosition.y;
            LocalZ = TargetTran.localPosition.z;

            WorldRX = TargetTran.eulerAngles.x;
            WorldRY = TargetTran.eulerAngles.y;
            WorldRZ = TargetTran.eulerAngles.z;

            LocalRX = TargetTran.localEulerAngles.x;
            LocalRY = TargetTran.localEulerAngles.y;
            LocalRZ = TargetTran.localEulerAngles.z;

            WorldSX = TargetTran.lossyScale.x;
            WorldSY = TargetTran.lossyScale.y;
            WorldSZ = TargetTran.lossyScale.z;

            LocalSX = TargetTran.localScale.x;
            LocalSY = TargetTran.localScale.y;
            LocalSZ = TargetTran.localScale.z;
        }
    }
}
#endif
