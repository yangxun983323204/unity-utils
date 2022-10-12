using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestMessageWnd : MonoBehaviour
{
    MessageWnd _msgWnd;

    private void Awake()
    {
        _msgWnd = gameObject.AddComponent<MessageWnd>();
        //_msgWnd.MsgShowCount = 100;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("我一在工奇葩械苛地");
        Debug.LogWarning("工觚 一头热人的人");
        Debug.LogError("人红又枯革 爱你的 的人的  的你  伯爱你的   革 在");
        
        _msgWnd.AddMsg(MessageWnd.TYPE_DEBUG, "哦是吗", "觚苦苦\n我的产不发");
        _msgWnd.AddMsg(MessageWnd.TYPE_WARNING, "哦是吗", "觚苦苦\n我的产不发");
        _msgWnd.AddMsg(MessageWnd.TYPE_ERROR, "哦是吗", "觚苦苦\n我的产不发");
        _msgWnd.AddMsg(MessageWnd.TYPE_CUSTOM, "哦是吗", "觚苦苦\n我的产不发");
        _msgWnd.AddMsg("不存在的", "哦是吗", "觚苦苦\n我的产不发");

        for (int i = 0; i < 100; i++)
        {
            Debug.Log("我一在工奇葩械苛地");
            Debug.LogWarning("工觚 一头热人的人");
            Debug.LogError("人红又枯革 爱你的 的人的  的你  伯爱你的   革 在");
        }
    }
}
