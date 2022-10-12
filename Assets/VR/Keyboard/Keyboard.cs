using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class Keyboard : MonoBehaviour
    {
        public class CharEvent : UnityEvent<string> { }
        public class FuncEvent : UnityEvent { }

        public enum InputType
        {
            Number,
            Char,
        }
        public CharEvent onInputChar { get; } = new CharEvent();
        public FuncEvent onInputDel { get; } = new FuncEvent();
        public FuncEvent onInputClear { get; } = new FuncEvent();
        public FuncEvent onInputSubmit { get; } = new FuncEvent();


        public RectTransform Left, Main, Right, Bottom;
        public RectTransform MultiCharSelect;

        private InputType _type = InputType.Number;
        public InputType Type { get; set; }

        private enum KeyLocation
        {
            Left,
            Main,
            Right,
            Bottom
        }

        Button[]_lBtns, _mBtns, _rBtns, _bBtns;
        Button[] _multiBtns;

        bool _Upper = false;

        void Start()
        {
            InitBinding();
            UpdateView();
        }

        private void OnDisable()
        {
            MultiCharSelect.gameObject.SetActive(false);
        }

        void InitBinding()
        {
            _mBtns = Main.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _mBtns.Length; i++)
            {
                int idx = i;
                _mBtns[i].onClick.AddListener(() => {
                    OnButtonClick(KeyLocation.Main, idx);
                });
            }

            _rBtns = Right.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _rBtns.Length; i++)
            {
                int idx = i;
                _rBtns[i].onClick.AddListener(() => {
                    OnButtonClick(KeyLocation.Right, idx);
                });
            }

            _bBtns = Bottom.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _bBtns.Length; i++)
            {
                int idx = i;
                _bBtns[i].onClick.AddListener(() => {
                    OnButtonClick(KeyLocation.Bottom, idx);
                });
            }

            _lBtns = Left.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _lBtns.Length; i++)
            {
                int idx = i;
                _lBtns[i].onClick.AddListener(() => {
                    OnButtonClick(KeyLocation.Left, idx);
                });
            }

            _multiBtns = MultiCharSelect.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _multiBtns.Length; i++)
            {
                int idx = i;
                _multiBtns[i].onClick.AddListener(() => {
                    if (_lastMultiChrs!=null)
                    {
                        MultiCharSelect.gameObject.SetActive(false);
                        onInputChar.Invoke(_lastMultiChrs[idx]);
                    }
                });
            }

            MultiCharSelect.gameObject.SetActive(false);
        }

        void UpdateView()
        {
            for (int i = 0; i <= 8; i++)
            {
                SetBtnText(_mBtns[i], GetBtnShowText(KeyLocation.Main, i));
            }

            for (int i = 0; i <= 2; i++)
            {
                SetBtnText(_rBtns[i], GetBtnShowText(KeyLocation.Right, i));
            }

            for (int i = 0; i <= 3; i++)
            {
                SetBtnText(_bBtns[i], GetBtnShowText(KeyLocation.Bottom, i));
            }

            for (int i = 0; i <= 3; i++)
            {
                SetBtnText(_lBtns[i], GetBtnShowText(KeyLocation.Left, i));
            }
        }

        void OnButtonClick(KeyLocation location, int idx)
        {
            var val = GetBtnVal(location, idx);
            if (location == KeyLocation.Main)
            {
                HandleValBtnClick(val);
            }
            else if (location == KeyLocation.Right)
            {
                if (idx == 0)
                    onInputDel.Invoke();
                else if (idx == 1)
                    onInputClear.Invoke();
                else if(idx == 2)
                {
                    if (Type == InputType.Number)
                        HandleValBtnClick(val);
                    else
                    {
                        _Upper = !_Upper;
                        UpdateView();
                    }
                }

            }
            else if (location == KeyLocation.Bottom)
            {
                if (idx == 0)
                {
                    Type = Type == InputType.Number ? InputType.Char : InputType.Number;
                    UpdateView();
                }
                else if (idx == 1)
                    HandleValBtnClick(" ");
                else if (idx == 2)
                    HandleValBtnClick(val);
                else if (idx == 3)
                    onInputSubmit.Invoke();
            }
            else if (location == KeyLocation.Left)
            {
                HandleValBtnClick(val);
            }
        }

        void SetBtnText(Button btn, string s)
        {
            btn.GetComponentInChildren<Text>().text = s;
        }

        void HandleValBtnClick(string s)
        {
            if (s.Length == 1)
            {
                onInputChar.Invoke(s);
            }
            else if(s.Length > 1)
            {
                string[] chrs = new string[s.Length];
                for (int i = 0; i < chrs.Length; i++)
                {
                    chrs[i] = s[i].ToString();
                }
                OpenCharSelect(chrs);
            }
        }

        string[] _lastMultiChrs = null;
        void OpenCharSelect(string[] chrs)
        {
            _lastMultiChrs = chrs;
            MultiCharSelect.gameObject.SetActive(true);
            for (int i = 0; i < chrs.Length; i++)
            {
                _multiBtns[i].gameObject.SetActive(true);
                SetBtnText(_multiBtns[i], chrs[i]);
            }

            for (int i = chrs.Length; i < _multiBtns.Length; i++)
            {
                _multiBtns[i].gameObject.SetActive(false);
            }
        }

        string[][] _leftVal = new string[][] {
            new string[]{ "+",},
            new string[]{ "-",},
            new string[]{ "*",},
            new string[]{ "/",},
        };

        string[][] _mainVal = new string[][] {
            new string[]{ "1", "@" },
            new string[]{ "2", "ABC", "abc" },
            new string[]{ "3", "DEF", "def" },
            new string[]{ "4", "GHI", "ghi" },
            new string[]{ "5", "JKL", "jkl" },
            new string[]{ "6", "MNO", "mno" },
            new string[]{ "7", "PQRS", "pqrs" },
            new string[]{ "8", "TUV", "tuv" },
            new string[]{ "9", "WXYZ", "wxyz" },
        };

        string[][] _rightVal = new string[][] {
            new string[]{ "删除"},
            new string[]{ "清空"},
            new string[]{ "0", "小写", "大写" },
        };

        string[][] _bottomVal = new string[][] {
            new string[]{ "字母", "数字" },
            new string[]{ "空格"},
            new string[]{ "."},
            new string[]{ "确定"},
        };

        string GetBtnShowText(KeyLocation location,int i)
        {
            string[] array = null;
            if (location == KeyLocation.Main)
            {
                array = _mainVal[i];
            }
            else if (location == KeyLocation.Right)
            {
                array = _rightVal[i];
            }
            else if (location == KeyLocation.Bottom)
            {
                array = _bottomVal[i]; ;
            }
            else if (location == KeyLocation.Left)
            {
                array = _leftVal[i];
            }

            if (array==null)
                return "";

            if (Type == InputType.Number)
            {
                return array[0];
            }
            else
            {
                int idx = 1;
                if (!_Upper)
                    idx = 2;

                if (idx >= array.Length)
                    idx = array.Length - 1;

                return array[idx];
            }
        }

        string GetBtnVal(KeyLocation location, int i)
        {
            string[] array = null;
            if (location == KeyLocation.Main)
            {
                array = _mainVal[i];
            }
            else if (location == KeyLocation.Right)
            {
                array = _rightVal[i];
            }
            else if (location == KeyLocation.Bottom)
            {
                array = _bottomVal[i]; ;
            }
            else if (location == KeyLocation.Left)
            {
                array = _leftVal[i];
            }

            if (array == null)
                return "";

            if (array.Length == 1)
                return array[0];

            if (Type == InputType.Number)
            {
                return array[0];
            }
            else
            {
                int idx = 1;
                if (!_Upper)
                    idx = 2;

                if (idx >= array.Length)
                    idx = array.Length - 1;

                return array[idx] + array[0];
            }
        }
    }
}
