using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YX
{
    public class SequenceDetector
    {
        public bool Repeat = false;
        public UnityEvent onSuccess { get; } = new UnityEvent();
        public UnityEvent onFail { get; } = new UnityEvent();
        public UnityEvent<int,int> onDetecting { get; } = new UnityEvent<int, int>();

        public enum DetectState
        {
            Fail = 0,
            Wait,
            Success,
        }

        public interface IDetector
        {
            void Enter(IDetector pre);
            DetectState Update(IDetector pre,float secDelta);
            void Exit(IDetector pre);
        }


        private List<IDetector> _detectors = new List<IDetector>();
        private bool _running = false;
        private int _idx = -1;

        public void PushDetector(IDetector detector)
        {
            _detectors.Add(detector);
        }

        public void ClearDetector()
        {
            _detectors.Clear();
            _idx = -1;
        }

        public void Start()
        {
            _running = true;
            if (_idx == -1)
            {
                _idx = 0;
                if (_idx >= _detectors.Count)
                {
                    Debug.LogWarning("No Detectors!");
                    onSuccess.Invoke();
                    _running = false;
                    return;
                }

                _detectors[_idx].Enter(null);
            }
        }

        public void Update(float secDelta)
        {
            if (!_running)
                return;

            if (_idx>=_detectors.Count)
                return;

            var detector = _detectors[_idx];
            var s = detector.Update(Pre(), secDelta);
            switch (s)
            {
                case DetectState.Fail:
                    _detectors[_idx].Exit(Pre());
                    if (_idx!=0)
                        onFail.Invoke();

                    _idx = -1;
                    Start();
                    return;
                case DetectState.Wait:
                    break;
                case DetectState.Success:
                    onDetecting.Invoke(_idx, _detectors.Count);
                    if (_idx == _detectors.Count - 1)// 最后一个了
                    {
                        _detectors[_idx].Exit(Pre());
                        onSuccess.Invoke();
                        if (Repeat)
                        {
                            _idx = -1;
                            Start();
                        }
                        else
                            _running = false;
                    }
                    else
                    {
                        _detectors[_idx].Exit(Pre());
                        _idx++;
                        _detectors[_idx].Enter(Pre());
                    }
                    break;
                default:
                    break;
            }
        }

        private IDetector Pre()
        {
            return _idx == 0 ? null : _detectors[_idx - 1];
        }


        public class DetectorProxy : IDetector
        {
            public Action<IDetector> onEnter, onExit;
            public Func<IDetector, float, DetectState> onUpdate;
            public float Timeout = -1;

            private float _currTime = 0;

            public void Enter(IDetector pre)
            {
                _currTime = 0;
                onEnter?.Invoke(pre);
            }

            public void Exit(IDetector pre)
            {
                _currTime = 0;
                onExit?.Invoke(pre);
            }

            public DetectState Update(IDetector pre, float secDelta)
            {
                _currTime += secDelta;
                if (Timeout > 0 && _currTime >= Timeout)
                {
                    return DetectState.Fail;
                }

                if (onUpdate!=null)
                {
                    return onUpdate.Invoke(pre, secDelta);
                }

                return DetectState.Fail;
            }
        }
    }
}
