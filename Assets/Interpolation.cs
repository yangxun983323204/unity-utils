using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class Interpolation
    {
        /// <summary>
        /// 凹曲线插值
        /// </summary>
        public static float Concave(float from, float to, float t)
        {
            float t1 = Mathf.Sqrt(t);
            return Mathf.Lerp(from, to, t1);
        }

        /// <summary>
        /// 凸曲线插值
        /// </summary>
        public static float Raise(float from, float to, float t)
        {
            float t1 = Mathf.Pow(t, 2);
            return Mathf.Lerp(from, to, t1);
        }
    }
}
