using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class Vector3InputField : MonoBehaviour
    {
        public Text Read;
        public InputField WriteX, WriteY, WriteZ;

        Vector3 _val = Vector3.zero;
        public Vector3 Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
                FromData();
            }
        }

        private void Start()
        {
            FromData();
        }

        public Vector3 ToData()
        {
            if (!float.TryParse(WriteX.text, out _val.x))
                _val.x = 0;

            if (!float.TryParse(WriteY.text, out _val.y))
                _val.y = 0;

            if (!float.TryParse(WriteZ.text, out _val.z))
                _val.z = 0;

            FromData();
            return _val;
        }

        public void FromData()
        {
            Read.text = string.Format("({0},{1},{2})", _val.x, _val.y, _val.z);
            WriteX.text = _val.x.ToString();
            WriteY.text = _val.y.ToString();
            WriteZ.text = _val.z.ToString();
        }
    }
}
