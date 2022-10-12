using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public interface IObjectAllocator<T>:System.IDisposable
    {
        T Clone();
        void OnClone(T inst);
        void OnSpawn(T inst);
        void OnRecycle(T inst);
        void Destroy(T inst);
    }
}
