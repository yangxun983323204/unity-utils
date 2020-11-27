using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestStrEx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("啊哒~~~~".Dye(Color.red));
        Debug.Log("啊哒~~~~".Dye(0,0.5f,0.5f,1));
    }
}
