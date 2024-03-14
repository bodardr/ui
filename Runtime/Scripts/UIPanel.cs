using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Bodardr.UI.Runtime
{
    [RequireComponent(typeof(UIView))]
    public class UIPanel : MonoBehaviour, ICancelHandler
    {
        [SerializeField]
        private bool showOnStart = false;

        [SerializeField]
        [Tooltip(
            "If true, it cannot be removed from the panel stack. Use for Main Menu Panels that you cannot cancel from.")]
        private bool isPersistent = false;

        [FormerlySerializedAs("useDeltaTime")]
        [SerializeField]
        private bool useTimeScale = true;

        [Header("Stack Details")]
        [FormerlySerializedAs("hideOnPush")]
        [SerializeField]
        private bool hideOnNewPanelPushed = false;

        [SerializeField]
        private bool hideLastPanel = false;

        [Space]
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

        private CanvasGroup canvasGroup;
        private List<UIView> childrenViews;
        private bool initialized = false;
        private bool isOpen = false;

        private UIView uiView;

        public bool IsPersistent => isPersistent;
        public bool HideOnNewPanelPushed => hideOnNewPanelPushed;
        public bool HideLastPanel => hideLastPanel;

        public bool IsOpen => isOpen;
        public UIView UIView => uiView;

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            if (showOnStart)
                Open();
        }

        private void OnDestroy()
        {
            UIPanelStack.Remove(this, true);
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (isActiveAndEnabled)
                UIPanelStack.Pop();
        }

        public event Action OnPanelOpened;
        public event Action OnPanelClosed;

        private void Initialize()
        {
            if (initialized)
                return;

            canvasGroup = GetComponent<CanvasGroup>();
            uiView = GetComponent<UIView>();

            childrenViews = GetComponentsInChildren<UIView>(true).Where(x => !x.ExcludeFromPanel).ToList();
            childrenViews.Remove(UIView);

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

            if (!UIPanelStack.Remove(this))
                HideInternal();
        }

        internal void ShowInternal()
        {
            if (UIView)
                UIView.Show(useTimeScale);

            foreach (var view in childrenViews)
                if (view)
                    view.Show(useTimeScale);

            PanelOpened.Invoke();
            OnPanelOpened?.Invoke();

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            isOpen = true;

            if (!EventSystem.current)
            {
                Debug.LogWarning(
                    "<b>UIPanel</b> : No EventSystem has been found, so no selection override will be performed");
                return;
            }

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

            UIView.Hide(useTimeScale);

            isOpen = false;

            foreach (var view in childrenViews)
                view.Hide(useTimeScale);

            PanelClosed.Invoke();
            OnPanelClosed?.Invoke();
        }

        public void ToggleVisibility()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }
    }

    public enum SelectionStrategy
    {
        FirstChild,
        None,
        Specified
    }
}