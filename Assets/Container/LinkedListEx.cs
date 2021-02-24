using System;
using System.Collections;
using System.Collections.Generic;

namespace YX
{
    public class LinkedListEx<T> : LinkedList<T>
    {
        public T Find(Predicate<T> match)
        {
            var p = First;
            while (p!=null)
            {
                if (match(p.Value))
                    return p.Value;

                p = p.Next;
            }

            return default(T);
        }

        public void Foreach(System.Action<T> callback)
        {
            var p = First;
            while (p != null)
            {
                var tmp = p;
                p = p.Next;
                callback(tmp.Value);
            }
        }
    }
}
