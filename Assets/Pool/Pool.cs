using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YX
{
    public class Pool<T>:IDisposable
    {
        public uint MaxCache { get; set; }
        public int Count
        {
            get { return _list.Count; }
        }

        private T _template;
        private IObjectAllocator<T> _allocator;
        private LinkedList<T> _list = new LinkedList<T>();

        public Pool()
        {
            MaxCache = 20;
        }

        public void SetTemplate(T template, IObjectAllocator<T> allocator)
        {
            Clear();
            if (_template != null)
                _allocator.Destroy(_template);

            _template = template;
            _allocator = allocator;
            _allocator.SetTemplate(_template);
        }

        public void Reserve(uint count)
        {
            var needClone = count - _list.Count;
            if (needClone > 0)
            {
                if (MaxCache < count)
                    MaxCache = count;

                for (int i = 0; i < needClone; i++)
                {
                    var inst = _allocator.Clone();
                    _allocator.OnClone(inst);
                    _list.AddLast(inst);
                }
            }
        }

        public T Spawn()
        {
            T inst;
            if (_list.Count <= 1)
                Reserve(4);

            inst = _list.Last.Value;
            _list.RemoveLast();
            _allocator.OnSpawn(inst);
            return inst;
        }

        public void Recycle(T obj)
        {
            if (_list.Count > (uint)(MaxCache * 1.5f))
            {
                _allocator.Destroy(obj);
                //
                var reduceCnt = (uint)(MaxCache * 0.5f);
                for (int i = 0; i < reduceCnt; i++)
                {
                    var item = _list.Last.Value;
                    _list.RemoveLast();
                    _allocator.Destroy(item);
                }
            }
            else
            {
                _allocator.OnRecycle(obj);
                _list.AddLast(obj);
            }
        }

        public void Clear()
        {
            foreach (var item in _list)
            {
                _allocator.Destroy(item);
            }
            _list.Clear();
        }

        public void Dispose()
        {
            _allocator.Destroy(_template);
            _allocator.Dispose();
        }
    }
}
