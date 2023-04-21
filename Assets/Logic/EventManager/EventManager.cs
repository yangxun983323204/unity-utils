using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    using EventListenerList = LinkedList<System.Action<YX.EventDataBase>>;
    using EventListenerMap = DictAccessor<ulong, LinkedList<System.Action<YX.EventDataBase>>>;
    using EventQueue = LinkedList<EventDataBase>;

    public class EventManager
    {
        private const int BUFFER_COUNT = 2;
        private EventListenerMap _evtListenerMap = new EventListenerMap();
        private EventQueue[] _evtQueue = new EventQueue[BUFFER_COUNT];
        private int _activeQueueIdx = 0;

        private System.Func<ulong, EventListenerList> _listCtor;

        public static EventManager Instance { get; private set; }

        public EventManager()
        {
            if (Instance != null)
                Instance.Abandon();

            Instance = this;
            _listCtor = (type) => { return new EventListenerList(); };
            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                _evtQueue[i] = new EventQueue();
            }
        }

        public bool AddListener(ulong type,System.Action<EventDataBase> callback)
        {
            var listeners = _evtListenerMap.GetOrNew(type, _listCtor);
            if (!listeners.Contains(callback))
            {
                listeners.AddLast(callback);
                return true;
            }

            return false;
        }

        public bool RemoveListener(ulong type, System.Action<EventDataBase> callback)
        {
            var listeners = _evtListenerMap.Get(type, null);
            if (listeners == null)
                return false;

            var node = listeners.Find(callback);
            if (node == null)
                return false;

            listeners.Remove(node);
            return true;
        }

        public bool TriggerEvent(EventDataBase evt)
        {
            bool processed = false;
            var listeners = _evtListenerMap.Get(evt.GetEventType(), null);
            if (listeners != null)
            {
                var node = listeners.First;
                while (node != null)
                {
                    node.Value.Invoke(evt);
                    node = node.Next;
                    processed = true;
                }
            }

            return processed;
        }

        public bool QueueEvent(EventDataBase evt)
        {
            if (evt == null)
                return false;

            var queue = _evtQueue[_activeQueueIdx];
            queue.AddLast(evt);
            return true;
        }

        public bool AbortEvent(ulong type,bool all=false)
        {
            bool hasEvt = false;
            var queue = _evtQueue[_activeQueueIdx];
            var curr = queue.First;
            while (curr!=null)
            {
                if(curr.Value.GetEventType() == type)
                {
                    hasEvt = true;
                    var node = curr;
                    curr = curr.Next;
                    queue.Remove(node);
                    if (!all)
                    {
                        break;
                    }
                }
            }

            return hasEvt;
        }

        public bool Update(long maxMs = -1)
        {
            var time0 = System.DateTime.Now.Ticks;
            // 交换缓冲
            var queueToProcess = _evtQueue[_activeQueueIdx];
            _activeQueueIdx = (_activeQueueIdx + 1) % BUFFER_COUNT;
            _evtQueue[_activeQueueIdx].Clear();
            //
            var curr = queueToProcess.First;
            while (curr != null)
            {
                var node = curr;
                curr = curr.Next;
                queueToProcess.Remove(node);
                TriggerEvent(node.Value);
                long cost = (System.DateTime.Now.Ticks - time0)/10000;
                if (maxMs > 0 && cost >= maxMs)// 如果耗时大于指定时间，不再处理剩下的事件
                    break;
            }

            bool flushed = queueToProcess.Count == 0;
            if (!flushed)// 如果有未处理事件，移到激活缓冲队列的前面
            {
                curr = queueToProcess.Last;
                while (curr!=null)
                {
                    var node = curr;
                    curr = curr.Previous;
                    queueToProcess.Remove(node);
                    _evtQueue[_activeQueueIdx].AddFirst(node);
                }
            }

            return flushed;
        }

        private void Abandon()
        {
            _evtListenerMap = null;
            _evtQueue = null;
        }
    }
}
