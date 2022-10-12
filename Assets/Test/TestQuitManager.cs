using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;
using System.IO;
using System.Text;

public class TestQuitManager : MonoBehaviour
{
    public SimpleLog.Stamp StampType = SimpleLog.Stamp.Unity;
    public float WaitTime = 3;
    QuitManager _mgr;
    SimpleLog _log;
    // Start is called before the first frame update
    void Start()
    {
        _mgr = new QuitManager(WaitTime);
        _mgr.onBeginQuit += Task0;
        _mgr.onBeginQuit += Task1;
        _mgr.onWillFinallyQuit += OnQuit;
        _mgr.SetQuitFunc(QuitFunc);

        _log = new SimpleLog(".","log");
        _log.logEnabled = true;
        _log.StampType = StampType;
        _log.StackDepth = 5;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _log.StampType = StampType;
            _log.Log("请求退出");
            _mgr.Quit();
        }

        _mgr.Update();
    }

    private void QuitFunc()
    {
        if (Application.isEditor)
        {
            _log.Log("我是Editor的Quit!!!!!!!!!!");
            _mgr.Reset();
        }
        else
            Application.Quit();
    }

    private void OnQuit()
    {
        _log.Log("退出");
        _log.Close();
    }

    private void Task0()
    {
        StartCoroutine(T0Impl());
    }

    private IEnumerator T0Impl()
    {
        _log.Log("Task0 begin");
        var id = _mgr.Lock();
        yield return new WaitForSeconds(1);
        _mgr.Unlock(id);
        _log.Log("Task0 end");
    }

    private void Task1()
    {
        StartCoroutine(T1Impl());
    }

    private IEnumerator T1Impl()
    {
        _log.Log("Task1 begin");
        var id = _mgr.Lock();
        yield return new WaitForSeconds(2);
        _mgr.Unlock(id);
        _log.Log("Task1 end");
    }
}
