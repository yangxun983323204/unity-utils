using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class LockCount
    {
        public Action onLock, onUnlock;

        public int Count { get { return _lockCnt; } }
        private int _lockCnt = 0;
        private uint _genId = 0;
        private HashSet<uint> _lockIds = new HashSet<uint>();

        public bool IsLocked()
        {
            return _lockCnt > 0;
        }

        public uint Lock()
        {
            var i = _genId;
            _genId++;
            _lockCnt++;
            _lockIds.Add(i);
            if (_lockCnt == 1)
                onLock?.Invoke();

            return i;
        }

        public bool Unlock(uint id)
        {
            if (_lockIds.Contains(id))
            {
                _lockIds.Remove(id);
                _lockCnt--;
                if (_lockCnt == 0)
                    onUnlock?.Invoke();

                return true;
            }
            return false;
        }
    }
}
