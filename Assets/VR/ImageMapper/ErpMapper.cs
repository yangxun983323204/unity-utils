using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class ErpMapper
    {
        public enum Layout
        {
            Single,
            LR,
            TB,
        }

        public class TextureData
        {
            public Texture2D tex;
            public Color[] pixels;
            public void Apply() { tex.SetPixels(pixels); tex.Apply(); }
        }

        public static Texture2D GenErp(Texture2D normal, Layout layout)
        {
            if (normal == null)
                throw new System.NullReferenceException("输入图片为null");

            if (!normal.isReadable)
                throw new System.AccessViolationException("输入图片isReadable==false");

            int w = normal.width;
            int h = normal.height;

            var erpTex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            var srcData = new TextureData() { tex = normal, pixels = normal.GetPixels() };
            var dstData = new TextureData() { tex = erpTex, pixels = erpTex.GetPixels() };
            System.Array.Clear(dstData.pixels,0, dstData.pixels.Length);
            if (layout == Layout.Single)
                Map(srcData, new RectInt(0, 0, w, h), dstData, new RectInt(0, 0, w, h), MapToErp);
            else if (layout == Layout.LR)
            {
                var halfW = w / 2;
                Map(srcData, new RectInt(0, 0, halfW, h), dstData, new RectInt(0, 0, halfW, h), MapToErp);
                Map(srcData, new RectInt(halfW, 0, halfW, h), dstData, new RectInt(halfW, 0, halfW, h), MapToErp);
            }
            else if (layout == Layout.TB)
            {
                var halfH = h / 2;
                Map(srcData, new RectInt(0, 0, w, halfH), dstData, new RectInt(0, 0, w, halfH), MapToErp);
                Map(srcData, new RectInt(0, halfH, w, halfH), dstData, new RectInt(0, halfH, w, halfH), MapToErp);
            }
            else
                Debug.LogError("不支持的layout:" + layout);

            dstData.Apply();
            return dstData.tex;
        }

        public delegate void MapFunc(TextureData src, RectInt srcRect, Vector2Int srcPt, TextureData dst, RectInt dstRect, Vector2Int dstPt);

        public static void Map(TextureData src, RectInt srcRect, TextureData dst, RectInt dstRect, MapFunc func)
        {
            float xScale = dstRect.width / srcRect.width;
            float yScale = dstRect.height / srcRect.height;
            var srcPt = new Vector2Int();
            var dstPt = new Vector2Int();
            for (int y = srcRect.yMin; y < srcRect.yMax; y++)
            {
                for (int x = srcRect.xMin; x < srcRect.xMax; x++)
                {
                    srcPt.x = x;
                    srcPt.y = y;
                    dstPt.x = Mathf.RoundToInt((x - srcRect.xMin) * xScale + dstRect.xMin);
                    dstPt.y = Mathf.RoundToInt((y - srcRect.yMin) * yScale + dstRect.yMin);
                    func?.Invoke(src, srcRect, srcPt, dst, dstRect, dstPt);
                }
            }
        }

        public static void MapToErp(TextureData src, RectInt srcRect, Vector2Int srcPt, TextureData dst, RectInt dstRect, Vector2Int dstPt)
        {
            Debug.Assert(src.tex.width == dst.tex.width && src.tex.height == dst.tex.height);
            Debug.Assert(srcRect.Equals(dstRect));
            var halfW = srcRect.width / 2f;
            var halfH = srcRect.height / 2f;
            float angle = 90 * Mathf.Abs((srcPt.y - srcRect.yMin - halfH) / halfH);
            if (angle == 90)
                angle = 89.9f;

            var scaleX = 1 / Mathf.Cos(angle * Mathf.Deg2Rad);
            var px = Mathf.RoundToInt((dstPt.x - srcRect.xMin - halfW) / scaleX + srcRect.xMin + halfW);
            var py = dstPt.y;
            if (px < 0 || px >= dst.tex.width)
                return;

            int i = dstPt.y * src.tex.width + dstPt.x;
            int j = py * src.tex.width + px;
            dst.pixels[i] = src.pixels[j]; 
        }
    }
}
