using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX.Diagnostics;

public class TestAssert : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Assert.Catch<System.NullReferenceException>(this, "Test1", 1, "asdf", 0.2f);
        Assert.Catch<System.NullReferenceException>(this, "Test3");
        try
        {
            Assert.Catch<System.NullReferenceException>(this, "Test2", 1, "asdf", 0.2f);
        }
        catch (System.IndexOutOfRangeException)
        {

        }

        Assert.Catch<System.NotSupportedException>(()=> {
            throw new System.NotSupportedException();
        });
    }

    void Test1(int a,string b,float c)
    {
        throw new System.NullReferenceException();
    }

    void Test2(int a, string b, float c)
    {
        throw new System.IndexOutOfRangeException();
    }

    void Test3()
    {
    }
}
