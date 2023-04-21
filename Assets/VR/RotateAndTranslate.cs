using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

namespace YX
{
    /// <summary>
    /// 手柄摇杆左右控制旋转，上下控制远近
    /// </summary>
    public class RotateAndTranslate:MonoBehaviour
    {
        public Transform Base;
        public Transform Target;
        public Transform BasePosSync;
        public bool Rotate = false;
        public bool Translate = true;
        public bool UseBasePosSync = true;
        public bool UseBaseAngleSync = true;

        public bool SeparateAxis = true;
        public float RotateSpeed = 1f;
        public float TranslateSpeed = 0.1f;
        public float MinDistance = 0;
        public float MaxDistance = 10;

        InputAction _leftHandAxis;
        InputAction _rightHandAxis;

        void Awake()
        {
            _leftHandAxis = new InputAction(
                name: "Left Hand Primary2DAxis",
                type: InputActionType.Value,
                binding: "<XRController>{LeftHand}/Primary2DAxis",
                processors: "StickDeadzone",
                expectedControlType: "Vector2"
                );

            _rightHandAxis = new InputAction(
                name: "Right Hand Primary2DAxis",
                type: InputActionType.Value,
                binding: "<XRController>{RightHand}/Primary2DAxis",
                processors: "StickDeadzone",
                expectedControlType: "Vector2"
                );

            _leftHandAxis.Enable();
            _rightHandAxis.Enable();
        }

        private void Start()
        {
            ApplyTranslate(0.00001f);
            ApplyRotate(0.001f);
        }

        private void OnDestroy()
        {
            _leftHandAxis.Disable();
            _rightHandAxis.Disable();

            _leftHandAxis.Dispose();
            _rightHandAxis.Dispose();
        }

        private void LateUpdate()
        {
            if (Base == null || Target == null)
                return;

            if (UseBasePosSync)
            {
                var syncFrom = BasePosSync == null ? Camera.main?.transform : BasePosSync;
                if (syncFrom != null)
                    Base.transform.position = syncFrom.position;
            }

            if (UseBaseAngleSync)
            {
                var syncFrom = BasePosSync == null ? Camera.main?.transform : BasePosSync;
                if (syncFrom != null)
                {
                    var angle = syncFrom.eulerAngles;
                    angle.x = 0;
                    angle.z = 0;
                    Base.transform.eulerAngles = angle;
                }
            }

            Apply(ReadInput());
        }

        private Vector2 ReadInput()
        {
            var leftHandValue = _leftHandAxis.ReadValue<Vector2>();
            var rightHandValue = _rightHandAxis.ReadValue<Vector2>();
            return leftHandValue + rightHandValue;
        }

        private void Apply(Vector2 val)
        {
            if (val.x == float.NaN)
                val.x = 0;
            if (val.y == float.NaN)
                val.y = 0;

            bool horizontal = Mathf.Abs(val.x) >= Mathf.Abs(val.y);
            if (SeparateAxis)
            {
                if (horizontal)
                {
                    ApplyRotate(val.x);
                }
                else
                {
                    ApplyTranslate(val.y);
                }
            }
            else
            {
                ApplyRotate(val.x);
                ApplyTranslate(val.y);
            }
        }

        private void ApplyRotate(float v) 
        {
            if (!Rotate)
                return;

            if (v == 0)
                return;

            var sign = Mathf.Sign(v);
            var delta = RotateSpeed * sign * Time.deltaTime;
            Target.RotateAround(Base.position, Base.up, delta);
            Target.LookAt(Base, Vector3.up);
        }

        private void ApplyTranslate(float v) 
        {
            if (v == 0)
                return;

            var sign = Mathf.Sign(v);
            var delta = RotateSpeed * sign * Time.deltaTime;
            var dir = Target.position - Base.position;
            if (dir == Vector3.zero)
                dir = Vector3.forward * MinDistance;

            var oldLen = dir.magnitude;
            var len = oldLen + delta;
            len = Mathf.Clamp(len, MinDistance, MaxDistance);
            Target.position = Base.position + dir / oldLen * len;
        }
    }
}
