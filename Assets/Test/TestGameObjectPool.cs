using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using YX;

public class TestGameObjectPool:MonoBehaviour
{
    void Start()
    {
        Test();
    }

    public IEnumerator Test()
    {
        var pool = new Pool<GameObject>();
        var tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var allc = new GameObjectAllocator();
        allc.SetTemplate(tmp);
        pool.SetAllocator(allc);
        yield return new WaitForSeconds(1);
        //
        var o1 = pool.Spawn();
        Assert.IsNotNull(o1);
        yield return new WaitForSeconds(1);
        pool.Recycle(o1);
        yield return new WaitForSeconds(1);
        pool.Reserve(10);
        Assert.AreEqual(10, pool.Count);
        yield return new WaitForSeconds(3);
        o1 = pool.Spawn();
        Assert.IsNotNull(o1);
        Assert.AreEqual(9, pool.Count);
        yield return new WaitForSeconds(1);
        pool.Clear();
        Assert.AreEqual(0, pool.Count);
        yield return new WaitForSeconds(3);
        pool.Dispose();
        yield return new WaitForSeconds(3);
    }
}
