using System.Collections;
using System.Collections.Generic;

namespace YX
{
    public class SwapChain<T>
    {
        private List<T> _array;
        private int _frontIdx = -1;

        public int BackCnt {
            get {
                var v = _array.Count - 1;
                return v < 0 ? 0 : v;
            }
        }

        public SwapChain(int capacity = 2)
        {
            _array = new List<T>(capacity);
        }

        public void AddFront(T v)
        {
            _array.Add(v);
            _frontIdx = _array.Count - 1;
        }

        public void AddBack(T v)
        {
            _array.Add(v);
        }

        public void Swap()
        {
            var i = _frontIdx + 1;
            _frontIdx = ClampIdx(i);
        }

        public T GetFront()
        {
            if(_frontIdx<0)
                throw new System.IndexOutOfRangeException();

            return _array[_frontIdx];
        }

        public T GetBack(int idx=0)
        {
            if (idx >= BackCnt)
                throw new System.IndexOutOfRangeException();

            var i = _frontIdx + 1 + idx;
            i = ClampIdx(i);
            return _array[i];
        }

        public void Clear()
        {
            _array.Clear();
            _frontIdx = -1;
        }

        private int ClampIdx(int i)
        {
            int cnt = _array.Count;
            if (i >= cnt)
                return i % cnt;
            else if (i < 0)
                return cnt + i % cnt;
            else
                return i;
        }
    }
}
