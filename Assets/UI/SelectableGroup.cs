using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YX
{
    public class SelectableGroup : UIBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
    {
        [Tooltip("Can the Selectable be interacted with?")]
        [SerializeField]
        private bool m_Interactable = true;

        [SerializeField]
        private SelectableData[] m_Items;

        private bool isPointerInside { get; set; }
        private bool isPointerDown { get; set; }
        private bool hasSelection { get; set; }

        protected SelectionState currentSelectionState
        {
            get
            {
                if (!IsInteractable())
                    return SelectionState.Disabled;
                if (isPointerDown)
                    return SelectionState.Pressed;
                if (hasSelection)
                    return SelectionState.Selected;
                if (isPointerInside)
                    return SelectionState.Highlighted;
                return SelectionState.Normal;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            foreach (var item in m_Items)
            {
                if(item!=null)
                    item._group = this;
            }
        }

        protected override void Start()
        {
            base.Start();
            DoStateTransition(currentSelectionState, true);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            EvaluateAndTransitionToSelectionState();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            hasSelection = true;
            EvaluateAndTransitionToSelectionState();
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            hasSelection = false;
            EvaluateAndTransitionToSelectionState();
        }

        private void OnSetProperty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DoStateTransition(currentSelectionState, true);
            else
#endif
                DoStateTransition(currentSelectionState, false);
        }

        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(currentSelectionState, false);
        }

        /// <summary>
        /// An enumeration of selected states of objects
        /// </summary>
        protected enum SelectionState
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled,
        }

        public virtual bool IsInteractable()
        {
            return m_GroupsAllowInteraction && m_Interactable;
        }

        protected virtual void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            foreach (var item in m_Items)
            {
                Color tintColor;
                Sprite transitionSprite;

                switch (state)
                {
                    case SelectionState.Normal:
                        tintColor = item.m_Colors.normalColor;
                        transitionSprite = null;
                        break;
                    case SelectionState.Highlighted:
                        tintColor = item.m_Colors.highlightedColor;
                        transitionSprite = item.m_SpriteState.highlightedSprite;
                        break;
                    case SelectionState.Pressed:
                        tintColor = item.m_Colors.pressedColor;
                        transitionSprite = item.m_SpriteState.pressedSprite;
                        break;
                    case SelectionState.Selected:
                        tintColor = item.m_Colors.selectedColor;
                        transitionSprite = item.m_SpriteState.selectedSprite;
                        break;
                    case SelectionState.Disabled:
                        tintColor = item.m_Colors.disabledColor;
                        transitionSprite = item.m_SpriteState.disabledSprite;
                        break;
                    default:
                        tintColor = Color.black;
                        transitionSprite = null;
                        break;
                }

                StartColorTween(item, tintColor * item.m_Colors.colorMultiplier, instant);
                DoSpriteSwap(item, transitionSprite);
            }
        }

        void StartColorTween(SelectableData item, Color targetColor, bool instant)
        {
            if (item.m_TargetGraphic == null)
                return;

            if (item.text != null)
                item.text.color = targetColor;
            else
                item.m_TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : item.m_Colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(SelectableData item, Sprite newSprite)
        {
            if (item.image == null)
                return;

            item.image.overrideSprite = newSprite;
        }

        private bool m_GroupsAllowInteraction = true;
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            // If our parenting changes figure out if we are under a new CanvasGroup.
            OnCanvasGroupChanged();
        }
        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
        protected override void OnCanvasGroupChanged()
        {
            // Figure out if parent groups allow interaction
            // If no interaction is alowed... then we need
            // to not do that :)
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(m_CanvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < m_CanvasGroupCache.Count; i++)
                {
                    // if the parent group does not allow interaction
                    // we need to break
                    if (!m_CanvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // if this is a 'fresh' group, then break
                    // as we should not consider parents
                    if (m_CanvasGroupCache[i].ignoreParentGroups)
                        shouldBreak = true;
                }
                if (shouldBreak)
                    break;

                t = t.parent;
            }

            if (groupAllowInteraction != m_GroupsAllowInteraction)
            {
                m_GroupsAllowInteraction = groupAllowInteraction;
                OnSetProperty();
            }
        }
    }
}
