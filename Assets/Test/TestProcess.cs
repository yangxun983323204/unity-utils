using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX.Async;

public class TestProcess : MonoBehaviour
{
    ProcessManager _mgr = new ProcessManager();
    // Start is called before the first frame update
    void Start()
    {
        var p1 = new TestProcess1();
        var p2 = new TestProcess2();
        p1.AttachChild(p2);
        _mgr.AttachProcess(p1);
    }

    // Update is called once per frame
    void Update()
    {
        _mgr.UpdateProcesses(30);
    }


    private void OnDestroy()
    {
        _mgr.Dispose();
    }

    public class TestProcess1 : Process
    {

        public override void OnInit()
        {
            base.OnInit();
            Debug.Log("p1 init");
        }

        public override void OnUpdate(ulong deltaMs)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                Succeed();
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                Fail();
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                SetState(State.Aborted);
            }
        }

        public override void OnSuccess()
        {
            Debug.Log("p1 success");
        }

        public override void OnFail()
        {
            Debug.Log("p1 fail");
        }

        public override void OnAbort()
        {
            Debug.Log("p1 abort");
        }
    }

    public class TestProcess2 : Process
    {
        public override void OnInit()
        {
            base.OnInit();
            Debug.Log("p2 init");
        }

        public override void OnUpdate(ulong deltaMs)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                Succeed();
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                Fail();
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                SetState(State.Aborted);
            }
        }

        public override void OnSuccess()
        {
            Debug.Log("p2 success");
        }

        public override void OnFail()
        {
            Debug.Log("p2 fail");
        }

        public override void OnAbort()
        {
            Debug.Log("p2 abort");
        }
    }
}
