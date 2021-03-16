using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestArrayEx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestInsert();
        TestSortedAdd();
        TestReplace();
    }

    void TestInsert()
    {
        int[] o = new int[] {0, 1, 2, 3, 5, 6, 7 };
        o.Insert(o.Length, 4, 4);
        for (int i = 0; i < o.Length; i++)
        {
            Debug.AssertFormat(o[i] == i,"o[i]:{0},i:{1}",o[i],i);
        }
    }

    void TestSortedAdd()
    {
        int[] o = new int[7] { 9,9,9,9,9,9,9};
        o.SortedAdd(0, 6, 0, (a) => { return a > 0; });
        o.SortedAdd(0, 6, 1, (a) => { return a > 1; });
        o.SortedAdd(0, 6, 2, (a) => { return a > 2; });
        o.SortedAdd(0, 6, 3, (a) => { return a > 3; });
        o.SortedAdd(0, 6, 4, (a) => { return a > 4; });
        o.SortedAdd(0, 6, 5, (a) => { return a > 5; });
        for (int i = 0; i < 6; i++)
        {
            Debug.AssertFormat(o[i] == i, "o[i]:{0},i:{1}", o[i], i);
        }
    }

    void TestReplace()
    {
        int[] o = new int[] { 0,1,2,3,5,5,6,7};
        o.SortedReplace(0, o.Length, 4, (a) => { return a > 4; });
        for (int i = 0; i < o.Length; i++)
        {
            Debug.AssertFormat(o[i] == i, "o[i]:{0},i:{1}", o[i], i);
        }
    }
}
