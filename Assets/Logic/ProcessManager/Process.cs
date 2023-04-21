using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace YX.Async
{
    public abstract class Process:IDisposable
    {
        public enum State
        {
            UnInitialized = 0,
            Removed,

            Running,
            Paused,

            Succeeded,
            Failed,
            Aborted,
        }

        State _state;
        Process _child;

        public Process()
        {
            _state = State.UnInitialized;
        }

        public virtual void OnInit()
        {
            _state = State.Running;
        }

        public abstract void OnUpdate(ulong deltaMs);

        public virtual void OnSuccess(){ }

        public virtual void OnFail(){ }

        public virtual void OnAbort() { }
        
        public void Succeed() {
            System.Diagnostics.Debug.Assert(_state == State.Running || _state == State.Paused);
            _state = State.Succeeded;
        }

        public void Fail()
        {
            System.Diagnostics.Debug.Assert(_state == State.Running || _state == State.Paused);
            _state = State.Failed;
        }

        public void Pause()
        {
            if(_state == State.Running)
                _state = State.Paused;
        }

        public void UnPause()
        {
            if (_state == State.Paused)
                _state = State.Running;
        }

        public State GetState() { return _state; }

        public bool IsAlive() { return _state == State.Running || _state == State.Paused; }

        public bool IsDead() { return _state == State.Succeeded || _state == State.Failed || _state == State.Aborted; }

        public bool IsRemoved() { return _state == State.Removed; }

        public bool IsPaused() { return _state == State.Paused; }

        public void AttachChild(Process p)
        {
            if (_child != null)
                _child.AttachChild(p);
            else
                _child = p;
        }

        public Process RemoveChild()
        {
            var p = _child;
            _child = null;
            return p;
        }

        public Process PeekChild()
        {
            return _child;
        }

        public void SetState(State newState) { _state = newState; }

        public void Dispose()
        {
            if (_child!=null)
            {
                _child.OnAbort();
                _child.Dispose();
                _child = null;
            }
        }
    }
}
