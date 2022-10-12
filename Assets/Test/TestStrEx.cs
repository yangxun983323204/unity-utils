using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestStrEx : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log("啊哒~~~~".Dye(Color.red));
        Debug.Log("啊哒~~~~".Dye(0,0.5f,0.5f,1));
        Debug.Log("啊哒~~~~".Dye(Color.red).ToHtmlColor());
        TestCallStack();
        yield return TestStamp();
        yield return TestSysStamp();
    }

    IEnumerator TestStamp()
    {
        Debug.Log("TestStamp".Dye(Color.yellow).Stamp(Color.gray));
        yield return new WaitForSeconds(1);
        Debug.Log("TestStamp".Dye(Color.yellow).Stamp(Color.gray));
    }

    IEnumerator TestSysStamp()
    {
        Debug.Log("TestSysStamp".Dye(Color.green).SysStamp(Color.gray));
        yield return new WaitForSeconds(1);
        Debug.Log("TestSysStamp".Dye(Color.green).SysStamp(Color.gray));
    }

    void TestCallStack()
    {
        A();
    }

    void A()
    {
        B();
    }

    void B()
    {
        C();
    }

    void C()
    {
        var str = "测试调用栈";
        Debug.Log(str.CallStack(1));
        Debug.Log(str.CallStack(2));
        Debug.Log(str.CallStack(3));
        Debug.Log(str.CallStack(4));
        Debug.Log(str.CallStack(5));
    }
}
