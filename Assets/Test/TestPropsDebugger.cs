using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestPropsDebugger : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var debugger = gameObject.AddComponent<PropsDebuger>();
        debugger.Add("Test1", 1);
        debugger.Add("Test2", Vector2.one);
        debugger.Add("Test3", "adsfag");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
