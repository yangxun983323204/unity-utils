using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class ArrayEx
    {
        public static void Insert<T>(this T[] array, int len, T val, int idx)
        {
            if (idx + 1 < len)
                Array.Copy(array, idx, array, idx + 1, len - idx - 1);
            array[idx] = val;
        }

        public static void SortedAdd<T>(this T[] array, int start, int end, T val, Func<T, bool> condition)
        {
            for (int i = start; i < end; i++)
            {
                if (condition(array[i]))
                {
                    array.Insert(end, val, i);
                    return;
                }
            }

            array.Insert(end, val, end);
        }

        public static void SortedReplace<T>(this T[] array, int start, int end, T val, Func<T, bool> condition)
        {
            for (int i = start; i < end; i++)
            {
                if (condition(array[i]))
                {
                    array[i] = val;
                    return;
                }
            }
        }
    }
}
