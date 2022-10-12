using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestTipQueue : MonoBehaviour
{
    TipQueue _tipQueue;

    private void Awake()
    {
        _tipQueue = gameObject.AddComponent<TipQueue>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _tipQueue.AddTip("1觚苦苦\n我的产不发");
        _tipQueue.AddTip("2觚苦苦\n我的产不发",-1);
        _tipQueue.AddTip("3觚苦苦\n我的产不发");
        _tipQueue.AddTip("4觚苦苦\n我的产不发");
        _tipQueue.AddTip("5觚苦苦\n我的产不发");
    }
}
