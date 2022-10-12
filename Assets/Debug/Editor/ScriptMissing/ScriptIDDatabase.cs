using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YX.Yaml
{
    public class ScriptIDDatabase
    {
        public class ScriptID
        {
            public string Name;
            public string FileID;
            public string GUID;
        }

        List<ScriptID> _all = new List<ScriptID>();
        public List<ScriptID> All { get { return _all; } }
        public bool Log { get; set; } = false;
        public bool Loaded { get; private set; } = false;
        private string _loadPath;

        public void Load(string path)
        {
            try
            {
                _loadPath = path;
                var input = new StringReader(File.ReadAllText(path, Encoding.UTF8));
                var deserializer = new DeserializerBuilder()
                    .Build();

                _all = deserializer.Deserialize<List<ScriptID>>(input);
                //
                Loaded = true;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                _all = new List<ScriptID>();
                Loaded = false;
            }
        }

        public void Save()
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(_all);
            File.WriteAllText(_loadPath, yaml, Encoding.UTF8);
        }

        public void SaveAs(string path)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(_all);
            File.WriteAllText(path, yaml, Encoding.UTF8);
        }

        public ScriptID Search(string name)
        {
            return _all.Find(n => { return n.Name == name; });
        }

        public ScriptID Search(string fileID, string guid)
        {
            return _all.Find(n => { return n.FileID == fileID && n.GUID == guid; });
        }

        public bool Add(ScriptID id)
        {
            var n0 = Search(id.Name);
            if (n0 != null)
                return false;
            else
            {
                _all.Add(id);
                return true;
            }
        }

        public void AddOrUpdate(ScriptID id)
        {
            var n0 = Search(id.Name);
            var n1 = Search(id.FileID, id.GUID);
            if (n1!=null && n0 != n1)
            {
                throw new System.Exception($"查询到冲突:\nname:{id.Name},fileID:{id.FileID},guid:{id.GUID}\nname:{n1.Name},fileID:{n1.FileID},guid:{n1.GUID}");
            }

            if (n0 != null)
            {
                n0.FileID = id.FileID;
                n0.GUID = id.GUID;
                if (Log)
                    Debug.Log($"update {n0.Name} from fileID:{n0.FileID},guid:{n0.GUID} to fileID:{id.FileID},guid:{id.GUID}");
            }
            else
            {
                _all.Add(id);
                if (Log)
                    Debug.Log($"add name:{id.Name},fileID:{id.FileID},guid:{id.GUID}");
            }
        }

        public void Clear()
        {
            _all.Clear();
        }
    }
}
