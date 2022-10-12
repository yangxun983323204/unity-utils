using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX.UGUI;

public class TestKeyboard : MonoBehaviour
{
    public Keyboard KB;
    // Start is called before the first frame update
    void Start()
    {
        KB.onInputChar.AddListener(c => { Debug.LogFormat("输入:{0}", c); });
        KB.onInputDel.AddListener(() => { Debug.Log("删除"); });
        KB.onInputClear.AddListener(() => { Debug.Log("清空"); });
        KB.onInputSubmit.AddListener(() => { Debug.Log("确定"); });
    }
}
