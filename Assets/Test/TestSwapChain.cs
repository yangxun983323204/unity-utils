using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YX;
using Error = YX.Diagnostics.Assert;

public class TestSwapChain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TestSwapChain begin");

        var chain = new SwapChain<string>();

        chain.Clear();
        chain.AddFront("b1");
        chain.AddBack("b2");
        Debug.Assert(chain.GetFront() == "b1");
        Debug.Assert(chain.GetBack() == "b2");
        Error.Catch<System.IndexOutOfRangeException>(()=>chain.GetBack(1));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b2");
        Debug.Assert(chain.GetBack() == "b1");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(1));

        chain.Clear();
        chain.AddFront("b1");
        chain.AddBack("b2");
        chain.AddBack("b3");
        Debug.Assert(chain.GetFront() == "b1");
        Debug.Assert(chain.GetBack() == "b2");
        Debug.Assert(chain.GetBack(1) == "b3");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(2));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b2");
        Debug.Assert(chain.GetBack() == "b3");
        Debug.Assert(chain.GetBack(1) == "b1");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(2));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b3");
        Debug.Assert(chain.GetBack() == "b1");
        Debug.Assert(chain.GetBack(1) == "b2");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(2));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b1");
        Debug.Assert(chain.GetBack() == "b2");
        Debug.Assert(chain.GetBack(1) == "b3");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(2));

        chain.Clear();
        chain.AddBack("b1");
        chain.AddBack("b2");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetFront());
        Debug.Assert(chain.GetBack() == "b1");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(1));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b1");
        Debug.Assert(chain.GetBack() == "b2");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(1));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b2");
        Debug.Assert(chain.GetBack() == "b1");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(1));
        chain.Swap();
        Debug.Assert(chain.GetFront() == "b1");
        Debug.Assert(chain.GetBack() == "b2");
        Error.Catch<System.IndexOutOfRangeException>(() => chain.GetBack(1));

        Debug.Log("TestSwapChain end");
    }
}
