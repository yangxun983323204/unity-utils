using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class StrEx
    {
        public static string Dye(this string self, Color col)
        {
            var val = ColorUtility.ToHtmlStringRGBA(col);
            return "<color=#" + val + ">" + self + "</color>";
        }

        public static string Dye(this string self, float r,float g,float b,float a)
        {
            return Dye(self, new Color(r, g, b, a));
        }
    }
}
