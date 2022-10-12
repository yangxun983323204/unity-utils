using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class QuitManager
    {
        private enum State {
            Running,
            WaitQuit,
            Quit,
            Shut,
        }

        public static QuitManager Instance { get; private set; }
        /// <summary>
        /// 当开始退出流程，可以在这个事件里调用Lock方法阻止真正的退出直到调用Unlock
        /// </summary>
        public event Action onBeginQuit;
        /// <summary>
        /// 当即将执行真正退出操作
        /// </summary>
        public event Action onWillFinallyQuit;

        private State _currState = State.Running;
        private float _maxWaitTime;
        private float _waitTime = 0;

        private int _waitCnt = 0;
        private HashSet<uint> _waitIds = new HashSet<uint>();
        private uint _genId = 0;
        /// <summary>
        /// 真正退出方法
        /// </summary>
        private Action _quitFunc = null;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="maxWaitSec">在执行退出方法前最多等待秒数</param>
        public QuitManager(float maxWaitSec)
        {
            if (Instance!=null)
            {
                throw new System.NotSupportedException("QuitManager不允许多次实例化");
            }

            Instance = this;
            _maxWaitTime = maxWaitSec;
        }

        public void Quit()
        {
            TryQuit();
        }

        /// <summary>
        /// 需外部不断调用这个方法以驱动逻辑
        /// </summary>
        public void Update()
        {
            switch (_currState)
            {
                case State.Running:
                    break;
                case State.WaitQuit:
                    if (_waitCnt<=0 || WaitExpired())// 如果外部模块全部Unlock或达到最大等待时间，进入真正退出状态
                    {
                        _currState = State.Quit;
                    }
                    break;
                case State.Quit:
                    if (onWillFinallyQuit != null)
                    {
                        try
                        {
                            onWillFinallyQuit();
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogException(e);
                        }
                    }
                    _currState = State.Shut;
                    FinallyQuit();
                    break;
                case State.Shut:
                    break;
            }
        }

        public uint Lock()
        {
            var i = _genId;
            _genId++;
            _waitCnt++;
            _waitIds.Add(i);
            return i;
        }

        public void Unlock(uint id)
        {
            if (_waitIds.Contains(id))
            {
                _waitIds.Remove(id);
                _waitCnt--;
            }
        }

        public void SetQuitFunc(Action action)
        {
            _quitFunc = action;
        }

        public void Reset()
        {
            _waitIds.Clear();
            _currState = State.Running;
        }

        public void Release()
        {
            if (Instance == this)
                Instance = null;

            _waitIds.Clear();
            _currState = State.Shut;
        }

        private bool TryQuit()
        {
            switch (_currState)
            {
                case State.Running:// 当在正常状态下收到了退出请求
                    if (onBeginQuit != null)
                    {
                        try
                        {
                            onBeginQuit();// 外部模块可以这里Lock以增加_waitCnt
                        }
                        catch(Exception e)
                        {
                            UnityEngine.Debug.LogException(e);
                        }
                    }

                    if (_waitCnt <= 0)
                    {
                        _currState = State.Quit;
                        return true;
                    }
                    else// 如果存在外部模块Lock了，等待它完成
                    {
                        _currState = State.WaitQuit;
                        _waitTime = 0;
                        return false;
                    }
                case State.WaitQuit:
                    return false;
                case State.Quit:
                    return true;
                default:
                    return true;
            }
        }

        private bool WaitExpired()
        {
            if (_maxWaitTime <= 0)
                return false;

            _waitTime += Time.deltaTime;
            if (_waitTime >= _maxWaitTime)
                return true;
            else
                return false;
        }

        private void FinallyQuit()
        {
            if (_quitFunc != null)
                _quitFunc();
            else
                Application.Quit();
        }
    }
}
