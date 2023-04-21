using System;
using System.Collections;
using System.Collections.Generic;

namespace YX.Async
{
    public class ProcessManager : IDisposable
    {
        LinkedList<Process> _processList = new LinkedList<Process>();

        public int UpdateProcesses(ulong deltaMs)
        {
            ushort successCount = 0;
            ushort failCount = 0;

            var it = _processList.First;
            while (it!=null)
            {
                var p = it.Value;
                var thisIt = it;
                it = it.Next;

                if (p.GetState() == Process.State.UnInitialized)
                    p.OnInit();

                if (p.GetState() == Process.State.Running)
                    p.OnUpdate(deltaMs);

                if (p.IsDead())
                {
                    switch (p.GetState())
                    {
                        case Process.State.Succeeded:
                            p.OnSuccess();
                            var child = p.RemoveChild();
                            if (child != null)
                                AttachProcess(child);
                            else
                                ++successCount;

                            break;
                        case Process.State.Failed:
                            p.OnFail();
                            ++failCount;
                            break;
                        case Process.State.Aborted:
                            p.OnAbort();
                            ++failCount;
                            break;
                        default:
                            break;
                    }

                    _processList.Remove(thisIt);
                    p.Dispose();
                }
            }

            return (successCount << 16) | failCount;
        }

        public void AttachProcess(Process p)
        {
            _processList.AddFirst(p);
        }

        public void AbortAllProcesses(bool immediate)
        {
            var it = _processList.First;
            while (it!=null)
            {
                var temp = it;
                it = it.Next;

                var p = temp.Value;
                if (p.IsAlive())
                {
                    p.SetState(Process.State.Aborted);
                    if (immediate)
                    {
                        p.OnAbort();
                        _processList.Remove(temp);
                    }
                }
            }
        }

        public int GetProcessCount()
        {
            return _processList.Count;
        }

        private void ClearAllProcesses()
        {
            _processList.Clear();
        }

        public void Dispose()
        {
            ClearAllProcesses();
        }
    }
}