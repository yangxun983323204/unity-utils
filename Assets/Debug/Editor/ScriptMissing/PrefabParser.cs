using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YX.Yaml
{
    public class RootNode
    {
        public string Type;
    }

    public class MonoBehaviourNode : RootNode
    {
        public const string TYPE = "MonoBehaviour";

        public string ScriptFileID;
        public string ScriptGUID;

        public MonoBehaviourNode()
        {
            Type = TYPE;
        }
    }

    public class PrefabParser
    {
        private List<RootNode> _allRoot = new List<RootNode>();

        private static readonly string _Key_Script = "m_Script";
        private static readonly string _Key_FileID = "fileID";
        private static readonly string _Key_Guid = "guid";

        public void Load(string prefabPath)
        {
            _allRoot.Clear();
            var yaml = new YamlStream();

            using (var input = new StreamReader(prefabPath, Encoding.UTF8))
            {
                yaml.Load(input);
            }

            foreach (var doc in yaml.Documents)
            {
                var mapping =(YamlMappingNode)doc.RootNode;
                foreach (var entry in mapping.Children)
                {
                    var com = entry.Key as YamlScalarNode;
                    var props = entry.Value as YamlMappingNode;
                    if (com.Value == MonoBehaviourNode.TYPE)
                    {
                        var scriptProp = FindValue(props.Children, _Key_Script) as YamlMappingNode;
                        var node = new MonoBehaviourNode();
                        node.ScriptFileID = (FindValue(scriptProp.Children, _Key_FileID) as YamlScalarNode).Value;
                        node.ScriptGUID = (FindValue(scriptProp.Children, _Key_Guid) as YamlScalarNode).Value;
                        _allRoot.Add(node);
                    }
                    else
                    {
                        var node = new RootNode();
                        node.Type = com.Value;
                        _allRoot.Add(node);
                    }
                }
            }
        }
        /// <summary>
        /// 得到所有的MonoBehaviour节点
        /// </summary>
        /// <returns></returns>
        public List<MonoBehaviourNode> GetMonoBehaviourNodes()
        {
            var list = new List<MonoBehaviourNode>();
            foreach (var item in _allRoot)
            {
                if (item is MonoBehaviourNode)
                {
                    list.Add(item as MonoBehaviourNode);
                }
            }

            return list;
        }

        private static YamlNode FindValue(IDictionary<YamlNode, YamlNode> children,string key)
        {
            return children.First(kv => { return (kv.Key as YamlScalarNode).Value == key; }).Value;
        }
    }
}