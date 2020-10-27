using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestTimeCost : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        TimeCost.Begin("0层");
        yield return new WaitForSeconds(1);
        yield return T1();
        Debug.Log(TimeCost.End());
    }

    IEnumerator T1()
    {
        TimeCost.Begin("1层");
        yield return new WaitForSeconds(1);
        yield return T2();
        Debug.Log(TimeCost.End());
    }

    IEnumerator T2()
    {
        TimeCost.Begin("2层");
        yield return new WaitForSeconds(1);
        Debug.Log(TimeCost.End());
    }
}
