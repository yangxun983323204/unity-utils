using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestGameObjectUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestGetFullPath();
    }

    void TestGetFullPath()
    {
        var g0 = new GameObject("一");
        var g1 = new GameObject("二"); g1.transform.SetParent(g0.transform);
        var g2 = new GameObject("三"); g2.transform.SetParent(g1.transform);
        var g3 = new GameObject("四"); g3.transform.SetParent(g2.transform);
        var g4 = new GameObject("五"); g4.transform.SetParent(g3.transform);

        Debug.Log(GameObjectUtils.GetFullPath(null));
        Debug.Log(GameObjectUtils.GetFullPath(g0.transform));
        Debug.Log(GameObjectUtils.GetFullPath(g1.transform));
        Debug.Log(GameObjectUtils.GetFullPath(g2.transform));
        Debug.Log(GameObjectUtils.GetFullPath(g3.transform));
        Debug.Log(GameObjectUtils.GetFullPath(g4.transform));
    }
}
