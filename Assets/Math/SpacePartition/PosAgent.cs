using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX.SpacePartition
{
    public class PosAgent : MonoBehaviour, IAgent
    {
        public event Action<IAgent> onMoved;

        private Transform _tran;

        void Awake()
        {
            _tran = transform;
        }

        public void GetPosition(ref Vector3 pos)
        {
            pos = _tran.position;
        }

        public void Reset()
        {
        }

        public void MoveTo(Vector3 pos)
        {
            _tran.position = pos;
            onMoved?.Invoke(this);
        }
    }
}
