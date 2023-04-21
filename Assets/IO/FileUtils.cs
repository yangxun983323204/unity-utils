using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace YX
{
    public class FileUtils
    {
        public static void WriteAllBytes(string path,byte[] bytes)
        {
            bool nofile = false;
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                nofile = true;
            }

            if (!nofile && File.Exists(path))
                File.Delete(path);

            File.WriteAllBytes(path, bytes);
        }
    }
}
