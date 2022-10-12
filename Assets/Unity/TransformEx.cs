using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YX
{
    public static class TransformEx
    {
        static Stack<Transform> _tranStack = new Stack<Transform>(20);
        /// <summary>
        /// 协程遍历所有节点
        /// </summary>
        public static IEnumerator AsyncWalk(this Transform t, Func<Transform, object> onNode)
        {
            if (t == null)
                yield break;

            var stack = new Stack<Transform>(20);
            stack.Push(t);
            while (stack.Count>0)
            {
                var top = stack.Pop();
                if (top == null)
                    continue;

                for (int i = top.childCount - 1; i >=0; i--)
                {
                    stack.Push(top.GetChild(i));
                }

                yield return onNode?.Invoke(top);
            }
        }
        /// <summary>
        /// 遍历所有节点
        /// </summary>
        public static void Walk(this Transform t, Action<Transform> onNode)
        {
            if (t == null)
                return;

            _tranStack.Clear();
            _tranStack.Push(t);
            while (_tranStack.Count > 0)
            {
                var top = _tranStack.Pop();
                for (int i = top.childCount - 1; i >= 0; i--)
                {
                    _tranStack.Push(top.GetChild(i));
                }

                onNode?.Invoke(top);
            }
        }

        static StringBuilder _sb = new StringBuilder(256);
        static Stack<string> _strStack = new Stack<string>(20);
        /// <summary>
        /// 获取全路径
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static string GetFullPath(this Transform tran)
        {
            if (tran == null)
                return null;

            _sb.Clear();
            _strStack.Clear();
            var p = tran;
            while (p != null)
            {
                _strStack.Push(p.gameObject.name);
                p = p.parent;
            }

            while (_strStack.Count > 0)
            {
                var node = _strStack.Pop();
                _sb.Append(node);
                _sb.Append("/");
            }
            _sb.Remove(_sb.Length - 1, 1);

            return _sb.ToString();
        }
        /// <summary>
        /// 查找名为name的节点
        /// </summary>
        public static Transform Search(this Transform tran,string name)
        {
            if (tran == null)
                return null;

            _tranStack.Clear();
            _tranStack.Push(tran);
            while (_tranStack.Count > 0)
            {
                var top = _tranStack.Pop();
                for (int i = top.childCount - 1; i >= 0; i--)
                {
                    _tranStack.Push(top.GetChild(i));
                }

                if (top!=null && top.gameObject.name == name)
                {
                    return top;
                }
            }

            return null;
        }
        /// <summary>
        /// 用自定义的方法找到节点
        /// </summary>
        public static Transform Search(this Transform tran, Func<Transform,bool> func)
        {
            if (func == null)
                return null;

            if (tran == null)
                return null;

            _tranStack.Clear();
            _tranStack.Push(tran);
            while (_tranStack.Count > 0)
            {
                var top = _tranStack.Pop();
                for (int i = top.childCount - 1; i >= 0; i--)
                {
                    _tranStack.Push(top.GetChild(i));
                }

                if (func.Invoke(top))
                {
                    return top;
                }
            }

            return null;
        }
    }
}
