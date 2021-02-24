using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestSharedObj : MonoBehaviour
{
    public class TA
    {
        public int a;
        public string b;

        public override string ToString()
        {
            return $"a:{a},b:{b}";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var raw = new TA() { a= 2,b="abc" };
        using (var p1 = new SharedObj<TA>()) {
            Debug.Assert(p1.Count == 0);
            p1.Set(raw, n => { Debug.Log("删除！"); });
            Debug.Assert(p1.Count == 1);

            using (var p2 = p1.Share()) {
                Debug.Assert(p1.Count == 2);
                Debug.Assert(p2.Count == 2);
                p2.Tar.a = 3;
                p2.Tar.b = "456";
                Debug.Assert(p1.Tar.a == 3);
                Debug.Assert(p1.Tar.b == "456");
            }

            Debug.Assert(p1.Count == 1);
        }
    }
}
