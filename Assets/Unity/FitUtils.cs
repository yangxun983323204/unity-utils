using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class FitUtils
    {
        public enum FitType
        {
            X,
            Y,
            Z,
            Inside,
            Outside,
        }

        public static Vector2 Fit(this Vector2 inst,Vector2 tar, FitType type)
        {
            float instRate = inst.x / inst.y;
            float tarRate = tar.x / tar.y;
            bool isWider = instRate > tarRate;

            switch (type)
            {
                case FitType.X:
                    return new Vector2(tar.y / inst.y * inst.x, tar.y);
                case FitType.Y:
                    return new Vector2(tar.x, tar.x / inst.x * inst.y);
                case FitType.Inside:
                    if (isWider)
                        return new Vector2(tar.x, tar.x / instRate);
                    else
                        return new Vector2(tar.y * instRate, tar.y);
                case FitType.Outside:
                    if (isWider)
                        return new Vector2(tar.y * instRate, tar.y);
                    else
                        return new Vector2(tar.x, tar.x / instRate);
                default:
                    Debug.LogError("Vector2不支持的适应类型:"+type);
                    return inst;
            }
        }

        public static Vector2 Fit(this Vector2 inst, float tarX,float tarY, FitType type)
        {
            return inst.Fit(new Vector2(tarX, tarY),type);
        }
    }
}
