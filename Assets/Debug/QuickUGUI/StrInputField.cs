using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class StrInputField : MonoBehaviour
    {
        public Text Read;
        public InputField Write;

        string _val = "";
        public string Value
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

        void Start()
        {
            FromData();
        }

        public string ToData()
        {
            _val = Write.text;
            FromData();
            return _val;
        }

        public void FromData()
        {
            Read.text = _val == null ? "" : _val.ToString();
            Write.text = Read.text;
        }
    }
}
