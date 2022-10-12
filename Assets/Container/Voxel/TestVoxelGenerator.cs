#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using YX;

public class TestVoxelGenerator : MonoBehaviour
{
    public Vector3 c0 = new Vector3(0, 0, 0);
    public Vector3 c1 = new Vector3(0.1f, 0, 0);
    public Vector3 c2 = new Vector3(-0.1f, 0, 0);
    public Vector3 cExp = new Vector3(0, 0.1f, 0);
    public int exp = 14;
    public float _scale = 1;
    List<Vector3> l0, l1, l2, lExp;

    void Start()
    {
        float scale;
        l0 = VoxelGenerator.CreateBoxScaled(0.05f, 0.001f, 0.05f, 10, _scale, out scale);
        l1 = VoxelGenerator.CreateSphereScaled(0.02f, 10, _scale, out scale);
        l2 = VoxelGenerator.CreateCapsuleScaled(0.02f, 0.04f, 10, _scale, out scale);

        lExp = VoxelGenerator.CreateBoxEstimate(0.05f, 0.005f, 0.05f, exp);
        Debug.Log($"期望:{exp.ToString()}，生成:{lExp.Count}");

        //DrawPoint("box", l0, 0.01f, c0);
        //DrawPoint("sphere", l1, 0.01f, c1);
        //DrawPoint("capsule", l2, 0.01f, c2);
        DrawPoint("box_estimate", lExp, 0.01f, cExp);

        DrawPoint("box_test", VoxelGenerator.CreateBox(0.008, 0.0015, 0.008, 0.0015, 0.0015, 0.0015), 0.001f, cExp);
    }

    void DrawPoint(string name,List<Vector3> points, float r, Vector3 offset)
    {
        var root = new GameObject(name);
        root.transform.position = offset;

        for (int i = 0; i < points.Count; i++)
        {
            var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            o.name = i.ToString();
            o.transform.SetParent(root.transform);
            o.transform.localPosition = points[i];
            o.transform.localScale = Vector3.one * r;
        }
    }
}
#endif