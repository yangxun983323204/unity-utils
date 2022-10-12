using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class GameObjectAllocator : IObjectAllocator<GameObject>
    {
        public GameObject CacheRoot
        {
            get { return _cacheRoot.gameObject; }
        }
        protected Transform _cacheRoot = null;
        protected GameObject _template = null;

        public GameObjectAllocator()
        {
            var obj = new GameObject("GameObjectPool_" + System.Guid.NewGuid().ToString());
            obj.SetActive(false);
            _cacheRoot = obj.transform;
            _cacheRoot.position = new Vector3(0, -1000, 0);
        }

        public GameObjectAllocator(GameObject root)
        {
            _cacheRoot = root.transform;
        }

        public virtual void SetTemplate(GameObject template)
        {
            _template = template;
        }

        public virtual GameObject Clone()
        {
            return Object.Instantiate(_template);
        }

        public virtual void Destroy(GameObject inst)
        {
            if (Application.isPlaying)
                Object.Destroy(inst);
            else
                Object.DestroyImmediate(inst);
        }

        public virtual void OnClone(GameObject inst)
        {
            inst.transform.SetParent(_cacheRoot, false);
        }

        public virtual void OnRecycle(GameObject inst)
        {
            inst.transform.SetParent(_cacheRoot, false);
        }

        public virtual void OnSpawn(GameObject inst)
        {
            inst.transform.SetParent(null, false);
        }

        public virtual void Dispose()
        {
            if (Application.isPlaying)
                Object.Destroy(_cacheRoot.gameObject);
            else
                Object.DestroyImmediate(_cacheRoot.gameObject);
        }
    }
}
