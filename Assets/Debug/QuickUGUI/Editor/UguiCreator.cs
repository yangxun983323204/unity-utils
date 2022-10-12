using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YX;

namespace YX.UGUI.Editor
{
    public static class UguiCreator
    {
        public static bool UseInvertColor = true;

        public static Vector2 TextSize = new Vector2(150, 50);
        public static Vector2 RWInputFielfSize = new Vector2(450, 50);
        public static Vector2 Vector2InputFielfSize = new Vector2(600, 50);
        public static Vector2 Vector3InputFielfSize = new Vector2(750, 50);
        public static Vector2 Color32InputFielfSize = new Vector2(900, 50);
        public static Vector2 ButtonSize = new Vector2(200, 50);
        public static Vector2 DropdownSize = new Vector2(450, 50);
        public static Vector2 ScrollViewSize = new Vector2(450, 450);
        public static Vector2 ToggleSize = new Vector2(200, 50);

        public static int FontSize = 30;

        const int IDX = 2100;
        const int IDX_EX = 2200;

        #region 反射UGUI内部方法
        private static string EditorType(string type)
        {
            return ReflectUtils.GetFullQualifiedName("UnityEditor.UI", "UnityEditor.UI." + type);
        }

        private static string UIType(string type) 
        {
            return ReflectUtils.GetFullQualifiedName("UnityEngine.UI", "UnityEngine.UI." + type);
        }

        static private object GetStandardResources()
        {
            return ReflectUtils.Call(EditorType("MenuOptions"), "GetStandardResources", null, false);
        }

        static public GameObject CreateText()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateText", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateImage()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateImage", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateButton()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateButton", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateToggle()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateToggle", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateSlider()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateSlider", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateScrollbar()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateScrollbar", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateDropdown()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateDropdown", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateInputField()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateInputField", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreatePanel()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreatePanel", null, true, GetStandardResources()) as GameObject;
        }

        static public GameObject CreateScrollView()
        {
            return ReflectUtils.Call(UIType("DefaultControls"), "CreateScrollView", null, true, GetStandardResources()) as GameObject;
        }
        #endregion

        [MenuItem("GameObject/YxUI/RWInputField", false, IDX+1)]
        public static GameObject CreateRWInputField() 
        {
            var root = CreatePanel();
            if (Selection.activeGameObject!=null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            var label = CreateText();
            var read = CreateText();
            var write = CreateInputField();

            root.name = "RWInputField";
            label.name = "Label";
            label.GetComponent<Text>().text = "[xx项]:";
            read.name = "Read";
            read.GetComponent<Text>().text = "值为xx";
            write.name = "Val";

            label.transform.SetParent(root.transform, false);
            read.transform.SetParent(root.transform, false);
            write.transform.SetParent(root.transform, false);

            new StyleBuilder.AnchorSequenceBuilder().
                Add(label, 1).
                Add(read, 1).
                Add(write, 1).
                BuildHorizontal();

            StyleBuilder.Size(root, RWInputFielfSize.x, RWInputFielfSize.y);
            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
                text.alignment = TextAnchor.MiddleLeft;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            return root;
        }

        [MenuItem("GameObject/YxUI/Vector2InputField", false, IDX+2)]
        public static GameObject CreateVector2InputField()
        {
            var root = CreatePanel();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            var label = CreateText();
            var read = CreateText();
            var valX = CreateInputField();
            var valY = CreateInputField();

            root.name = "Vector2InputField";
            label.name = "Label";
            label.GetComponent<Text>().text = "[xx项]:";
            read.name = "Read";
            read.GetComponent<Text>().text = "{0,0,0}";
            valX.name = "ValX";
            valY.name = "ValY";

            label.transform.SetParent(root.transform, false);
            read.transform.SetParent(root.transform, false);
            valX.transform.SetParent(root.transform, false);
            valY.transform.SetParent(root.transform, false);

            new StyleBuilder.AnchorSequenceBuilder().
                Add(label, 1).
                Add(read, 1).
                Add(valX, 1).
                Add(valY, 1).
                BuildHorizontal();

            StyleBuilder.Size(root, Vector2InputFielfSize.x, Vector2InputFielfSize.y);
            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
                text.alignment = TextAnchor.MiddleLeft;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            var ctrl = root.AddComponent<Vector2InputField>();
            ctrl.Read = read.GetComponent<Text>();
            ctrl.WriteX = valX.GetComponent<InputField>();
            ctrl.WriteY = valY.GetComponent<InputField>();
            ctrl.FromData();
            return root;
        }

        [MenuItem("GameObject/YxUI/Vector3InputField", false, IDX+3)]
        public static GameObject CreateVector3InputField()
        {
            var root = CreatePanel();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            var label = CreateText();
            var read = CreateText();
            var valX = CreateInputField();
            var valY = CreateInputField();
            var valZ = CreateInputField();

            root.name = "Vector3InputField";
            label.name = "Label";
            label.GetComponent<Text>().text = "[xx项]:";
            read.name = "Read";
            read.GetComponent<Text>().text = "{0,0,0}";
            valX.name = "ValX";
            valY.name = "ValY";
            valZ.name = "ValZ";

            label.transform.SetParent(root.transform, false);
            read.transform.SetParent(root.transform, false);
            valX.transform.SetParent(root.transform, false);
            valY.transform.SetParent(root.transform, false);
            valZ.transform.SetParent(root.transform, false);

            new StyleBuilder.AnchorSequenceBuilder().
                Add(label, 1).
                Add(read, 1).
                Add(valX, 1).
                Add(valY, 1).
                Add(valZ, 1).
                BuildHorizontal();

            StyleBuilder.Size(root, Vector3InputFielfSize.x, Vector3InputFielfSize.y);
            if(UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
                text.alignment = TextAnchor.MiddleLeft;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            var ctrl = root.AddComponent<Vector3InputField>();
            ctrl.Read = read.GetComponent<Text>();
            ctrl.WriteX = valX.GetComponent<InputField>();
            ctrl.WriteY = valY.GetComponent<InputField>();
            ctrl.WriteZ = valZ.GetComponent<InputField>();
            ctrl.FromData();
            return root;
        }

        [MenuItem("GameObject/YxUI/Button", false, IDX+4)]
        public static GameObject CreateButtonEx()
        {
            var root = CreateButton();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            StyleBuilder.Size(root, ButtonSize.x, ButtonSize.y);

            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
            }

            return root;
        }

        [MenuItem("GameObject/YxUI/Panel", false, IDX+5)]
        public static GameObject CreatePanelEx()
        {
            var root = CreatePanel();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            StyleBuilder.Fill(root, Vector2.zero, Vector2.one);

            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            return root;
        }

        [MenuItem("GameObject/YxUI/Dropdown", false, IDX + 6)]
        public static GameObject CreateDropdownEx()
        {
            var root = CreateDropdown();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            StyleBuilder.Size(root, DropdownSize.x, DropdownSize.y);

            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
            }
            //
            var a = root.transform.Find("Arrow") as RectTransform;
            a.sizeDelta = new Vector2(DropdownSize.y, DropdownSize.y);
            a.anchoredPosition = new Vector2(-DropdownSize.y / 2, 0);
            if (UseInvertColor)
                StyleBuilder.ColorInvert(a.gameObject);// 箭头不需要反色
            // 修改template
            var t = root.GetComponent<Dropdown>().template;
            t.sizeDelta = new Vector2(0, DropdownSize.y * 7);
            var content = t.Find("Viewport").Find("Content") as RectTransform;
            content.sizeDelta = new Vector2(0, DropdownSize.y);
            var item = t.Find("Viewport").Find("Content").Find("Item") as RectTransform;
            item.sizeDelta = new Vector2(0, DropdownSize.y);
            item.anchoredPosition = Vector2.zero;
            if (UseInvertColor)
            {
                StyleBuilder.ColorInvert(item.Find("Item Checkmark").gameObject);
            }
            //
            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            return root;
        }

        [MenuItem("GameObject/YxUI/ScrollView", false, IDX + 7)]
        public static GameObject CreateScrollViewEx()
        {
            var root = CreateScrollView();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            StyleBuilder.Size(root, ScrollViewSize.x, ScrollViewSize.y);

            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            return root;
        }

        [MenuItem("GameObject/YxUI/Text", false, IDX + 8)]
        public static GameObject CreateTextEx()
        {
            var root = CreateText();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            StyleBuilder.Size(root, TextSize.x, TextSize.y);

            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
                text.alignment = TextAnchor.MiddleLeft;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            return root;
        }

        [MenuItem("GameObject/YxUI/Toggle", false, IDX + 9)]
        public static GameObject CreateToggleEx()
        {
            var root = CreateToggle();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            new StyleBuilder.AnchorSequenceBuilder().
                Add(root.transform.Find("Background").gameObject, 10).
                AddSpan(1).
                Add(root.transform.Find("Label").gameObject, 30).
                BuildHorizontal();

            StyleBuilder.Fill(root.transform.Find("Background").Find("Checkmark").gameObject,
                Vector2.zero, Vector2.one);

            StyleBuilder.Size(root, ToggleSize.x, ToggleSize.y);

            if (UseInvertColor)
            {
                StyleBuilder.ColorInvert(root);
                StyleBuilder.ColorInvert(root.GetComponent<Toggle>().graphic.gameObject);
            }

            var texts = root.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
                text.alignment = TextAnchor.MiddleLeft;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            return root;
        }

        [MenuItem("GameObject/YxUI/Color32InputField", false, IDX + 10)]
        public static GameObject CreateColor32InputField()
        {
            var root = CreatePanel();
            if (Selection.activeGameObject != null)
            {
                root.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            var label = CreateText();
            var read = CreateImage();
            var valR = CreateInputField();
            var valG = CreateInputField();
            var valB = CreateInputField();
            var valA = CreateInputField();

            root.name = "Vector3InputField";
            label.name = "Label";
            label.GetComponent<Text>().text = "[xx项]:";
            read.name = "Read";
            read.GetComponent<Image>().color = Color.white;
            valR.name = "ValR";
            valG.name = "ValG";
            valB.name = "ValB";
            valA.name = "ValA";

            label.transform.SetParent(root.transform, false);
            read.transform.SetParent(root.transform, false);
            valR.transform.SetParent(root.transform, false);
            valG.transform.SetParent(root.transform, false);
            valB.transform.SetParent(root.transform, false);
            valA.transform.SetParent(root.transform, false);

            new StyleBuilder.AnchorSequenceBuilder().
                Add(label, 1).
                Add(read, 1).
                Add(valR, 1).
                Add(valG, 1).
                Add(valB, 1).
                Add(valA, 1).
                BuildHorizontal();

            StyleBuilder.Size(root, Color32InputFielfSize.x, Color32InputFielfSize.y);
            if (UseInvertColor)
                StyleBuilder.ColorInvert(root);

            var texts = root.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.fontSize = FontSize;
                text.alignment = TextAnchor.MiddleLeft;
            }

            root.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            var ctrl = root.AddComponent<Color32InputField>();
            ctrl.Read = read.GetComponent<Image>();
            ctrl.WriteR = valR.GetComponent<InputField>();
            ctrl.WriteG = valG.GetComponent<InputField>();
            ctrl.WriteB = valB.GetComponent<InputField>();
            ctrl.WriteA = valA.GetComponent<InputField>();
            ctrl.FromData();
            return root;
        }


        [MenuItem("GameObject/YxUI/Ex/StrInputField", false, IDX_EX+1)]
        public static GameObject CreateStrInputField()
        {
            var obj = CreateRWInputField();
            obj.name = "StrInputField";
            var ctrl = obj.AddComponent<StrInputField>();
            ctrl.Read = obj.transform.Find("Read").GetComponent<Text>();
            ctrl.Write = obj.transform.Find("Val").GetComponent<InputField>();
            ctrl.FromData();
            return obj;
        }

        [MenuItem("GameObject/YxUI/Ex/NumInputField", false, IDX_EX+2)]
        public static GameObject CreateNumInputField()
        {
            var obj = CreateRWInputField();
            obj.name = "NumInputField";
            var ctrl = obj.AddComponent<NumInputField>();
            ctrl.Read = obj.transform.Find("Read").GetComponent<Text>();
            ctrl.Write = obj.transform.Find("Val").GetComponent<InputField>();
            ctrl.FromData();
            return obj;
        }
    }
}
