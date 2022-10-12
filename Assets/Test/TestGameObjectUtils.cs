using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestGameObjectUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestMakeHierarchy();
    }

    

    void TestMakeHierarchy()
    {
        string fmt = 
@"
我是第一层
 我是第二层
 我也是第二层
  我是第三层
我又是第一层
我还是第一层
 我是第二层
我再是第一层
";
        GameObjectUtils.MakeHierarchy(fmt);
    }
}
