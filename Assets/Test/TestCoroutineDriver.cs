using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestCoroutineDriver : MonoBehaviour
{
    CoroutineDriver _driver;
    int _frame = 0;
    bool _input = false;
    // Start is called before the first frame update
    void Start()
    {
        _driver = new CoroutineDriver();
        _driver.StartCoroutine(F0());
    }

    // Update is called once per frame
    void Update()
    {
        _driver.FixedUpdate();
        _driver.Update();
        _frame++;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _input = true;
        }
    }


    IEnumerator F0()
    {
        Log("f0 start");
        yield return new WaitForEndOfFrame();
        Log("f0 WaitForEndOfFrame");
        yield return new WaitForSeconds(3);
        Log("f0 WaitForSeconds");
        yield return _driver.StartCoroutine(FP());
        Log("f0 wait fp");
        yield return F1();
        Log("f0 wait F1");
        yield break;
        Log("f0 break");
    }

    IEnumerator F1()
    {
        Log("f1 start");
        yield return new WaitForEndOfFrame();
        Log("f1 WaitForEndOfFrame");
        yield return new WaitForSeconds(3);
        Log("f1 WaitForSeconds");
        yield return new WaitUntil(() => _input);
        Log("f1 WaitUntil");
    }

    IEnumerator FP()
    {
        Log("fP start");
        yield return new WaitForEndOfFrame();
        Log("fP WaitForEndOfFrame");
        yield return new WaitForSeconds(3);
        Log("fP WaitForSeconds");
    }

    void Log(string s)
    {
        Debug.Log($"frame:{_frame} time:[{Time.time}]   {s}");
    }
}
