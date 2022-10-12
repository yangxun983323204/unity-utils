using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YX
{
    public static class HandleUtls
    {
        static GUIStyle _styleLabel = null;

        static HandleUtls()
        {
            _styleLabel = new GUIStyle(GUI.skin.label);
        }

        public static Vector3[] GetScreenRect(Vector3 wpos, float size)
        {
            var rect = new Vector3[4];
            GetScreenRect(wpos, size, ref rect);
            return rect;
        }

        public static void GetScreenRect(Vector3 wpos, float size, ref Vector3[] v4)
        {
            var sCenter = HandleUtility.WorldToGUIPoint(wpos);
            var lt = sCenter + new Vector2(-size, size);
            var rt = sCenter + new Vector2(size, size);
            var lb = sCenter + new Vector2(-size, -size);
            var rb = sCenter + new Vector2(size, -size);
            if (v4 == null || v4.Length<4)
                v4 = new Vector3[4];

            v4[0] = HandleUtility.GUIPointToWorldRay(lt).GetPoint(1);
            v4[1] = HandleUtility.GUIPointToWorldRay(rt).GetPoint(1);
            v4[2] = HandleUtility.GUIPointToWorldRay(rb).GetPoint(1);
            v4[3] = HandleUtility.GUIPointToWorldRay(lb).GetPoint(1);
        }

        public static Vector3[] GetPlaneRect(Transform tran, float w, float h)
        {
            var rect = new Vector3[4];
            GetPlaneRect(tran, w, h, ref rect);
            return rect;
        }

        public static void GetPlaneRect(Transform tran, float w, float h, ref Vector3[] v4)
        {
            var f = tran.forward;
            var d = tran.right;
            var center = tran.position;
            var s = HandleUtility.GetHandleSize(center);
            var sw = s * w;
            var sh = s * h;
            if (v4 == null || v4.Length < 4)
                v4 = new Vector3[4];

            v4[0] = center - d * sw + f * sh;
            v4[1] = center + d*sw + f*sh;
            v4[2] = center + d*sw - f*sh;
            v4[3] = center - d*sw - f*sh;
        }

        public static Vector3[] GetScreenLine(Vector3 wpos, Vector2 dir, float len)
        {
            var pair = new Vector3[2];
            GetScreenLine(wpos, dir, len, ref pair);
            return pair;
        }

        public static void GetScreenLine(Vector3 wpos, Vector2 dir, float len, ref Vector3[] v2)
        {
            var from = HandleUtility.WorldToGUIPoint(wpos);
            var to = from + dir * len;
            if (v2 == null || v2.Length < 2)
                v2 = new Vector3[2];

            v2[0] = HandleUtility.GUIPointToWorldRay(from).GetPoint(1);
            v2[1] = HandleUtility.GUIPointToWorldRay(to).GetPoint(1);
        }

        public static void Label(Vector3 wpos, string text, Color col)
        {
            var old = _styleLabel.normal.textColor;
            _styleLabel.normal.textColor = col;
            Handles.Label(wpos, text, _styleLabel); 
            _styleLabel.normal.textColor = old;
        }
    }
}
