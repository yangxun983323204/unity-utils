using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace YX
{
    public class SelectableData : UIBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
    {
        [Tooltip("是否能触发选中态")]
        [SerializeField]
        private bool m_Interactable = true;

        [Tooltip("Text是直接改Text.Color而非CanvasRender.Color")]
        [SerializeField]
        internal ColorBlock m_Colors = ColorBlock.defaultColorBlock;

        [SerializeField]
        internal SpriteState m_SpriteState;

        [SerializeField]
        internal Graphic m_TargetGraphic;

        [Tooltip("是否将事件转发给SelectableGroup，仅控制SeletableData自身的转发")]
        [SerializeField]
        private bool m_EventForward = true;

        internal SelectableGroup _group;

        public Image image
        {
            get { return m_TargetGraphic as Image; }
            set { m_TargetGraphic = value; }
        }

        public Text text
        {
            get { return m_TargetGraphic as Text; }
            set { m_TargetGraphic = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (m_TargetGraphic == null)
                m_TargetGraphic = GetComponent<Graphic>();
        }

        public virtual bool IsInteractable()
        {
            return m_GroupsAllowInteraction && m_Interactable;
        }

        #region 事件转发
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // Selection tracking
            if (IsInteractable() && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);

            EventForward(eventData, ExecuteEvents.pointerDownHandler);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            EventForward(eventData, ExecuteEvents.pointerUpHandler);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            EventForward(eventData, ExecuteEvents.pointerEnterHandler);// 虽然OnPointerEnter和OnPointerExit本来就会向parent转发，这里还是转发一下，以支持非父子节点的情况
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            EventForward(eventData, ExecuteEvents.pointerExitHandler);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            EventForward(eventData, ExecuteEvents.selectHandler);
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            EventForward(eventData, ExecuteEvents.deselectHandler);
        }

        void EventForward<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
        {
            if (m_EventForward && _group != null)
            {
                ExecuteEvents.Execute(_group.gameObject, eventData, functor);
            }
        }
        #endregion

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
            }
        }
    }
}
