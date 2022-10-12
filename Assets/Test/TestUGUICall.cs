using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUGUICall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn)
        {
            btn.onClick.AddListener(() => { Debug.Log("click"); });
        }
    }

    private void Test()
    {
        Debug.Log("Test~~~");
    }
}
