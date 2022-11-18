using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace YX
{
    public class MeasureWnd : EditorWindow
    {
        [Serializable]
        public class Point
        {
            [Tooltip("优先使用")]
            public Transform tran;
            public Vector3 pos;
            public Vector3 offset;
            public bool localOffset;

            public Vector3 GetWPos()
            {
                if (tran != null)
                {
                    if (localOffset)
                        return tran.TransformPoint(offset);
                    else
                        return tran.position + offset;
                }
                else
                {
                    return pos + offset;
                }
            }
        }

        SerializedObject _serializedObj;

        Color _TextColor = Color.yellow;
        Color _PosTextColor = Color.green;
        Vector3[] _tempRect = new Vector3[4];
        List<Vector3> _tempList = new List<Vector3>(10);

        // 测量transform之间距离
        bool _measure = false;
        bool _measureIgnoreX = false;
        bool _measureIgnoreY = false;
        bool _measureIgnoreZ = false;
        [SerializeField]
        List<Point> _measureTrans = new List<Point>();
        SerializedProperty _measureTransSP;
        // 测量点与平面距离
        bool _measureProj = false;
        bool _showProjCoor = false;
        bool _measureProjDis = false;
        [SerializeField]
        List<Point> _projDots = new List<Point>();
        SerializedProperty _projDotsSP;
        Transform _projPlane;
        Plane _planeCache = new Plane();

        [MenuItem("Debug/测量工具")]
        public static void Open()
        {
            var wnd = GetWindow<MeasureWnd>("测量");
            wnd.Show();
            wnd.minSize = new Vector2(600, 400);
            wnd.maxSize = new Vector2(600, 400);
        }

        [MenuItem("GameObject/测量工具", priority = 0)]
        public static void Call()
        {
            Open();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            _serializedObj = new SerializedObject(this);
            _measureTransSP = _serializedObj.FindProperty("_measureTrans");
            _projDotsSP = _serializedObj.FindProperty("_projDots");
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        

        private void OnGUI()
        {
            _serializedObj.Update();
            DrawMeasureDistance();
            DrawMeasureProj();
            if (EditorGUI.EndChangeCheck())
            {
                _serializedObj.ApplyModifiedProperties();
            }
        }

        void DrawMeasureDistance()
        {
            GUILayout.BeginHorizontal();
            _measure = GUILayout.Toggle(_measure, "测距");
            _measureIgnoreX = GUILayout.Toggle(_measureIgnoreX, "忽略x轴");
            _measureIgnoreY = GUILayout.Toggle(_measureIgnoreY, "忽略y轴");
            _measureIgnoreZ = GUILayout.Toggle(_measureIgnoreZ, "忽略z轴");
            if (GUILayout.Button("清空", GUILayout.Width(50)))
                _measureTrans.Clear();

            if (GUILayout.Button("从选中", GUILayout.Width(50)))
            {
                _measureTrans = Selection.GetTransforms(SelectionMode.ExcludePrefab)
                    .Select((n,idx)=>new Point() { tran = n }).ToList();

                _measureTrans.Sort((a, b) => {
                    if (a == null)
                        return 1;
                    else if (b == null)
                        return -1;
                    return string.Compare(a.tran.gameObject.name, b.tran.gameObject.name);
                });
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_measureTransSP, true);
        }

        void DrawMeasureProj()
        {
            GUILayout.BeginHorizontal();
            _measureProj = GUILayout.Toggle(_measureProj, "测量点到平面距离");
            if(_measureProj)
                _showProjCoor = GUILayout.Toggle(_showProjCoor, "显示投影点坐标");

            if (_measureProj)
                _measureProjDis = GUILayout.Toggle(_measureProjDis, "测量投影点距离");

            if (GUILayout.Button("清空", GUILayout.Width(50)))
                _projDots.Clear();

            if (GUILayout.Button("从选中", GUILayout.Width(50)))
            {
                _projDots = Selection.GetTransforms(SelectionMode.ExcludePrefab)
                    .Select((n, idx) => new Point() { tran = n }).ToList();

                _projDots.Sort((a, b) => {
                    if (a == null)
                        return 1;
                    else if (b == null)
                        return -1;
                    return string.Compare(a.tran.gameObject.name, b.tran.gameObject.name);
                });
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_projDotsSP, true);
            _projPlane = EditorGUILayout.ObjectField("平面", _projPlane, typeof(Transform), true) as Transform;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.Repaint)
            {
                OnSceneGUIRepaint(sceneView);
            }
        }

        void OnSceneGUIRepaint(SceneView sceneView)
        {
            MeasureTransDistance(sceneView);
            MeasureProj(sceneView);
        }

        void MeasureTransDistance(SceneView sceneView)
        {
            if (_measure && _measureTrans.Count >= 2)
            {
                _tempList.Clear();
                _measureTrans.ForEach(t => _tempList.Add(t.GetWPos()));
                MeasurePsDistanceInternal(sceneView, _tempList);
            }
        }

        void MeasurePsDistanceInternal(SceneView sceneView, List<Vector3> vs)
        {
            if (vs.Count < 2)
                return;

            Vector3 a, b;
            float dis;
            float sum = 0;
            float circle = 0;
            Vector3 center = Vector3.zero;
            for (int i = 0; i < vs.Count - 1; i++)
            {
                a = vs[i];
                b = vs[i + 1];
                dis = MeasureDistance(a, b);
                sum += dis;
                circle += dis;
                center += a;
                Handles.DrawLine(a, b);
                HandleUtls.Label((a + b) / 2f, dis.ToString(), _TextColor);
                if (i == 0)
                    DrawMainPoint(a);
                else
                    DrawPoint(a);

                if(i == vs.Count-2)
                    DrawPoint(b);
            }

            if (vs.Count >= 3)
            {
                a = vs[vs.Count - 1];
                b = vs[0];
                dis = MeasureDistance(a, b);
                circle += dis;
                center += a;
                center /= vs.Count;
                Handles.DrawDottedLine(a, b, 4);
                HandleUtls.Label((a + b) / 2f, dis.ToString(), _TextColor);
                
                HandleUtls.GetScreenLine(center, Vector2.right, 300, ref _tempRect);
                Handles.DrawDottedLine(_tempRect[0], _tempRect[1], 1);
                HandleUtls.Label(_tempRect[1], $"总长度:{sum}\n环长度:{circle}", _TextColor);
            }
        }

        void MeasureProj(SceneView sceneView)
        {
            if (_measureProj && _projDots.Count>0 && _projPlane!=null)
            {
                _planeCache.SetNormalAndPosition(_projPlane.up, _projPlane.position);
                if (_measureProjDis)
                    _tempList.Clear();

                foreach (var _projDot in _projDots)
                {
                    var d = _projDot.GetWPos();
                    var proj = _planeCache.ClosestPointOnPlane(d);
                    var dis = Vector3.Distance(d, proj);

                    DrawMainPoint(d);
                    DrawPoint(proj);
                    HandleUtls.GetPlaneRect(_projPlane, 1, 1, ref _tempRect);
                    Handles.DrawSolidRectangleWithOutline(_tempRect, Color.green * 0.2f, Color.green);
                    Handles.DrawDottedLine(d, proj, 4);
                    HandleUtls.Label((d + proj) / 2f, dis.ToString(), _TextColor);
                    if(_showProjCoor)
                        HandleUtls.Label(proj, $"<{proj.x}, {proj.y}, {proj.z}>", _PosTextColor);

                    if (_measureProjDis)
                        _tempList.Add(proj);
                }

                if (_measureProjDis)
                {
                    MeasurePsDistanceInternal(sceneView, _tempList);
                }
            }
        }

        float MeasureDistance(Vector3 a,Vector3 b)
        {
            if (_measureIgnoreX)
            {
                a.x = 0;
                b.x = 0;
            }

            if (_measureIgnoreY)
            {
                a.y = 0;
                b.y = 0;
            }

            if (_measureIgnoreZ)
            {
                a.z = 0;
                b.z = 0;
            }

            return Vector3.Distance(a, b);
        }

        void DrawMainPoint(Vector3 wpos)
        {
            HandleUtls.GetScreenRect(wpos, 4, ref _tempRect);
            Handles.DrawSolidRectangleWithOutline(_tempRect, Color.red, Color.green);
        }

        void DrawPoint(Vector3 wpos)
        {
            HandleUtls.GetScreenRect(wpos, 4, ref _tempRect);
            Handles.DrawSolidRectangleWithOutline(_tempRect, Color.red * 0.2f, Color.green);
        }
    }
}
