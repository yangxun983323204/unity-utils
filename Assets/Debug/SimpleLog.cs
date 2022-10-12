using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

namespace YX
{
    public class SimpleLog
    {
        public enum Stamp
        {
            None,Unity,System,Frame
        }

        public bool logEnabled { get; set; } = true;
        public Stamp StampType { get; set; } = Stamp.Unity;
        /// <summary>
        /// 使用大于0的值将记录调用栈
        /// </summary>
        public int StackDepth { get; set; } = 10;
        public Color StampColor { get; set; } = new Color(0.5f, 0.5f, 0.5f);

        private StreamWriter _logFile;

        public SimpleLog(string path,string nameWithoutEx)
        {
            if (!Application.isEditor)
            {
                _logFile = new StreamWriter(Path.Combine(path, nameWithoutEx+".md"), true, Encoding.UTF8);
            }
        }

        public void Close()
        {
            if (!Application.isEditor)
            {
                _logFile.Close();
            }
        }

        public void Log(object msg)
        {
            if (Application.isEditor)
                Debug.Log(StampHead(ToString(msg)));
            else
                FileLog(ToString(msg));
        }

        public void LogError(object msg)
        {
            if (Application.isEditor)
                Debug.LogError(StampHead(ToString(msg)));
            else
                FileLogError(ToString(msg));
        }

        public void LogWarning(object msg)
        {
            if (Application.isEditor)
                Debug.LogWarning(StampHead(ToString(msg)));
            else
                FileLogWarning(ToString(msg));
        }

        public void LogException(Exception msg)
        {
            if (Application.isEditor)
                Debug.LogError(StampHead(ToString(msg)));
            else
                FileLogError(ToString(msg));
        }

        public void LogFormat(string format, params object[] args)
        {
            if (Application.isEditor)
                Debug.Log(StampHead(Format(format, args)));
            else
                FileLog(Format(format,args));
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            if (Application.isEditor)
                Debug.LogWarning(StampHead(Format(format, args)));
            else
                FileLogWarning(Format(format, args));
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            if (Application.isEditor)
                Debug.LogError(StampHead(Format(format, args)));
            else
                FileLogError(Format(format, args));
        }

        private string Format(string fmt, params object[] args)
        {
            return string.Format(fmt, args);
        }

        private string ToString(object msg)
        {
            if (msg == null)
                return string.Empty;

            else
                return msg.ToString();
        }

        private void FileLog(string str)
        {
            WriteFileLine(str);
        }

        private void FileLogWarning(string str)
        {
            WriteFileLine(str.Dye(Color.yellow));
        }

        private void FileLogError(string str)
        {
            WriteFileLine(str.Dye(Color.red));
        }

        private string StampHead(string str)
        {
            if (StampType == Stamp.Unity)
                str = str.Stamp(StampColor);
            else if (StampType == Stamp.System)
                str = str.SysStamp(StampColor);
            else if (StampType == Stamp.Frame)
                str = str.FrameStamp(StampColor);
            return str;
        }

        private void WriteFileLine(string str)
        {
            str = StampHead(str);

            _logFile.WriteLine(str.CallStack(StackDepth, 3).ToHtmlColor());
        }
    }
}
