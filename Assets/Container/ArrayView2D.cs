using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class ArrayView2D<T>
    {
        T[] _array = null;
        int _offset;
        int _rCnt;
        int _cCnt;

        public void Bind(T[] array, int rowCnt,int colCnt, int offset = 0)
        {
            System.Diagnostics.Debug.Assert(array != null && array.Length - offset >= rowCnt * colCnt);
            _array = array;
            _offset = offset;
            _rCnt = rowCnt;
            _cCnt = colCnt;
        }

        public int Idx(int row, int col)
        {
            return _offset + row * _cCnt + col;
        }

        public T Get(int row,int col)
        {
            return _array[_offset + row * _cCnt + col];
        }

        public void Set(int row,int col,T v)
        {
            _array[_offset + row * _cCnt + col] = v;
        }
    }
}
