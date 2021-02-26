#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

namespace YX
{
    public class PauseOnClick : MonoBehaviour, IPointerClickHandler
    {
        public int DelayFrame = 0;

        public void OnPointerClick(PointerEventData eventData)
        {
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
