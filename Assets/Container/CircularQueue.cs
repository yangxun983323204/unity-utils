using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YX
{
    public class CircularQueue<T>
    {
        public int Count { get; private set; }

        private T[] _data;
        private int _idxHead;
        private int _idxTail;
        private int _size;

        public CircularQueue(int size)
        {
            _data = new T[size];
            _idxHead = _idxTail = -1;
            _size = size;
            Count = 0;
        }

        public bool IsFull()
        {
            return (_idxTail + 1) % _size == _idxHead;
        }

        public bool IsEmpty()
        {
            return _idxHead == -1;
        }

        public T Enqueue(T val)
        {
            if (IsFull())
                throw new OutOfMemoryException();

            if (IsEmpty())
                _idxHead = 0;

            _idxTail = (_idxTail + 1) % _size;
            _data[_idxTail] = val;
            Count++;
            return val;
        }

        public T Dequeue()
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException();

            T val = _data[_idxHead];
            if (_idxHead == _idxTail)
                _idxHead = _idxTail = -1;
            else
                _idxHead = (_idxHead + 1) % _size;

            Count--;
            return val;
        }

        public T Frist()
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException();

            return _data[_idxHead];
        }

        public T Last()
        {
            if (IsEmpty())
                throw new IndexOutOfRangeException();

            return _data[_idxTail];
        }

        public bool Contains(Func<T, bool> func)
        {
            int i = _idxHead;
            while (true)
            {
                int idx = i % _size;
                if (func(_data[idx]))
                    return true;

                if (idx == _idxTail)
                    break;

                i = idx;
                i++;
            }

            return false;
        }

        public int FindIndex(Func<T, bool> func)
        {
            if (IsEmpty())
                return -1;

            int i = _idxHead;
            while (true)
            {
                int idx = i % _size;
                if (func(_data[idx]))
                    return (idx + _size - _idxHead) % _size;

                if (idx == _idxTail)
                    break;

                i = idx;
                i++;
            }

            return -1;
        }

        public void Clear()
        {
            _idxHead = _idxTail = -1;
            Count = 0;
        }
    }
}
