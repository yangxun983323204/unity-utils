using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Text;

namespace YX
{
    public class Options
    {
        XmlDocument _xml;

        public void InitWithFile(string xmlPath)
        {
            _xml = new XmlDocument();
            _xml.Load(xmlPath);
            ParseXml();
        }
        public void InitWithContent(string xmlContent)
        {
            _xml = new XmlDocument();
            _xml.LoadXml(xmlContent);
            ParseXml();
        }

        private void ParseXml()
        {
            var stack = new Stack<XmlNode>(20);
            stack.Push(_xml);
            while (stack.Count>0)
            {
                var n = stack.Pop();
                OnParseNode(n, stack.Count);
                foreach (XmlNode c in n)
                {
                    stack.Push(c);
                }
            }
        }

        protected virtual void OnParseNode(XmlNode node,int depth)
        {
            StringBuilder sb = new StringBuilder(20);
            for (int i = 0; i < depth; i++)
            {
                sb.Append("    ");
            }
            sb.AppendLine(node.Name);
            Debug.Log(sb.ToString());
        }
    }
}
