using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class NumInputField:MonoBehaviour
    {
        public Text Read;
        public InputField Write;

        float _val = 0;
        public float Value {
            get{
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

        public float ToData()
        {
            if (!float.TryParse(Write.text,out _val))
                _val = 0;

            FromData();
            return _val;
        }

        public void FromData()
        {
            Read.text = _val.ToString();
            Write.text = Read.text;
        }
    }
}
