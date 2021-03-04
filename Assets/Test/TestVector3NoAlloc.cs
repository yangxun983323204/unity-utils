using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestVector3NoAlloc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var t1 = new Vector3(1, -5, 3);
        var t2 = new Vector3(-5, -4, 6);

        var t1n = t1;
        Vector3NoAlloc.Add(ref t1n, ref t2);
        Debug.Assert(t1 + t2 == t1n);

        t1n = t1;
        Vector3NoAlloc.Sub(ref t1n, ref t2);
        Debug.Assert(t1 - t2 == t1n);

        t1n = t1;
        Vector3NoAlloc.Scale(ref t1n, 3);
        Debug.Assert(t1 * 3 == t1n);

        t1n = t1;
        Vector3NoAlloc.Cross(ref t1n, ref t2);
        var c = Vector3.Cross(t1, t2);
        Debug.Assert(c == t1n,$"{c}<--->{t1n}");
    }
}
