using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YX
{
    public class CoroutineDriver
    {
        public class CoroutineContextHandle
        {
            public uint Id { get; private set; }
            public CoroutineContextHandle(uint id) { Id = id; }
        }

        private class CoroutineContext
        {
            public uint id;
            public Stack<IEnumerator> stack;
            public int fixedUpdateCnt;
            public int updateCnt;
            public bool initFrame = true;
            public bool initFixFrame = true;
            public float frameTime;
            public float frameRealTime;
            public bool isDone;

            public object Current
            {
                get
                {
                    return stack.Peek().Current;
                }
            }

            public bool MoveNext()
            {
                if (stack.Count <= 0)
                    return false;

                bool s = false;
                bool pushed = false;
                while (!s && stack.Count>0)
                {
                    s = stack.Peek().MoveNext();
                    if (!s)
                    {
                        stack.Pop();
                        if (pushed)// 有压入新值，并且这个新值不能MoveNext，那么它本身作为yield值
                        {
                            break;
                        }
                    }
                    else
                    {
                        var c = stack.Peek().Current;
                        if (c is IEnumerator)
                        {
                            var cc = c as IEnumerator;
                            stack.Push(cc);
                            s = false;
                            pushed = true;
                        }
                        else
                            break;
                    }
                }

                s = stack.Count > 0;
                if (!s)
                    isDone = true;

                return s;
            }

            public override string ToString()
            {
                return id.ToString();
            }
        }

        private LinkedList<CoroutineContext> _ctxList = new LinkedList<CoroutineContext>();
        private Dictionary<Type, Func<bool>> _extraHandle;

        public void FixedUpdate()
        {
            var p = _ctxList.First;
            while (p!=null)
            {
                FixedUpdateCoroutine(p.Value);
                p = p.Next;
            }
        }

        public void Update()
        {
            var p = _ctxList.First;
            while (p != null)
            {
                UpdateCoroutine(p.Value);
                p = p.Next;
            }
        }

        public CoroutineContextHandle StartCoroutine(IEnumerator enumerator)
        {
            var inst = CreateNewCtx(enumerator);
            if(inst.MoveNext())
                _ctxList.AddLast(inst);

            return new CoroutineContextHandle(inst.id);
        }

        public void StopCoroutine(CoroutineContextHandle h)
        {
            if (_ctxList.Count <= 0)
                return;

            LinkedListNode<CoroutineContext> inst = FindNode(h.Id);
            if (inst != null)
                _ctxList.Remove(inst);
        }

        private uint _idx = 0;
        private CoroutineContext CreateNewCtx(IEnumerator enumerator)
        {
            var inst = new CoroutineContext();
            inst.id = _idx;
            _idx++;
            inst.stack = new Stack<IEnumerator>(4);
            inst.stack.Push(enumerator);
            inst.fixedUpdateCnt = 0;
            inst.updateCnt = 0;
            inst.frameTime = Time.time;
            inst.frameRealTime = Time.realtimeSinceStartup;
            return inst;
        }

        private void UpdateCoroutine(CoroutineContext ctx)
        {
            ctx.updateCnt++;
            var curr = ctx.Current;
            if (curr is WaitForEndOfFrame)
            {
                if (ctx.initFrame)
                    ctx.initFrame = false;
                else
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
            }
            else if (curr is WaitForSeconds)
            {
                if (CheckWaitForSecondsDone(ctx,curr as WaitForSeconds))
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
            }
            else if (curr is WaitForSecondsRealtime)
            {
                if (Time.realtimeSinceStartup - ctx.frameRealTime >= (curr as WaitForSecondsRealtime).waitTime)
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
            }
            else if (curr is AsyncOperation)
            {
                if ((curr as AsyncOperation).isDone)
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
            }
            else if (curr is CustomYieldInstruction)
            {
                if (!(curr as CustomYieldInstruction).keepWaiting)
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
            }
            else if (curr is CoroutineContextHandle)
            {
                var node = FindNode((curr as CoroutineContextHandle).Id);
                if (node == null)
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
                else if (node.Value.isDone)
                {
                    _ctxList.Remove(ctx);
                }
            }
            else if (curr is WaitForFixedUpdate)
            {
            }
            else if (curr == null || curr.GetType().IsValueType)
            {
                var s = ctx.MoveNext();
                if (!s)
                    _ctxList.Remove(ctx);
            }
            else
            {
                Debug.LogError("不支持的yield类型:" + curr.GetType().ToString());
            }

            ctx.initFrame = false;
            if (!(curr is WaitForSeconds))
                ctx.frameTime = Time.time;

            if (!(curr is WaitForSecondsRealtime))
                ctx.frameRealTime = Time.realtimeSinceStartup;
        }

        private void FixedUpdateCoroutine(CoroutineContext ctx)
        {
            ctx.fixedUpdateCnt++;
            var curr = ctx.Current;
            if (curr is WaitForFixedUpdate)
            {
                if (ctx.initFixFrame)
                    ctx.initFixFrame = false;
                else
                {
                    var s = ctx.MoveNext();
                    if (!s)
                        _ctxList.Remove(ctx);
                }
            }

            ctx.initFixFrame = false;
        }

        private LinkedListNode<CoroutineContext> FindNode(uint id)
        {
            LinkedListNode<CoroutineContext> inst = null;
            var p = _ctxList.First;
            while (p != null)
            {
                if (p.Value.id == id)
                {
                    inst = p;
                    break;
                }

                p = p.Next;
            }

            return inst;
        }


        static FieldInfo _wfs_sec = null;

        private static bool CheckWaitForSecondsDone(CoroutineContext ctx,WaitForSeconds w)
        {
            if (_wfs_sec == null)
            {
                var _wfs = typeof(WaitForSeconds);
                _wfs_sec = _wfs.GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            var sec = (float)_wfs_sec.GetValue(w);
            return (Time.time - ctx.frameTime) >= sec;
        }
    }
}
