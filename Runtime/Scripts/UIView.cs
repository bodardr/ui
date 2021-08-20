using DG.Tweening;
using UnityEngine;

namespace Bodardr.UI
{
    [AddComponentMenu("UI/UI View")]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class UIView : MonoBehaviour
    {
        [Header("Enabling / Disabling")]
        [SerializeField]
        private bool controlsSetActive = true;

        [SerializeField]
        private bool onEnableCallsShow = false;

        [Header("Animation")]
        [SerializeField]
        private bool animate;

        [SerializeField]
        private DotweenAnim showAnimation = new DotweenAnim
        {
            value = new Vector2(0, 1)
        };

        [SerializeField]
        private DotweenAnim hideAnimation = new DotweenAnim
        {
            value = new Vector2(1, 0)
        };

        private CanvasGroup canvasGroup;

        private Tween currentTween;
        private bool shown;

        public bool isAnimating => currentTween is {active: true};

        public Tween ShowTween
        {
            get
            {
                var showTween = showAnimation.GetTweenFrom(transform, canvasGroup);
                showTween = showAnimation.GetTweenFrom(transform, canvasGroup);
                showTween.OnComplete(() => shown = true);

                if (controlsSetActive)
                    showTween.OnStart(() => gameObject.SetActive(true));

                return showTween;
            }
        }

        public Tween HideTween
        {
            get
            {
                var hideTween = hideAnimation.GetTweenFrom(transform, canvasGroup);
                hideTween = hideAnimation.GetTweenFrom(transform, canvasGroup);
                hideTween.OnComplete(() =>
                {
                    shown = false;

                    if (controlsSetActive)
                        gameObject.SetActive(false);
                });

                return hideTween;
            }
        }

        public bool IsShown => shown;
        public bool IsHidden => !shown;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (!onEnableCallsShow)
                return;

            Show();
        }

        private void OnDisable()
        {
            InstantHide();
        }

        [ContextMenu("Show")]
        public void Show()
        {
            if (!animate)
            {
                InstantShow();
                return;
            }

            SetTween(ShowTween);
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            if (!animate)
            {
                InstantHide();
                return;
            }

            SetTween(HideTween);
        }

        private void InstantShow()
        {
            canvasGroup.alpha = 1;
            transform.localScale = Vector3.one;

            if (controlsSetActive)
                gameObject.SetActive(true);
        }

        private void InstantHide()
        {
            if (hideAnimation.transformationType == TransformationType.Fade)
                canvasGroup.alpha = 0;
            else
                transform.localScale = Vector3.zero;

            if (controlsSetActive)
                gameObject.SetActive(false);
        }

        public void SetTween(Tween tween)
        {
            CompleteCurrentTween();
            currentTween = tween;
        }

        public void SetTween(DotweenAnim dotweenAnim, TweenAnimType tweenAnimType = TweenAnimType.Additive)
        {
            SetTween(dotweenAnim.GetTweenFrom(transform, canvasGroup, tweenAnimType));
        }

        private void CompleteCurrentTween()
        {
            if (currentTween == null)
                return;

            if (currentTween.active)
                currentTween.Complete(true);

            currentTween.Kill();
        }
    }
}