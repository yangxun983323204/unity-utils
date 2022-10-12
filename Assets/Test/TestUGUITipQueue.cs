using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestUGUITipQueue : MonoBehaviour
{
    UGUITipQueue _tipQueue;

    private void Awake()
    {
        _tipQueue = gameObject.AddComponent<UGUITipQueue>();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        _tipQueue.AddTip("1觚苦苦\n我的产不发");
        _tipQueue.AddTip("2觚苦苦\n我的产不发",-1);
        _tipQueue.AddTip("3觚苦苦\n我的产不发");
        _tipQueue.AddTip("4觚苦苦\n我的产不发");
        _tipQueue.AddTip("5觚苦苦\n我的产不发");
        _tipQueue.AddTip("6觚苦苦\n我的产不发", -1);
        _tipQueue.AddTip("7觚苦苦ad fasdfasfdsafdasfdasfsafasfadsfasd\n我的产不发", -1);
        yield return new WaitForSeconds(2);
        _tipQueue.FontSize = 20;
    }
}
