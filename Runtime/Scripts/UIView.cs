using System;
using System.Collections;
using Bodardr.Utility.Runtime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Bodardr.UI
{
    [AddComponentMenu("UI/UI View")]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class UIView : MonoBehaviour
    {
        private enum StartState
        {
            Nothing,
            Show,
            Hide,
        }

        private Coroutine hideAfterDelayCoroutine;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas canvas;

        private Tween currentTween;
        private bool shown;

        [Header("Enabling / Disabling")]
        [FormerlySerializedAs("onEnableCallsShow")]
        [SerializeField]
        private StartState onStart = StartState.Nothing;

        [SerializeField]
        private bool controlsSetActive = true;

        [SerializeField]
        private bool excludeFromPanel = false;

        [Header("Animation")]
        [SerializeField]
        private TweenData showAnimation = new()
        {
            value = new Vector2(0, 1)
        };

        [SerializeField]
        private TweenData hideAnimation = new()
        {
            value = new Vector2(1, 0)
        };

        [SerializeField]
        private bool hideAfterDelay = false;

        [ShowIf(nameof(hideAfterDelay))]
        [SerializeField]
        private float hideDelay;

        [FormerlySerializedAs("onShow")]
        [Header("Events")]
        [SerializeField]
        private UnityEvent onShowStarted;

        [FormerlySerializedAs("onHide")]
        [SerializeField]
        private UnityEvent onHideStarted;

        public UnityEvent OnShowStarted => onShowStarted;
        public UnityEvent OnHideStarted => onHideStarted;
        
        public event Action OnShowCompleted;
        public event Action OnHideCompleted;

        private Tween ShowTween
        {
            get
            {
                var showTween = showAnimation.GetTweenFrom(rectTransform, canvas, canvasGroup, TweenAnimType.Override);
                showTween.OnComplete(() =>
                {
                    OnShowCompleted?.Invoke();
                    
                    if (hideAfterDelay)
                        hideAfterDelayCoroutine = StartCoroutine(HideAfterDelayCoroutine());
                });
                return showTween;
            }
        }

        private Tween HideTween
        {
            get
            {
                var hideTween = hideAnimation.GetTweenFrom(rectTransform, canvas, canvasGroup, TweenAnimType.Override);
                hideTween.OnComplete(() =>
                {
                    OnHideCompleted?.Invoke();
                    
                    if (controlsSetActive)
                        gameObject.SetActive(false);
                });

                return hideTween;
            }
        }

        public bool IsShown => shown;

        public bool IsHidden => !shown;

        public bool ExcludeFromPanel => excludeFromPanel;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();

            InitializeTweens();
        }
        public void InitializeTweens()
        {

            showAnimation.Initialize(rectTransform);
            hideAnimation.Initialize(rectTransform);
        }

        private void Start()
        {
            switch (onStart)
            {
                case StartState.Show:
                    Show(true, true);
                    break;
                case StartState.Hide:
                    InstantHide();
                    break;
                case StartState.Nothing:
                    shown = gameObject.activeSelf;
                    break;
            }
        }

        [ContextMenu("Show")]
        public void Show()
        {
            Show(true);
        }

        public void Show(bool useDeltaTime, bool enforceFromValues = false)
        {
            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            if (hideAfterDelayCoroutine != null)
            {
                StopCoroutine(hideAfterDelayCoroutine);
                hideAfterDelayCoroutine = null;
            }

            if (IsShown)
            {
                if (hideAfterDelay)
                    hideAfterDelayCoroutine = StartCoroutine(HideAfterDelayCoroutine());

                return;
            }

            shown = true;
            onShowStarted.Invoke();

            if (hideAnimation.transformationType == TransformationType.None)
            {
                InstantShow();
                return;
            }

            if (canvasGroup)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            SetTween(ShowTween, useDeltaTime);
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            Hide(true);
        }

        public void Hide(bool useDeltaTime)
        {
            if (IsHidden)
                return;

            onHideStarted.Invoke();
            shown = false;

            if (!gameObject.activeInHierarchy || hideAnimation.transformationType == TransformationType.None)
            {
                InstantHide();
                return;
            }

            if (canvasGroup)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            SetTween(HideTween, useDeltaTime);
        }

        public void InstantShow()
        {
            shown = true;

            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            canvasGroup.alpha = showAnimation.value.y;
            transform.localScale = showAnimation.value.y * Vector3.one;

            if (controlsSetActive)
                gameObject.SetActive(true);
        }

        public void InstantHide()
        {
            if (!canvasGroup)
                return;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            shown = false;

            switch (hideAnimation.transformationType)
            {
                case TransformationType.Scale:
                    transform.localScale = hideAnimation.value.y * Vector3.one;
                    break;
                case TransformationType.Move:
                    rectTransform.anchoredPosition = hideAnimation.GetOffPosition(rectTransform, canvas);
                    break;
                case TransformationType.Fade:
                    canvasGroup.alpha = hideAnimation.value.y;
                    break;
            }

            if (controlsSetActive)
                gameObject.SetActive(false);
        }

        public void ToggleVisibility()
        {
            if (IsShown)
                Hide();
            else
                Show();
        }

        public void SetTween(Tween tween, bool useDeltaTime = false)
        {
            if (currentTween != null && currentTween.active && currentTween.IsPlaying())
                currentTween.Kill();
                
            currentTween = tween;
            currentTween.SetUpdate(!useDeltaTime);
        }

        public void SetTween(TweenData tweenData, TweenAnimType tweenAnimType = TweenAnimType.Additive)
        {
            SetTween(tweenData.GetTweenFrom(rectTransform, canvas, canvasGroup,
                IsHidden ? TweenAnimType.Additive : tweenAnimType));
        }
        
        private IEnumerator HideAfterDelayCoroutine()
        {
            yield return new WaitForSecondsRealtime(hideDelay);
            Hide();
        }
    }
}