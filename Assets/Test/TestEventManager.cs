using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestEventManager : MonoBehaviour
{
    public class Event1 : EventDataBase
    {
        public const ulong EvtType = 0;

        public override EventDataBase Clone()
        {
            throw new System.NotImplementedException();
        }

        public override ulong GetEventType()
        {
            return EvtType;
        }

        public override string GetName()
        {
            return "Event1";
        }
    }

    public class Event2 : EventDataBase
    {
        public const ulong EvtType = 1;

        public override EventDataBase Clone()
        {
            throw new System.NotImplementedException();
        }

        public override ulong GetEventType()
        {
            return EvtType;
        }

        public override string GetName()
        {
            return "Event2";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var mgr = new EventManager();
        mgr.AddListener(Event1.EvtType, (e) => 
        {
            Debug.LogFormat("收到事件{0}，在{1}帧",e.GetName(), Time.frameCount);
        });

        mgr.AddListener(Event2.EvtType, (e) =>
        {
            Debug.LogFormat("收到事件{0}，在{1}帧", e.GetName(), Time.frameCount);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventManager.Instance.QueueEvent(new Event1());
            Debug.LogFormat("入队事件1，在{0}帧",Time.frameCount);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventManager.Instance.QueueEvent(new Event2());
            Debug.LogFormat("入队事件2，在{0}帧", Time.frameCount);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            EventManager.Instance.QueueEvent(new Event1());
            Debug.LogFormat("触发事件1，在{0}帧", Time.frameCount);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            EventManager.Instance.QueueEvent(new Event1());
            Debug.LogFormat("触发事件2，在{0}帧", Time.frameCount);
        }

        EventManager.Instance.Update();
    }
}
