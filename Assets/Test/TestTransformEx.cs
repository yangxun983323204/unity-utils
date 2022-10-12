using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestTransformEx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestGetFullPath();
        TestAsyncWalk();
        TestSearch();
    }

    void TestAsyncWalk()
    {
        string fmt =
@"
一
 地
 在
  要
  工
   上
    是
中
";
        var root = GameObjectUtils.MakeHierarchy(fmt);
        StartCoroutine(root.transform.AsyncWalk((t)=> {
            Debug.Log(t.gameObject.name.Stamp(Color.red));
            return new WaitForSeconds(1);
        }));
    }

    void TestGetFullPath()
    {
        var g0 = new GameObject("一");
        var g1 = new GameObject("二"); g1.transform.SetParent(g0.transform);
        var g2 = new GameObject("三"); g2.transform.SetParent(g1.transform);
        var g3 = new GameObject("四"); g3.transform.SetParent(g2.transform);
        var g4 = new GameObject("五"); g4.transform.SetParent(g3.transform);

        Debug.Log(TransformEx.GetFullPath(null));
        Debug.Log(g0.transform.GetFullPath());
        Debug.Log(g1.transform.GetFullPath());
        Debug.Log(g2.transform.GetFullPath());
        Debug.Log(g3.transform.GetFullPath());
        Debug.Log(g4.transform.GetFullPath());
    }

    void TestSearch()
    {
        string fmt =
@"
一
 地
 在
  要
  工
   上
    是
中
";
        var root = GameObjectUtils.MakeHierarchy(fmt);
        Debug.Assert(root.transform.Search("是").name == "是");
        Debug.Assert(root.transform.Search("同") == null);
        Debug.Assert(root.transform.Search(n => n.name == "是").name == "是");
        Debug.Assert(root.transform.Search(n => n.name == "同") == null);
    }
}
