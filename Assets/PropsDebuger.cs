using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class PropsDebuger : MonoBehaviour
    {
        [Serializable]
        public class KV1
        {
            public string Name;
            public string Value;
        }

        public KV1[] Props = new KV1[0];

        private int _useCnt = 0;
        public void Add(string name, object val)
        {
            if (!Application.isEditor)
                return;

            var idx = Array.FindIndex(Props, n => { return n!=null && n.Name == name; });
            if (idx>=0)
            {
                Props[idx].Value = ToStr(val);
            }
            else
            {
                if (_useCnt >= Props.Length)
                {
                    var newArray = new KV1[Props.Length + 2];
                    Array.Copy(Props, newArray, Props.Length);
                    Props = newArray;
                }

                for (int i = 0; i < Props.Length; i++)
                {
                    if (Props[i] == null)
                    {
                        var node = new KV1() { Name = name, Value = ToStr(val) };
                        Props[i] = node;
                        _useCnt++;
                        break;
                    }
                }
            }

        }
        public void Remove(string name)
        {
            if (!Application.isEditor)
                return;

            for (int i = 0; i < Props.Length; i++)
            {
                if (Props[i]!=null && Props[i].Name == name)
                {
                    Props[i] = null;
                }
            }
        }


        private static string ToStr(object o)
        {
            if (o == null)
                return string.Empty;
            else if (o is Vector2)
                return Vec2Str((Vector2)o);
            else if (o is Vector3)
                return Vec2Str((Vector3)o);
            else
                return o.ToString();
        }
        private static string Vec2Str(Vector2 v)
        {
            return string.Format("({0},{1})", v.x, v.y);
        }
        private static string Vec2Str(Vector3 v)
        {
            return string.Format("({0},{1},{2})", v.x, v.y, v.z);
        }
    }
}
