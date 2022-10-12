using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX
{
    public class CoroutineObj
    {
        private Func<IEnumerator, Coroutine> _startFunc = null;
        private Action<Coroutine> _stopFunc = null;
        private Coroutine _cor = null;

        public void SetRunner(MonoBehaviour mono)
        {
            SetRunner(mono.StartCoroutine,mono.StopCoroutine);
        }

        public void SetRunner(Func<IEnumerator, Coroutine> startFunc,Action<Coroutine> stopFunc)
        {
            _startFunc = startFunc;
            _stopFunc = stopFunc;
        }

        private IEnumerator _target;
        public void StartCoroutine(IEnumerator routine)
        {
            StopCoroutine();
            _target = routine;
            _cor = _startFunc(CoroutineWrap());
        }

        public void StopCoroutine()
        {
            if (_cor != null)
            {
                _stopFunc(_cor);
                _cor = null;
            }
        }

        public bool IsRunning()
        {
            return _cor != null;
        }

        private IEnumerator CoroutineWrap()
        {
            yield return _target;
            _target = null;
            _cor = null;
        }
    }
}
