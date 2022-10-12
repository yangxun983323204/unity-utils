using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestLockCount : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var lck = new LockCount();
        lck.onLock = () =>{
            Debug.Log("lock!");
        };

        lck.onUnlock = () => {
            Debug.Log("unlock!");
        };

        uint id;

        id = lck.Lock();
        Debug.Assert(lck.Unlock(id));
        Debug.Assert(!lck.Unlock(id));

        id = lck.Lock();
        Debug.Assert(lck.IsLocked());
        Debug.Assert(lck.Count == 1);
        id = lck.Lock();
        Debug.Assert(lck.IsLocked());
        Debug.Assert(lck.Count == 2);
    }
}
