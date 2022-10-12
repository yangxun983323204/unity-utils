using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YX.UGUI
{
    public class ForceInput : MonoBehaviour
    {
        public Color TargetColor = Color.blue;
        public bool KeyboardFollow = false;
        public Vector3 FollowOffset;

        [SerializeField]
        public Keyboard _kb;
        ColorBlock _currNormalColor;
        InputField _currInput = null;

        [SerializeField]
        GameObject[] _initRegistRoot = null;

        private void Start()
        {
            if (_kb == null)
                return;

            _kb.onInputChar.AddListener(s => {
                var field = GetCurrInputField();
                if (field != null)
                {
                    field.text += s;
                }
            });

            _kb.onInputDel.AddListener(() => {
                var field = GetCurrInputField();
                if (field != null)
                {
                    var s = field.text;
                    if (string.IsNullOrEmpty(s))
                        return;

                    field.text = s.Substring(0, s.Length - 1);
                }
            });

            _kb.onInputClear.AddListener(() => {
                var field = GetCurrInputField();
                if (field != null)
                {
                    field.text = "";
                }
            });

            _kb.onInputSubmit.AddListener(() => {
                var field = GetCurrInputField();
                if (field != null)
                {
                    field.OnSubmit(new BaseEventData(EventSystem.current));
                }
            });

            if (_initRegistRoot != null)
            {
                foreach (var go in _initRegistRoot)
                {
                    if (go != null)
                    {
                        RegisterAllInputFields(go);
                    }
                }
            }
        }
        public InputField GetCurrInputField()
        {
            return _currInput;
        }

        public void RegisterAllInputFields(GameObject root)
        {
            var fields = root.GetComponentsInChildren<InputField>(true);
            foreach (var field in fields)
                RegistInputField(field);
        }

        public void RegistInputField(InputField field)
        {
            field.keyboardType = (TouchScreenKeyboardType)(-1);
            var trigger = field.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(e => {
                SetFocus(field);
            });
            trigger.triggers.Add(entry);
        }

        public void SetFocus(InputField field)
        {
            if (_currInput == field)
                return;

            if (_currInput != null && field != _currInput)
            {
                _currInput.colors = _currNormalColor;
                try
                {
                    _currInput.OnDeselect(null);
                }
                catch
                {
                }
            }

            _currInput = field;
            if (_currInput == null)
            {
                Debug.Log("清除选中");
                return;
            }

            _currNormalColor = _currInput.colors;

            var cols = _currInput.colors;
            cols.normalColor = TargetColor;
            cols.selectedColor = TargetColor;
            cols.highlightedColor = TargetColor;
            cols.pressedColor = TargetColor;
            _currInput.colors = cols;
            if (_kb != null)
                _kb.gameObject.SetActive(true);

            Debug.Log("选中" + field.gameObject.name);
        }

        private void LateUpdate()
        {
            if (!_kb.gameObject.activeSelf)
                return;

            //if (!HasFocus()) // vr下由于故意使弹出IME报错，焦点监测有问题
            //{
            //    _kb.gameObject.SetActive(false);
            //    return;
            //}

            if (KeyboardFollow && _kb != null && _currInput != null)
            {
                _kb.transform.position = _currInput.transform.position + FollowOffset;
                _kb.transform.rotation = _currInput.transform.rotation;
            }
        }

        bool HasFocus()
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem.current == null");
                return false;
            }

            var curr = EventSystem.current.currentSelectedGameObject;
            if (curr == null)
                return false;

            return curr.transform.IsChildOf(_kb.transform) ||
                (_currInput != null && curr.transform.IsChildOf(_currInput.transform));
        }
    }
}
