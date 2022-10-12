using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class ListAccessor<T>
    {
        public int Count { get { return List.Count; } }

        public IList<T> List { get; set; }

        public T this[int idx]
        {
            get { return List[idx]; }
            set { List[idx] = value; }
        }

        public ListAccessor()
        {
            List = new List<T>();
        }

        public ListAccessor(int capacity)
        {
            List = new List<T>(capacity);
        }

        public ListAccessor(IList<T> list)
        {
            List = list;
        }

        public void SSet(int idx,T val)
        {
            Reserve(idx + 1);
            this[idx] = val;
        }

        public void SetRange(int startIdx,int endIdx,T val)
        {
            for (int i = startIdx; i < endIdx; i++)
            {
                this[i] = val;
            }
        }

        public void Reserve(int cnt)
        {
            if (List.Count < cnt)
            {
                var addCnt = cnt - List.Count;
                for (int i = 0; i < addCnt; i++)
                {
                    List.Add(default(T));
                }
            }
        }
    }
}
