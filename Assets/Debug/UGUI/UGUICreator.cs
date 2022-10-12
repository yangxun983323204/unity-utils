using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YX
{
    public static class UGUICreator
    {
        public static Canvas CreateCanvas(int w,int h,RenderMode mode,Camera cam)
        {
            var root = new GameObject("Canvas");
            return CreateCanvas(root, w, h, mode, cam);
        }

        public static Canvas CreateCanvas(GameObject root,int w, int h, RenderMode mode, Camera cam)
        {
            var canvas = root.AddComponent<Canvas>();
            var caster = root.AddComponent<GraphicRaycaster>();
            canvas.renderMode = mode;
            if (mode != RenderMode.ScreenSpaceOverlay)
                canvas.worldCamera = cam;

            if (mode == RenderMode.ScreenSpaceOverlay || mode == RenderMode.ScreenSpaceCamera)
            {
                var scaler = root.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 1;
                scaler.referenceResolution = new Vector2(w, h);
            }
            else
            {
                var rectTran = canvas.GetComponent<RectTransform>();
                rectTran.sizeDelta = new Vector2(w, h);
            }

            return canvas;
        }

        public static RectTransform CreateNode(RectTransform parent,int w,int h)
        {
            var obj = new GameObject("Node");
            var t = obj.AddComponent<RectTransform>();
            t.SetParent(parent);
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
            t.sizeDelta = new Vector2(w, h);

            return t;
        }

        public static Image CreateImage()
        {
            var obj = new GameObject("Image");
            var img = obj.AddComponent<Image>();
            return img;
        }

        public static Text CreateText()
        {
            var obj = new GameObject("Text");
            var txt = obj.AddComponent<Text>();
            txt.font = Font.CreateDynamicFontFromOSFont("Arials", 30);
            return txt;
        }

        public static void FillAnchor(RectTransform tar)
        {
            tar.anchorMin = Vector2.zero;
            tar.anchorMax = Vector2.one;
            tar.sizeDelta = Vector2.zero;
        }
    }
}
