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
        public static float Concave(float from, float to, float t,float intensity = 2)
        {
            float t1 = Mathf.Pow(t, intensity);
            return Mathf.Lerp(from, to, t1);
        }

        /// <summary>
        /// 凸曲线插值
        /// </summary>
        public static float Raise(float from, float to, float t, float intensity = 2)
        {
            float t1 = 1 - Mathf.Pow(t, intensity);
            return Mathf.Lerp(from, to, t1);
        }
    }
}
