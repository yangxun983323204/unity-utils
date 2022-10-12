using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CollisionIgnoreChecker : MonoBehaviour
{
    public GameObject Go1;
    public GameObject Go2;

    public bool CheckIsIgnore()
    {
        if (Go1 == null || Go2 == null)
        {
            Debug.Log("请先指定Go1和Go2");
            return true;
        }

        bool isIgnore = true;
        var go1List = Go1.GetComponentsInChildren<Collider>(true);
        var go2List = Go2.GetComponentsInChildren<Collider>(true);

        foreach (var c1 in go1List)
        {
            foreach (var c2 in go2List)
            {
                var s = Physics.GetIgnoreCollision(c1, c2);
                if (!s)
                {
                    isIgnore = false;
                    Debug.LogFormat("存在碰撞:<color=#ff0000>{0}</color>和<color=#ff0000>{1}</color>", 
                        GetName(c1.gameObject) + "<color=#ffff00><" + c1.GetType().Name + "></color>",
                        GetName(c2.gameObject) + "<color=#ffff00><" + c2.GetType().Name + "></color>");
                }
            }
        }

        if (isIgnore)
        {
            Debug.LogFormat("<color=#ff0000>{0}</color>和<color=#ff0000>{1}</color>之间不存在碰撞", Go1.name, Go2.name);
        }

        return isIgnore;
    }

    private string GetName(GameObject g,int useParentCnt = 3)
    {
        StringBuilder sb = new StringBuilder(20);
        var list = new List<Transform>(useParentCnt);
        var curr = g.transform;
        for (int i = 0; i < useParentCnt; i++)
        {
            if (curr == null)
                break;

            list.Add(curr);
            curr = curr.parent;
        }

        if (list.Count <= 0)
            return null;

        if (curr!=null)
        {
            sb.Append("...");
            sb.Append('/');
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            sb.Append(list[i].gameObject.name);
            sb.Append('/');
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}
