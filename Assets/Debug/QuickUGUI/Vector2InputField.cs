using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class Vector2InputField : MonoBehaviour
    {
        public Text Read;
        public InputField WriteX, WriteY;

        Vector2 _val = Vector2.zero;
        public Vector2 Value
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

        public Vector2 ToData()
        {
            if (!float.TryParse(WriteX.text, out _val.x))
                _val.x = 0;

            if (!float.TryParse(WriteY.text, out _val.y))
                _val.y = 0;

            FromData();
            return _val;
        }

        public void FromData()
        {
            Read.text = string.Format("({0},{1})", _val.x, _val.y);
            WriteX.text = _val.x.ToString();
            WriteY.text = _val.y.ToString();
        }
    }
}
