using System;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class VoxelGenerator
    { 
        public static int CalcCount_Box(double xSize, double ySize, double zSize, double xUnit, double yUnit, double zUnit)
        {
            int xCnt = (int)Math.Floor(xSize / xUnit + 1);
            int yCnt = (int)Math.Floor(ySize / yUnit + 1);
            int zCnt = (int)Math.Floor(zSize / zUnit + 1);

            return xCnt * yCnt * zCnt;
        }

        public static List<Vector3> CreateBox(double xSize,double ySize,double zSize,double xUnit,double yUnit,double zUnit)
        {
            int xCnt = (int)Math.Floor(xSize / xUnit + 1);
            int yCnt = (int)Math.Floor(ySize / yUnit + 1);
            int zCnt = (int)Math.Floor(zSize / zUnit + 1);

            double xSpan;
            if (xCnt == 1)
                xSpan = 0;
            else
                xSpan = xSize / (xCnt - 1);

            double ySpan;
            if (yCnt == 1)
                ySpan = 0;
            else
                ySpan = ySize / (yCnt - 1);

            double zSpan;
            if (zCnt == 1)
                zSpan = 0;
            else
                zSpan = zSize / (zCnt - 1);

            double xStart = -xSpan * (xCnt / 2 - 1 + 0.5);
            double yStart = -ySpan * (yCnt / 2 - 1 + 0.5);
            double zStart = -zSpan * (zCnt / 2 - 1 + 0.5);

            var list = new List<Vector3>(xCnt + yCnt + zCnt);

            for (int z = 0; z < zCnt; z++)
            {
                for (int y = 0; y < yCnt; y++)
                {
                    for (int x = 0; x < xCnt; x++)
                    {
                        list.Add(new Vector3((float)(xStart + xSpan * x), (float)(yStart + ySpan * y), (float)(zStart + zSpan * z)));
                    }
                }
            }

            return list;
        }

        public static List<Vector3> CreateSphere(double r, double unit)
        {
            int halfCnt = (int)(r / unit);
            UnityEngine.Debug.Assert(halfCnt > 0);
            List<Vector3> list = new List<Vector3>((int)Mathf.Pow(halfCnt * 2,3));
            double r2 = r * r;
            for (int z = 0; z < halfCnt; z++)
            {
                for (int y = 0; y < halfCnt; y++)
                {
                    for (int x = 0; x < halfCnt; x++)
                    {
                        var dis2 = unit * x * unit * x + unit * y * unit * y + unit * z * unit * z;
                        if (dis2 < r2)
                        {
                            Permutation((float)unit * x, (float)unit * y, (float)unit * z, list);
                        }
                    }
                }
            }
            return list;
        }

        public static List<Vector3> CreateCapsule(double r, double h, double rUnit, double hUnit)
        {
            int rHalfCnt = (int)(r / rUnit);
            int hHalfCnt = ((int)(h / hUnit)) / 2;
            double r2 = r * r;
            UnityEngine.Debug.Assert(rHalfCnt > 0);
            var estimate = (hHalfCnt * 2 + rHalfCnt * 2) * Mathf.Pow(rHalfCnt * 2, 2);
            List<Vector3> list = new List<Vector3>((int)estimate);
            for (int z = 0; z < rHalfCnt; z++)
            {
                for (int x = 0; x < rHalfCnt; x++)
                {
                    double tx = x * rUnit;
                    double tz = z * rUnit;
                    if (tx*tx+tz*tz<r2)
                    {
                        // 圆柱形
                        for (int i = 0; i < hHalfCnt; i++)
                        {
                            Permutation((float)tx, (float)hUnit * i, (float)tz, list);
                        }
                    }
                }
            }

            for (int z = 0; z < rHalfCnt; z++)
            {
                for (int y = 0; y < rHalfCnt; y++)
                {
                    for (int x = 0; x < rHalfCnt; x++)
                    {
                        var dis2 = rUnit * x * rUnit * x + rUnit * y * rUnit * y + rUnit * z * rUnit * z;
                        if (dis2 < r2)
                        {
                            // 两端半球
                            Permutation((float)rUnit * x, (float)(rUnit * y + h / 2), (float)rUnit * z, list);
                        }
                    }
                }
            }
            
            return list;
        }

        public static float Scale(float baseValue,params double[] vs)
        {
            double min = double.MaxValue;
            for (int i = 0; i < vs.Length; i++)
            {
                min = Math.Min(min, vs[i]);
            }

            return (float)((double)baseValue / min);
        }

        public static List<Vector3> CreateBoxScaled(double xSize, double ySize, double zSize, int step,float baseVal,out float scale)
        {
            scale = Scale(baseVal, xSize, ySize, zSize);
            var x = scale * xSize;
            var y = scale * ySize;
            var z = scale * zSize;
            return CreateBox(x, y, z, x / step, y / step, z / step);
        }

        public static List<Vector3> CreateSphereScaled(double r, int step,float baseVal, out float scale)
        {
            scale = Scale(baseVal, r);
            return CreateSphere(r * scale, r * scale / step);
        }

        public static List<Vector3> CreateCapsuleScaled(double r, double h, int step, float baseVal, out float scale)
        {
            scale = Scale(baseVal, r, h);
            var sr = r * scale;
            var sh = h * scale;
            return CreateCapsule(sr, sh, sr / step, sh / step);
        }

        public static List<Vector3> CreateBoxEstimate(double xSize,double ySize,double zSize,int expCnt)
        {
            var min = Math.Min(xSize, ySize);
            min = Math.Min(min, zSize);
            var max = Math.Max(xSize, ySize);
            max = Math.Max(max, zSize);
            double mid;
            // 二分法逼近
            min = min / expCnt;
            mid = (min + max) / 2;
            int iter = 0;
            int iterMax = expCnt*2;
            while (Math.Abs(max-min)>0.000001f)
            {
                mid = (min + max) / 2;
                if (iter >= iterMax)
                    break;

                var e = CalcCount_Box(xSize, ySize, zSize, mid, mid, mid);
                if (e == expCnt)
                    break;
                else if (e > expCnt)// 如果以mid为格子大小，box划分出来的格子个数大于期望个数
                    min = mid;
                else
                    max = mid;

                iter++;
            }
            Debug.LogFormat("[CreateBoxEstimate] iter:{0} unit:{1}", iter.ToString(), mid.ToString());
            return CreateBox(xSize, ySize, zSize, mid, mid, mid);
        }
        /// <summary>
        /// 8个象限对应点全排列
        /// </summary>
        private static void Permutation(float x,float y,float z,List<Vector3> list)
        {
            var v0 = new Vector3(x, y, z);
            var v1 = new Vector3(x, y, -z);
            var v2 = new Vector3(x, -y, z);
            var v3 = new Vector3(x, -y, -z);

            var v4 = new Vector3(-x, y, z);
            var v5 = new Vector3(-x, y, -z);
            var v6 = new Vector3(-x, -y, z);
            var v7 = new Vector3(-x, -y, -z);

            list.Add(v0);
            list.Add(v1);
            list.Add(v2);
            list.Add(v3);
            list.Add(v4);
            list.Add(v5);
            list.Add(v6);
            list.Add(v7);
        }
    }
}
