using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YX
{
    public class InputFieldNoKB : InputField
    {
        #region 反射
        public class ReflectBase
        {
            protected object _inst;
        }

        public class Method : ReflectBase
        {
            MethodInfo _info;
            public Method(object inst, MethodInfo info)
            {
                Debug.Assert(info != null);
                _inst = inst;
                _info = info;
            }

            public void Call()
            {
                Call(new object[] { });
            }
            public void Call(object[] parameters)
            {
                _info.Invoke(_inst, parameters);
            }

            public override string ToString()
            {
                return $"{_info}";
            }
        }

        public class Method<T> : ReflectBase
        {
            MethodInfo _info;
            public Method(object inst, MethodInfo info)
            {
                Debug.Assert(info != null);
                _inst = inst;
                _info = info;
            }
            public T Call()
            {
                return Call(new object[] { });
            }
            public T Call(object[] parameters)
            {
                return (T)(_info.Invoke(_inst, parameters));
            }
            public override string ToString()
            {
                return $"{_info}";
            }
        }

        public class Field<T> : ReflectBase
        {
            FieldInfo _info;
            public Field(object inst, FieldInfo info)
            {
                Debug.Assert(info != null);
                _inst = inst;
                _info = info;
            }
            public T Get()
            {
                return (T)(_info.GetValue(_inst));
            }

            public void Set(T v)
            {
                _info.SetValue(_inst, v);
            }
            public override string ToString()
            {
                return $"{_info}";
            }
        }

        public class Property<T> : ReflectBase
        {
            PropertyInfo _info;
            public Property(object inst, PropertyInfo info)
            {
                Debug.Assert(info != null);
                _inst = inst;
                _info = info;
            }
            public T Get()
            {
                return (T)(_info.GetValue(_inst));
            }

            public void Set(T v)
            {
                _info.SetValue(_inst, v);
            }
            public override string ToString()
            {
                return $"{_info}";
            }
        }
        #endregion
        #region 父类私有反射
        Field<bool> ref_m_ShouldActivateNextUpdate;
        Field<CanvasRenderer> ref_m_CachedInputRenderer;
        Field<bool> ref_m_ReadOnly;
        Field<bool> ref_m_WasCanceled;
        Field<bool> ref_m_HideMobileInput;
        Field<bool> ref_m_TouchKeyboardAllowsInPlaceEditing;
        Field<bool> ref_m_AllowInput;
        Field<string> ref_m_OriginalText;

        Property<BaseInput> ref_input;

        Method ref_AssignPositioningIfNeeded;
        Method<bool> ref_InPlaceEditingChanged;
        Method<bool> ref_InPlaceEditing;
        Method ref_UpdateCaretFromKeyboard;
        Method ref_SendOnValueChangedAndUpdateLabel;
        Method ref_SetCaretVisible;



        void InitReflection()
        {
            var binding = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
            var ms = typeof(InputField).GetMethods(binding).ToList();
            var fs = typeof(InputField).GetFields(binding).ToList();
            var ps = typeof(InputField).GetProperties(binding).ToList();

            ref_m_ShouldActivateNextUpdate = new Field<bool>(this, fs.Find(n => n.Name == "m_ShouldActivateNextUpdate"));
            ref_m_CachedInputRenderer = new Field<CanvasRenderer>(this, fs.Find(n => n.Name == "m_CachedInputRenderer"));
            ref_m_ReadOnly = new Field<bool>(this, fs.Find(n => n.Name == "m_ReadOnly"));
            ref_m_WasCanceled = new Field<bool>(this, fs.Find(n => n.Name == "m_WasCanceled"));
            ref_m_HideMobileInput = new Field<bool>(this, fs.Find(n => n.Name == "m_HideMobileInput"));
            ref_m_TouchKeyboardAllowsInPlaceEditing = new Field<bool>(this, fs.Find(n => n.Name == "m_TouchKeyboardAllowsInPlaceEditing"));
            ref_m_AllowInput = new Field<bool>(this, fs.Find(n => n.Name == "m_AllowInput"));
            ref_m_OriginalText = new Field<string>(this, fs.Find(n => n.Name == "m_OriginalText"));

            ref_input = new Property<BaseInput>(this, ps.Find(n => n.Name == "input"));

            ref_AssignPositioningIfNeeded = new Method(this, ms.Find(n => n.Name == "AssignPositioningIfNeeded"));
            ref_InPlaceEditingChanged = new Method<bool>(this, ms.Find(n => n.Name == "InPlaceEditingChanged"));
            ref_InPlaceEditing = new Method<bool>(this, ms.Find(n => n.Name == "InPlaceEditing"));
            ref_UpdateCaretFromKeyboard = new Method(this, ms.Find(n => n.Name == "UpdateCaretFromKeyboard"));
            ref_SendOnValueChangedAndUpdateLabel = new Method(this, ms.Find(n => n.Name == "SendOnValueChangedAndUpdateLabel"));
            ref_SetCaretVisible = new Method(this, ms.Find(n => n.Name == "SetCaretVisible"));
        }

        bool m_ShouldActivateNextUpdate { 
            get { return ref_m_ShouldActivateNextUpdate.Get(); } 
            set { ref_m_ShouldActivateNextUpdate.Set(value); }
        }
        void AssignPositioningIfNeeded()
        {
            ref_AssignPositioningIfNeeded.Call();
        }
        bool InPlaceEditingChanged()
        {
            return ref_InPlaceEditingChanged.Call();
        }
        CanvasRenderer m_CachedInputRenderer
        {
            get { return ref_m_CachedInputRenderer.Get(); }
            set { ref_m_CachedInputRenderer.Set(value); }
        }
        bool InPlaceEditing()
        {
            return ref_InPlaceEditing.Call();
        }
        bool m_ReadOnly
        {
            get { return ref_m_ReadOnly.Get(); }
            set { ref_m_ReadOnly.Set(value); }
        }
        bool m_WasCanceled
        {
            get { return ref_m_WasCanceled.Get(); }
            set { ref_m_WasCanceled.Set(value); }
        }
        void UpdateCaretFromKeyboard()
        {
            ref_UpdateCaretFromKeyboard.Call();
        }
        void SendOnValueChangedAndUpdateLabel()
        {
            ref_SendOnValueChangedAndUpdateLabel.Call();
        }
        bool m_HideMobileInput
        {
            get { return ref_m_HideMobileInput.Get(); }
            set { ref_m_HideMobileInput.Set(value); }
        }
        bool m_TouchKeyboardAllowsInPlaceEditing
        {
            get { return ref_m_TouchKeyboardAllowsInPlaceEditing.Get(); }
            set { ref_m_TouchKeyboardAllowsInPlaceEditing.Set(value); }
        }
        BaseInput input
        {
            get { return ref_input.Get(); }
            set { ref_input.Set(value); }
        }
        bool m_AllowInput
        {
            get { return ref_m_AllowInput.Get(); }
            set { ref_m_AllowInput.Set(value); }
        }
        string m_OriginalText
        {
            get { return ref_m_OriginalText.Get(); }
            set { ref_m_OriginalText.Set(value); }
        }
        void SetCaretVisible()
        {
            ref_SetCaretVisible.Call();
        }
        #endregion

        protected override void Awake()
        {
            InitReflection();
            base.Awake();
        }

        protected override void LateUpdate()
        {
            // Only activate if we are not already activated.
            if (m_ShouldActivateNextUpdate)
            {
                if (!isFocused)
                {
                    ActivateInputFieldInternal();
                    m_ShouldActivateNextUpdate = false;
                    return;
                }

                // Reset as we are already activated.
                m_ShouldActivateNextUpdate = false;
            }

            AssignPositioningIfNeeded();

            // If the device's state changed in a way that affects whether we should use a touchscreen keyboard or not,
            // then we make sure to clear all of the caret/highlight state visually and deactivate the input field.
            if (isFocused && InPlaceEditingChanged())
            {
                if (m_CachedInputRenderer != null)
                {
                    using (var helper = new VertexHelper())
                        helper.FillMesh(mesh);

                    m_CachedInputRenderer.SetMesh(mesh);
                }

                DeactivateInputField();
            }

            if (!isFocused || InPlaceEditing())
                return;

            if (m_Keyboard == null || m_Keyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                if (m_Keyboard != null)
                {
                    if (!m_ReadOnly)
                        text = m_Keyboard.text;

                    if (m_Keyboard.status == TouchScreenKeyboard.Status.Canceled)
                        m_WasCanceled = true;
                }

                OnDeselect(null);
                return;
            }

            string val = m_Keyboard.text;

            if (m_Text != val)
            {
                if (m_ReadOnly)
                {
                    m_Keyboard.text = m_Text;
                }
                else
                {
                    m_Text = "";

                    for (int i = 0; i < val.Length; ++i)
                    {
                        char c = val[i];

                        if (c == '\r' || (int)c == 3)
                            c = '\n';

                        if (onValidateInput != null)
                            c = onValidateInput(m_Text, m_Text.Length, c);
                        else if (characterValidation != CharacterValidation.None)
                            c = Validate(m_Text, m_Text.Length, c);

                        if (lineType == LineType.MultiLineSubmit && c == '\n')
                        {
                            m_Keyboard.text = m_Text;

                            OnDeselect(null);
                            return;
                        }

                        if (c != 0)
                            m_Text += c;
                    }

                    if (characterLimit > 0 && m_Text.Length > characterLimit)
                        m_Text = m_Text.Substring(0, characterLimit);

                    if (m_Keyboard.canGetSelection)
                    {
                        UpdateCaretFromKeyboard();
                    }
                    else
                    {
                        caretPositionInternal = caretSelectPositionInternal = m_Text.Length;
                    }

                    // Set keyboard text before updating label, as we might have changed it with validation
                    // and update label will take the old value from keyboard if we don't change it here
                    if (m_Text != val)
                        m_Keyboard.text = m_Text;

                    SendOnValueChangedAndUpdateLabel();
                }
            }
            else if (m_HideMobileInput && m_Keyboard.canSetSelection)
            {
                var selectionStart = Mathf.Min(caretSelectPositionInternal, caretPositionInternal);
                var selectionLength = Mathf.Abs(caretSelectPositionInternal - caretPositionInternal);
                m_Keyboard.selection = new RangeInt(selectionStart, selectionLength);
            }
            else if (m_Keyboard.canGetSelection && !m_HideMobileInput)
            {
                UpdateCaretFromKeyboard();
            }


            if (m_Keyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                if (m_Keyboard.status == TouchScreenKeyboard.Status.Canceled)
                    m_WasCanceled = true;

                OnDeselect(null);
            }
        }

        void ActivateInputFieldInternal()
        {
            if (EventSystem.current == null)
                return;

            if (EventSystem.current.currentSelectedGameObject != gameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);

            // Cache the value of isInPlaceEditingAllowed, because on UWP this involves calling into native code
            // Usually, the value only needs to be updated once when the TouchKeyboard is opened; however, on Chrome OS,
            // we check repeatedly to see if the in-place editing state has changed, so we can take action.
            m_TouchKeyboardAllowsInPlaceEditing = TouchScreenKeyboard.isInPlaceEditingAllowed;

            // Perform normal OnFocus routine if platform supports it
            if (!TouchScreenKeyboard.isSupported || m_TouchKeyboardAllowsInPlaceEditing)
            {
                if (input != null)
                    input.imeCompositionMode = IMECompositionMode.On;
                OnFocus();
            }
            m_AllowInput = true;
            m_OriginalText = text;
            m_WasCanceled = false;
            SetCaretVisible();
            UpdateLabel();
        }
    }
}