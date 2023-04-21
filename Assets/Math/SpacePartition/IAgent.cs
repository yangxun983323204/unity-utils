using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX.SpacePartition
{
    public interface IAgent
    {
        void GetPosition(ref Vector3 pos);
        event Action<IAgent> onMoved;
        void Reset();
    }
}
