using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class ArrayView3D<T>
    {
        T[] _array = null;
        int _offset;
        int _xCnt;
        int _yCnt;
        int _zCnt;

        public void Bind(T[] array, int xCnt,int yCnt, int zCnt, int offset = 0)
        {
            System.Diagnostics.Debug.Assert(array != null && array.Length - offset >= xCnt * yCnt * zCnt);
            _array = array;
            _offset = offset;
            _xCnt = xCnt;
            _yCnt = yCnt;
            _zCnt = zCnt;
        }

        public int Idx(int x,int y,int z)
        {
            return _offset + x * _yCnt * _zCnt + y * _zCnt + z;
        }

        public T Get(int x,int y,int z)
        {
            return _array[_offset + x * _yCnt * _zCnt + y * _zCnt + z];
        }

        public void Set(int x,int y,int z, T v)
        {
            _array[_offset + x * _yCnt * _zCnt + y * _zCnt + z] = v;
        }
    }
}
