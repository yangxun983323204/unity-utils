using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class TransformFollow : MonoBehaviour
    {
        [Serializable]
        public class FollowInfo
        {
            public bool Use = false;
            public float Delay = 0;
        }

        [Serializable]
        public class RotFollowInfo: FollowInfo
        {
            public bool Z = true;
        }

        public Transform Target;
        public FollowInfo Pos = new FollowInfo();
        public RotFollowInfo Rot = new RotFollowInfo();

        Transform _tran;
        Vector3 _vPos = Vector3.zero;
        Quaternion _vRot = Quaternion.identity;

        private void Awake()
        {
            _tran = transform;
        }

        private void LateUpdate()
        {
            if (Target!=null)
            {
                if (Pos.Use)
                    _tran.position = Vector3.SmoothDamp(_tran.position, Target.position, ref _vPos, Pos.Delay);

                if (Rot.Use)
                {
                    var from = _tran.rotation;
                    Quaternion to;
                    if (Rot.Z)
                        to = Target.rotation;
                    else {
                        var a = Target.rotation.eulerAngles;
                        a.z = 0;
                        to = Quaternion.Euler(a);
                    }
                    _tran.rotation = SmoothDamp(from, to, ref _vRot, Pos.Delay);
                }
            }
        }

        public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
        {
            // https://gist.github.com/maxattack/4c7b4de00f5c1b95a33b
            if (Time.deltaTime < Mathf.Epsilon) return rot;
            // account for double-cover
            var Dot = Quaternion.Dot(rot, target);
            var Multi = Dot > 0f ? 1f : -1f;
            target.x *= Multi;
            target.y *= Multi;
            target.z *= Multi;
            target.w *= Multi;
            // smooth damp (nlerp approx)
            var Result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
            ).normalized;

            // ensure deriv is tangent
            var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
            deriv.x -= derivError.x;
            deriv.y -= derivError.y;
            deriv.z -= derivError.z;
            deriv.w -= derivError.w;

            return new Quaternion(Result.x, Result.y, Result.z, Result.w);
        }
    }
}
