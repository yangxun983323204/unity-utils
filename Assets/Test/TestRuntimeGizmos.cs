using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestRuntimeGizmos : MonoBehaviour
{
    private void OnPostRender()
    {
        RuntimeGizmos.mat = new Material(Shader.Find("Unlit/Color"));
        RuntimeGizmos.color = Color.green;
        RuntimeGizmos.DrawSphere(new Vector3(0,0,0), 1);
        RuntimeGizmos.color = Color.red;
        RuntimeGizmos.DrawSphere(new Vector3(2,2,2), 0.5f);
        RuntimeGizmos.DrawLine(new Vector3(-2,-2,-2), new Vector3(-4, -2, -2));
    }
}
