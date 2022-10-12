#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

namespace YX
{
    public class PauseOnClick : MonoBehaviour, IPointerClickHandler
    {
        public int DelayFrame = 0;
        private Button _btn;

        void Start()
        {
            _btn = GetComponent<Button>();
            if (_btn!=null)
            {
                _btn.onClick.AddListener(BtnCall);
            }
        }

        void OnDestroy()
        {
            if (_btn != null)
            {
                _btn.onClick.RemoveListener(BtnCall);
            }
        }

        private void BtnCall()
        {
            OnPointerClick(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (EditorApplication.isPaused)
                return;

            if (DelayFrame <= 0)
            {
                EditorApplication.isPaused = true;
            }
            else
            {
                StartCoroutine(DelayPause(DelayFrame));
            }
        }

        private IEnumerator DelayPause(int frame)
        {
            var f = new WaitForEndOfFrame();
            for (int i = 0; i < frame; i++)
            {
                yield return f;
                EditorApplication.isPaused = true;
            }
        }
    }
}
#endif
