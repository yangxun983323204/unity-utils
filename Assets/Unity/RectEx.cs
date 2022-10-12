using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class RectEx
    {

        public static void MoveXMin(ref this Rect rect, float v)
        {
            var w = rect.width;
            rect.xMin = v;
            rect.xMax = v + w;
        }

        public static Rect MoveXMin(Rect rect, float v)
        {
            var w = rect.width;
            rect.xMin = v;
            rect.xMax = v + w;
            return rect;
        }

        public static void MoveYMin(ref this Rect rect, float v)
        {
            var h = rect.height;
            rect.yMin = v;
            rect.yMax = v + h;
        }

        public static Rect MoveYMin(Rect rect, float v)
        {
            var h = rect.height;
            rect.yMin = v;
            rect.yMax = v + h;
            return rect;
        }

        public static void MoveXMax(ref this Rect rect, float v)
        {
            var w = rect.width;
            rect.xMax = v;
            rect.xMin = v - w;
        }

        public static Rect MoveXMax(Rect rect, float v)
        {
            var w = rect.width;
            rect.xMax = v;
            rect.xMin = v - w;
            return rect;
        }

        public static void MoveYMax(ref this Rect rect, float v)
        {
            var h = rect.height;
            rect.yMax = v;
            rect.yMin = v - h;
        }

        public static Rect MoveYMax(Rect rect, float v)
        {
            var h = rect.height;
            rect.yMax = v;
            rect.yMin = v - h;
            return rect;
        }
    }
}
