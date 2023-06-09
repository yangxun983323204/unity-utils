using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YX
{
    public class ImageMapEditor : EditorWindow
    {
        [MenuItem("GameObject/ImageMapper", priority = 0)]
        public static void Call()
        {
            var wnd = GetWindow<ImageMapEditor>("ImageMapper");
            wnd.Show();
            wnd.minSize = new Vector2(400, 270);
            wnd.maxSize = new Vector2(400, 270);
        }

        Texture2D _tar;
        ErpMapper.Layout _layout;
        private void OnGUI()
        {
            _tar = EditorGUILayout.ObjectField(_tar, typeof(Texture2D), false) as Texture2D;
            _layout = (ErpMapper.Layout)EditorGUILayout.EnumPopup("图片布局", _layout);
            if (GUILayout.Button("生成ERP纹理"))
            {
                if (_tar == null)
                    throw new System.NullReferenceException("未选择图片");

                var path = AssetDatabase.GetAssetPath(_tar);
                var dstPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_erp.png");
                Debug.LogFormat("GenErp layout:{0}, {1}->{2}", _layout, path, dstPath);
                var dstTex = ErpMapper.GenErp(_tar, _layout);
                var bytes = dstTex.EncodeToPNG();
                File.WriteAllBytes(dstPath, bytes);
                AssetDatabase.Refresh();
            }
        }
    }
}