using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestCoroutineObj : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        var cor = new CoroutineObj();
        cor.SetRunner(this);
        cor.StartCoroutine(E1());
        yield return new WaitForSeconds(2);
        cor.StartCoroutine(E2());
    }

    IEnumerator E1()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("1");
    }

    IEnumerator E2()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("2");
    }
}
