using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YX
{
    public static class StrEx
    {
        /// <summary>
        /// 染色
        /// </summary>
        public static string Dye(this string self, Color col)
        {
            var val = ColorUtility.ToHtmlStringRGB(col);
            return "<color=#" + val + ">" + self + "</color>";
        }

        /// <summary>
        /// 染色
        /// </summary>
        public static string Dye(this string self, float r,float g,float b,float a)
        {
            return Dye(self, new Color(r, g, b, a));
        }

        /// <summary>
        /// 在字符串前面加上Unity时间
        /// </summary>
        public static string Stamp(this string self,Color color)
        {
            return ("["+Time.unscaledTime.ToString()+"]").Dye(color) + " " + self;
        }

        /// <summary>
        /// 在字符串前面加上Unity帧数
        /// </summary>
        public static string FrameStamp(this string self, Color color)
        {
            return ("[" + Time.frameCount.ToString() + "]").Dye(color) + " " + self;
        }

        /// <summary>
        /// 在字符串前面加上系统时间
        /// </summary>
        public static string SysStamp(this string self,Color color)
        {
            return ("["+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+"]").Dye(color) + " " + self;
        }

        /// <summary>
        /// 把字符串中的unity颜色标签替换成html颜色标签
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToHtmlColor(this string self)
        {
            self = Regex.Replace(self, @"<color=(?<c>.*?)>", "<font color=\"${c}\">");
            self = self.Replace("</color>", "</font>");
            return self;
        }

        /// <summary>
        /// 在字符串后添加调用栈
        /// </summary>
        public static string CallStack(this string self,int depth = 1,int skip = 0)
        {
            if (depth < 1)
                return self;

            StringBuilder sb = new StringBuilder(self,self.Length + 20);
            var stack = new StackTrace(1+skip);// 跳过自身和外部指定的层数
            for (int i = 0; i <= depth; i++)
            {
                if (i >= stack.FrameCount)
                    break;

                if (i == 0)
                    sb.AppendLine();

                var frame = stack.GetFrame(i);
                var method = frame.GetMethod();
                sb.AppendLine($"<color=#4A79F3>{method.DeclaringType.ToString()}:{method.Name}</color>");
            }
            self = sb.ToString();
            return self;
        }
    }
}
