using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Bodardr.UI.Runtime
{
    [RequireComponent(typeof(UIView))]
    public class UIPanel : MonoBehaviour, ICancelHandler
    {
        private bool initialized = false;

        private UIView uiView;
        private List<UIView> childrenViews;

        private CanvasGroup canvasGroup;

        [SerializeField]
        [Tooltip(
            "Means it appears automatically and cannot be removed from the panel stack. Use for Main Menu Panels that you cannot cancel from.")]
        private bool isPersistent = false;

        [SerializeField]
        private bool useDeltaTime = true;

        [Header("Stack Details")]
        [FormerlySerializedAs("hideOnPush")]
        [SerializeField]
        private bool hideOnNewPanelPushed = false;

        [SerializeField]
        private bool hideLastPanel = false;

        [SpaceAttribute]
        [Header("Events")]
        [SerializeField]
        private UnityEvent PanelOpened = new();

        [SerializeField]
        private UnityEvent PanelClosed = new();

        [SerializeField]
        private SelectionStrategy selectionStrategy;

        [ShowIfEnum("selectionStrategy", (int)SelectionStrategy.Specified)]
        [SerializeField]
        private Selectable firstSelected = null;

        public bool IsPersistent => isPersistent;
        public bool HideOnNewPanelPushed => hideOnNewPanelPushed;
        public bool HideLastPanel => hideLastPanel;

        public event Action OnPanelOpened;
        public event Action OnPanelClosed;

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            if (isPersistent)
                Open();
        }

        private void Initialize()
        {
            if (initialized)
                return;

            canvasGroup = GetComponent<CanvasGroup>();
            uiView = GetComponent<UIView>();

            childrenViews = GetComponentsInChildren<UIView>(true).Where(x => !x.ExcludeFromPanel).ToList();
            childrenViews.Remove(uiView);

            initialized = true;
        }

        public void OpenNoPushToStack()
        {
            if (!initialized)
                Initialize();

            ShowInternal();
        }

        [ContextMenu("Open")]
        public void Open()
        {
            if (!initialized)
                Initialize();

            UIPanelStack.Push(this);
        }

        [ContextMenu("Close")]
        public void Close()
        {
            if (!initialized)
                Initialize();

            UIPanelStack.Remove(this);
        }

        internal void ShowInternal()
        {
            uiView.Show(useDeltaTime);

            foreach (var view in childrenViews)
                view.Show(useDeltaTime);

            PanelOpened.Invoke();
            OnPanelOpened?.Invoke();

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            switch (selectionStrategy)
            {
                case SelectionStrategy.Specified:
                    EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
                    break;
                case SelectionStrategy.FirstChild when transform.childCount > 0:
                    EventSystem.current.SetSelectedGameObject(transform.GetChild(0).gameObject);
                    break;
            }
        }

        internal void HideInternal(bool fromPush = false)
        {
            if (EventSystem.current)
                EventSystem.current.SetSelectedGameObject(null);
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            uiView.Hide(useDeltaTime);

            foreach (var view in childrenViews)
                view.Hide(useDeltaTime);

            PanelClosed.Invoke();
            OnPanelClosed?.Invoke();
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (isActiveAndEnabled)
                UIPanelStack.Pop();
        }

        private void OnDestroy()
        {
            UIPanelStack.Remove(this, true);
        }
    }

    public enum SelectionStrategy
    {
        FirstChild,
        None,
        Specified
    }
}