using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollisionIgnoreChecker))]
public class CollisionIgnoreCheckerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("检查"))
        {
            (target as CollisionIgnoreChecker).CheckIsIgnore();
        }
    }
}
