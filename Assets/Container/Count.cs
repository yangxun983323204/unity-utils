using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class Count<TKey>
    {
        public event Action<TKey> onNew, onDelete;

        private Dictionary<TKey, int> _dict;

        public Count(int capacity)
        {
            _dict = new Dictionary<TKey, int>(capacity);
        }

        public int Add(TKey key)
        {
            int val = 0;
            _dict.TryGetValue(key, out val);
            if (val <= 0)
                onNew?.Invoke(key);

            val++;
            _dict[key] = val;
            return val;
        }

        public int Release(TKey key)
        {
            int val = 0;
            if (_dict.TryGetValue(key,out val))
            {
                if (val > 0)
                    onDelete?.Invoke(key);

                val--;
                if (val < 0)
                    val = 0;

                _dict[key] = val;
            }

            return val;
        }

        public int GetCount(TKey key)
        {
            int val = 0;
            _dict.TryGetValue(key, out val);
            return val;
        }
    }
}
