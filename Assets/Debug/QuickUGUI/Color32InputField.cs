using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class Color32InputField : MonoBehaviour
    {
        public Image Read;
        public InputField WriteR, WriteG, WriteB, WriteA;

        Color32 _val = Color.white;
        public Color32 Value
        {
            get { return _val; }
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

        public Color32 ToData()
        {
            if (!byte.TryParse(WriteR.text, out _val.r))
                _val.r = 0;

            if (!byte.TryParse(WriteG.text, out _val.g))
                _val.g = 0;

            if (!byte.TryParse(WriteB.text, out _val.b))
                _val.b = 0;

            if (!byte.TryParse(WriteA.text, out _val.a))
                _val.a = 0;

            FromData();
            return _val;
        }

        public void FromData()
        {
            Read.color = _val;
            WriteR.text = _val.r.ToString();
            WriteG.text = _val.g.ToString();
            WriteB.text = _val.b.ToString();
            WriteA.text = _val.a.ToString();
        }
    }
}
