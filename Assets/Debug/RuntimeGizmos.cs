using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class RuntimeGizmos
    {
        private const int SphereSliceCount = 24;
        private const int SphereStackCount = 24;

        public static Color color;
        public static Material mat;

        private static Vector3[] _sphereV = new Vector3[(SphereSliceCount - 1) * (SphereSliceCount + 1) + 2];
        private static int[] _sphereI = new int[SphereSliceCount * 3 + (SphereStackCount - 2) * SphereSliceCount * 6 + SphereSliceCount * 3];

        public static void DrawSphere(Vector3 center, float radius)
        {
            // 球体创建方法提取自DX11龙书
            float phiStep = Mathf.PI / SphereStackCount;
            float thetaStep = 2.0f * Mathf.PI / SphereSliceCount;

            _sphereV[0].x = center.x;
            _sphereV[0].y = radius + center.y;
            _sphereV[0].z = center.z;
            int vidx = 1;
            for (int i = 1; i <= SphereStackCount - 1; ++i)
            {
                float phi = i * phiStep;

                for (int j = 0; j <= SphereSliceCount; ++j)
                {
                    float theta = j * thetaStep;
                    _sphereV[vidx].x = radius * Mathf.Sin(phi) * Mathf.Cos(theta) + center.x;
                    _sphereV[vidx].y = radius * Mathf.Cos(phi) + center.y;
                    _sphereV[vidx].z = radius * Mathf.Sin(phi) * Mathf.Sin(theta) + center.z;
                    vidx++;
                }
            }

            _sphereV[_sphereV.Length - 1].x = center.x;
            _sphereV[_sphereV.Length - 1].y = -radius + center.y;
            _sphereV[_sphereV.Length - 1].z = center.z;

            int iidx = 0;
            for (int i = 1; i <= SphereSliceCount; ++i)
            {
                _sphereI[iidx++] = 0;
                _sphereI[iidx++] = i + 1;
                _sphereI[iidx++] = i;
            }

            int baseIndex = 1;
            int ringVertexCount = SphereSliceCount + 1;
            for (int i = 0; i < SphereStackCount - 2; ++i)
            {
                for (int j = 0; j < SphereSliceCount; ++j)
                {
                    _sphereI[iidx++] = (baseIndex + i * ringVertexCount + j);
                    _sphereI[iidx++] = (baseIndex + i * ringVertexCount + j + 1);
                    _sphereI[iidx++] = (baseIndex + (i + 1) * ringVertexCount + j);

                    _sphereI[iidx++] = (baseIndex + (i + 1) * ringVertexCount + j);
                    _sphereI[iidx++] = (baseIndex + i * ringVertexCount + j + 1);
                    _sphereI[iidx++] = (baseIndex + (i + 1) * ringVertexCount + j + 1);
                }
            }

            int southPoleIndex = _sphereV.Length - 1;

            baseIndex = southPoleIndex - ringVertexCount;

            for (int i = 0; i < SphereSliceCount; ++i)
            {
                _sphereI[iidx++] = (southPoleIndex);
                _sphereI[iidx++] = (baseIndex + i);
                _sphereI[iidx++] = (baseIndex + i + 1);
            }

            mat.color = color;
            mat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);
            for (int i = 0; i < _sphereI.Length; i++)
            {
                int vi = _sphereI[i];
                GL.Vertex(_sphereV[vi]);
            }
            GL.End();
        }

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            mat.color = color;
            mat.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(from);
            GL.Vertex(to);
            GL.End();
        }
    }
}