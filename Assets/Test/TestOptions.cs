using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOptions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var options = new YX.Options();
        options.InitWithFile("Assets/Test/TestOptions.xml");
    }
}
