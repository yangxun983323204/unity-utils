using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YX
{
    public static class GameObjectUtils
    {
        public static GameObject MakeHierarchy(string hierachyLines)
        {
            GameObject root = null;
            GameObject currObj = null;
            int currLv = 0;
            var lines = hierachyLines.Trim().Split('\n');
            foreach (var line in lines)
            {
                int spaceCnt = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != ' ')
                        break;

                    spaceCnt++;
                }

                if (currLv==spaceCnt)
                {
                    var n = new GameObject(line.Trim());
                    if(spaceCnt>0)
                        n.transform.SetParent(currObj.transform.parent);
                    currObj = n;
                }
                else if(currLv == (spaceCnt - 1))
                {
                    var n = new GameObject(line.Trim());
                    n.transform.SetParent(currObj.transform);
                    currObj = n;
                    currLv = spaceCnt;
                }
                else if (currLv>spaceCnt)
                {
                    var p = currObj.transform;
                    for (int i = 0; i < currLv - spaceCnt; i++)
                    {
                        p = p.parent;
                    }

                    var n = new GameObject(line.Trim());
                    if (spaceCnt > 0)
                        n.transform.SetParent(p.parent);
                    currObj = n;
                    currLv = spaceCnt;
                }
                else
                {
                    Debug.LogWarning($"跳过不符合格式的行:{line}");
                }

                if (root==null)
                {
                    root = currObj;
                }
            }

            return root;
        }
    }
}
