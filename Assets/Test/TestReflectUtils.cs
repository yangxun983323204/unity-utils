using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestReflectUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ReflectUtils.ClearCache();
        Debug.Log(typeof(Debug).AssemblyQualifiedName);
        ReflectUtils.Call("UnityEngine.Debug, UnityEngine.CoreModule", "Log", null, true, "我了个大擦");
        ReflectUtils.Call("TestReflectUtils", "Func0", this, false);
        ReflectUtils.Call("TestReflectUtils", "Func1", null, false);

        Debug.Assert(ReflectUtils.GetProperty("TestReflectUtils", "P0", this, false) as string == "P0");
        Debug.Assert(ReflectUtils.GetProperty("TestReflectUtils", "P1", null, false) as string == "P1");
        Debug.Assert(ReflectUtils.GetProperty("TestReflectUtils", "P3", this, false) as string == null);
        ReflectUtils.SetProperty("TestReflectUtils", "P3", this, false, "P3");
        Debug.Assert(ReflectUtils.GetProperty("TestReflectUtils", "P3", this, false) as string == "P3");

        ReflectUtils.New("TestReflectUtils+TC", null);
    }

    string P0
    {
        get { return "P0"; }
    }

    static string P1
    {
        get { return "P1"; }
    }

    string P3 { get; set; }

    void Func0()
    {
        Debug.Log("Func0");
    }

    static void Func1()
    {
        Debug.Log("Func1");
    }

    class TC
    {
        public TC()
        {
            Debug.Log("TC NEW");
        }
    }
}
