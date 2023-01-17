using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public static class LinkedListAsQueue
    {
        public interface IPriority
        {
            int Priority { get; }
        }

        /// <summary>
        /// 以优先级从大到小插入
        /// </summary>
        public static void Enqueue<T>(this LinkedList<T> list, T item) where T : IPriority
        {
            if (list.Count <= 0 || item.Priority <= list.Last.Value.Priority)
            {
                list.AddLast(item);
                return;
            }

            var i = list.First;
            while (i != null && i.Value.Priority >= item.Priority)
            {
                i = i.Next;
            }

            if (i == null)
                list.AddLast(item);
            else
                list.AddBefore(i, item);
        }
        /// <summary>
        /// 正常出队列
        /// </summary>
        public static T Dequeue<T>(this LinkedList<T> list)
        {
            if (list.Count <= 0)
                return default(T);

            var first = list.First.Value;
            list.RemoveFirst();
            return first;
        }
        /// <summary>
        /// 使优先级最低且最早入队列的元素出队
        /// </summary>
        public static T DequeueWithPriorityMin<T>(this LinkedList<T> list) where T : IPriority
        {
            if (list.Count <= 0)
                return default(T);

            var first = list.First;
            var last = list.Last;
            if (first.Value.Priority == last.Value.Priority)
            {
                list.RemoveFirst();
                return first.Value;
            }

            var p = last;
            var pp = p.Previous;

            while (pp != null && p.Value.Priority == pp.Value.Priority)
            {
                p = pp;
                pp = p.Previous;
            }

            list.Remove(p);
            return p.Value;
        }
    }
}
