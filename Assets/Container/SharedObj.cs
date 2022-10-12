using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class SharedObj<T>:IDisposable where T: class
    {
        internal class Ref<T1>
        {
            public T1 Value;
        }

        private Ref<T> _tar;
        public T Tar {
            get { return _tar.Value; }
            private set { _tar.Value = value; }
        }

        private Ref<int> _count;
        public int Count
        {
            get {
                if (_count == null) return 0;
                return _count.Value;
            }
        }
        private Action<T> _deleter;

        public void Set(T v, Action<T> deleter=null)
        {
            if (_tar == null)
                _tar = new Ref<T>();

            Dispose();
            Tar = v;
            _count = new Ref<int>() { Value = v == null ? 0 : 1 };
            _deleter = deleter;
        }

        public SharedObj<T> Share()
        {
            var n = new SharedObj<T>();
            if (_tar==null || _tar.Value==null)
            {
                return n;
            }

            n._tar = _tar;
            n._count = _count;
            n._deleter = _deleter;
            _count.Value++;
            return n;
        }

        ~SharedObj()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Tar != null)
            {
                _count.Value--;
                if (_count.Value <= 0)
                {
                    if (_deleter != null)
                        _deleter(Tar);

                    Tar = null;
                }
            }
            _deleter = null;
        }
    }
}
