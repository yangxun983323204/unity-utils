using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YX
{
    public static class GameObjectUtils
    {
        static StringBuilder _sb = new StringBuilder(256);
        static Stack<string> _stack = new Stack<string>(20);

        public static string GetFullPath(Transform tran)
        {
            if (tran == null)
                return null;

            _sb.Clear();
            var p = tran;
            while (p != null)
            {
                _stack.Push(p.gameObject.name);
                p = p.parent;
            }

            while (_stack.Count > 0)
            {
                var node = _stack.Pop();
                _sb.Append(node);
                _sb.Append("/");
            }
            _sb.Remove(_sb.Length - 1, 1);

            return _sb.ToString();
        }
    }
}
