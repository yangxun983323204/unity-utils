using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;
using UnityEngine.Assertions;

using YXAssert = YX.Diagnostics.Assert;

public class TestUTMatrix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var utm0 = new UTMatrix<int>(2);
        utm0.Set(0, 0, 99);
        Assert.AreEqual(99, utm0.Get(0, 0));
        Assert.AreEqual(default(int), utm0.Get(0, 1));
        Assert.AreEqual(default(int), utm0.Get(1, 1));
        utm0.Set(1, 1, 88);
        Assert.AreEqual(88, utm0.Get(1, 1));
        YXAssert.ExpectException<System.NotSupportedException>(utm0, "Set", 1, 0, 5);
        YXAssert.ExpectException<System.NotSupportedException>(utm0, "Get", 1, 0);
    }
}
