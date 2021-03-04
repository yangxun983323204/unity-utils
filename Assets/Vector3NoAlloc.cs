using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class Vector3NoAlloc
    {
        public static void Add(ref Vector3 self,ref Vector3 other)
        {
            self.x += other.x;
            self.y += other.y;
            self.z += other.z;
        }

        public static void Sub(ref Vector3 self, ref Vector3 other)
        {
            self.x -= other.x;
            self.y -= other.y;
            self.z -= other.z;
        }

        public static void Scale(ref Vector3 self, float s)
        {
            self.x *= s;
            self.y *= s;
            self.z *= s;
        }

        public static void Cross(ref Vector3 self, ref Vector3 other)
        {
            var x = self.y * other.z - self.z * other.y;
            var y = self.z * other.x - self.x * other.z;
            var z = self.x * other.y - self.y * other.x;
            self.x = x;
            self.y = y;
            self.z = z;
        }
    }
}
