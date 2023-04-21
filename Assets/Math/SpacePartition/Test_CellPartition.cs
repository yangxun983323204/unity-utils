#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX.SpacePartition;

public class Test_CellPartition : MonoBehaviour
{
    public int xCnt = 4, yCnt = 4, zCnt = 4;
    public int xUnit = 1, yUnit = 1, zUnit = 1;

    CellPartition _cellPartition;
    PosAgent _agent;
    // Start is called before the first frame update
    void Start()
    {
        _cellPartition = new CellPartition();
        _cellPartition.CreateGrid(xCnt, yCnt, zCnt, xUnit, yUnit, zUnit);

        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "main";
        obj.GetComponent<Renderer>().material.color = Color.red;
        obj.transform.localScale = Vector3.one * 0.5f;
        _agent = obj.AddComponent<PosAgent>();
        _cellPartition.AddAgent(_agent);

        // add some static obj
        for (int i = 0; i < 4; i++)
        {
            var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            o.name = "static_"+i.ToString();
            o.GetComponent<Renderer>().material.color = Color.gray;
            o.transform.localScale = Vector3.one * 0.5f;
            var x = (Random.value - 0.5f) * xCnt * xUnit;
            var y = (Random.value - 0.5f) * yCnt * yUnit;
            var z = (Random.value - 0.5f) * zCnt * zUnit;
            o.transform.position = new Vector3(x, y, z);
            var a = o.AddComponent<PosAgent>();
            _cellPartition.AddAgent(a);
        }
        //
    }

    List<IAgent> _adjList = new List<IAgent>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            var x = (Random.value - 0.5f) * xCnt * xUnit;
            var y = (Random.value - 0.5f) * yCnt * yUnit;
            var z = (Random.value - 0.5f) * zCnt * zUnit;
            _agent.MoveTo(new Vector3(x, y, z));

            _cellPartition.GetAdjoin(_agent, ref _adjList);
            Debug.Log("agent的邻接对象:");
            foreach (var a in _adjList)
            {
                Debug.Log((a as MonoBehaviour).gameObject.name);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            _cellPartition.DrawGizmos();
    }
}
#endif