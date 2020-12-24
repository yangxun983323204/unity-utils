using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class GameObjectEx
    {
        public static T Create<T>(GameObject gameObject) where T : MonoBehaviour
        {
            if (gameObject == null)
            {
                gameObject = new GameObject(typeof(T).Name);
            }

            return gameObject.AddComponent<T>();
        }
    }
}
