using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class ImmergeCalculate
    {
        private static Vector3 _zero = Vector3.zero;
        private static List<Vector3> _tmpList = new List<Vector3>();

        public static bool IsInWater(Plane water,List<Vector3> voxel,out float ratio,out Vector3 center)
        {
            bool inWater = false;
            ratio = 0;
            center = _zero;
            //
            if (voxel!=null && voxel.Count>0)
            {
                _tmpList.Clear();
                for (int i = 0; i < voxel.Count; i++)
                {
                    var v = voxel[i];
                    if (!water.GetSide(v))
                    {
                        _tmpList.Add(v);
                    }
                }

                if (_tmpList.Count>0)
                {
                    inWater = true;
                    ratio = (float)_tmpList.Count / voxel.Count;
                    for (int i = 0; i < _tmpList.Count; i++)
                    {
                        center += _tmpList[i];
                    }
                    center /= _tmpList.Count;
                }
            }
            //
            return inWater;
        }
    }
}
