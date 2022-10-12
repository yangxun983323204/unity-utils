using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestListAccessor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var la = new ListAccessor<int>(5);
        la.Reserve(10);
        la[9] = 2;
        Debug.Assert(la[0] == default(int));
        Debug.Assert(la[8] == default(int));
        Debug.Assert(la[9] == 2);
        Debug.Log(la[10]);// 产生异常
    }
}
