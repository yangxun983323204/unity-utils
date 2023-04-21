using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public struct Line
    {
        private Vector3 _o;
        private Vector3 _dir;

        public Line(Vector3 point,Vector3 dir)
        {
            _o = point;
            _dir = dir.normalized;
        }

        public bool InLine(Vector3 pos)
        {
            var op = pos - _o;
            var proj = Vector3.Dot(op, _dir);
            return proj == 0;
        }

        public float Distance(Vector3 pos)
        {
            var op = pos - _o;
            var proj = Vector3.Dot(op, _dir);
            if (proj == 0)
                return 0;

            return (op - _dir * proj).magnitude;
        }
    }

}
