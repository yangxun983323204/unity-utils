using System.Collections;
using System.Collections.Generic;

namespace YX
{
    /// <summary>
    /// 上三角矩阵
    /// </summary>
    public class UTMatrix<T>
    {
        ListAccessor<T> _data;
        int _n;
        int _max;
        /// <summary>
        /// 构造一个n*n的上三角矩阵
        /// </summary>
        /// <param name="n"></param>
        public UTMatrix(int n = 2)
        {
            _n = n;
            int size = GetBufferSize();
            _data = new ListAccessor<T>(size);
            _data.Reserve(size);
        }
        /// <summary>
        /// 获取i行j列的值，只能取上三角部分
        /// </summary>
        public T Get(int i, int j)
        {
            return GetRaw(GetBufferIdx(i, j));
        }
        /// <summary>
        /// 设置i行j列的值，只能设置上三角部分
        /// </summary>
        public void Set(int i, int j, T val)
        {
            SetRaw(GetBufferIdx(i, j), val);
        }
        /// <summary>
        /// 把矩阵重置为n*n，不清除已有数据
        /// </summary>
        public void Resize(int n)
        {
            _n = n;
            int size = GetBufferSize();
            _data.Reserve(size);
        }
        /// <summary>
        /// 把整个矩阵设为val
        /// </summary>
        public void Reset(T val)
        {
            SetRawRange(0, GetBufferSize(), val);
        }

        #region 更多控制
        public int GetBufferSize()
        {
            _max = GetBufferSize(_n);
            return _max;
        }

        public static int GetBufferSize(int n)
        {
            return  n * (n + 1) / 2;
        }

        public int GetBufferIdx(int i,int j)
        {
            if (i > j)
                throw new System.NotSupportedException("上三角阵不支持对下三角访问!");

            // n*n矩阵
            // 只说i<=j的情况
            // i<=0时，S(i) = 0
            // i>0时，S(i) = i*n - i*(i-1)/2
            // K(i,j) = S(i) + (j-i)
            int n = _n;
            int Si = 0;
            if (i <= 0)
                Si = 0;
            else
                Si = i * n - (int)(i * (i - 1) / 2f);

            return Si + (j - i);
        }

        public T GetRaw(int idx)
        {
            if (idx >= _max)
                throw new System.IndexOutOfRangeException();

            return _data[idx];
        }

        public void SetRaw(int idx,T val)
        {
            if (idx >= _max)
                throw new System.IndexOutOfRangeException();

            _data[idx] = val;
        }
        /// <summary>
        /// 把[start,end)范围内的值设为val
        /// </summary>
        public void SetRawRange(int start,int end,T val)
        {
            if (start >= _max)
                throw new System.IndexOutOfRangeException();
            if (end > _max)
                throw new System.IndexOutOfRangeException();

            _data.SetRange(start, end, val);
        }
        #endregion
    }
}
