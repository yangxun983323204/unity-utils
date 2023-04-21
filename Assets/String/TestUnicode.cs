#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace YX
{
    public class TestUnicode : MonoBehaviour
    {
        public Text T;
        public string Unicode;
        public int UniNum;

        UnicodeSet.BlocksGroupContext _ctx0;
        UnicodeSet.BlocksGroupContext _ctx1;
        // Start is called before the first frame update
        void Start()
        {
            var bytes = HexStrToBytes(Unicode);
            var s = Encoding.Unicode.GetString(bytes);
            if (T != null)
            {
                T.text = "->" + s + "<-";
                
            }
            print(s);
            print("int->string:"+Encoding.Unicode.GetString(BitConverter.GetBytes(UniNum)));

            var group0 = new UnicodeSet.BlockType[] { UnicodeSet.BlockType.基本拉丁字母 };
            var group1 = UnicodeSet.GetCommonUseBlocks();
            _ctx0 = UnicodeSet.GenerateContext(group0);
            _ctx1 = UnicodeSet.GenerateContext(group1);
        }

        byte[] HexStrToBytes(string hex)
        {
            int n = hex.Length;
            int mod = n % 2;
            if (mod == 1)
                hex = "0" + hex;

            byte[] bytes = new byte[(n + 1) / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            Array.Reverse(bytes);

            return bytes;
        }


        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                print("->"+UnicodeSet.Random(_ctx0, 8)+"<-");
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                print("->" + UnicodeSet.Random(_ctx1, 8) + "<-");
            }
        }
    }
}
#endif