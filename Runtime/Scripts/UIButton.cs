using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Bodardr.UI
{
    [AddComponentMenu("UI/UI Button")]
    [RequireComponent(typeof(UIView), typeof(Button))]
    public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler, ISubmitHandler, ICancelHandler, IPointerDownHandler
    {
        private static TweenData toDefaultTween = new TweenData()
        {
            value = new Vector2(.8f, 1),
            duration = .25f,
            ease = Ease.InOutSine,
            loops = 0,
            transformationType = TransformationType.Scale
        };

        [SerializeField]
        private bool tweensBypassInteractableStatus;

        [SerializeField]
        private bool applyColorTintToChildrenGraphics = true;

        [SerializeField]
        private TweenData hoverAnim = new TweenData
        {
            value = new Vector2(1, 1)
        };

        [SerializeField]
        private TweenData pressAnim = new TweenData()
        {
            value = new Vector2(1, 0.8f),
            ease = Ease.OutBack
        };

        [FormerlySerializedAs("clickAnim")]
        [SerializeField]
        private TweenData releaseAnim = new TweenData()
        {
            value = new Vector2(0.8f, 1),
            ease = Ease.InBack,
        };

        private Button button;
        private Canvas canvas;
        private CanvasGroup canvasGroup;

        private Graphic[] childrenGraphics;

        private Tween currentTween;
        private bool isInteractable;

        private EventTriggerType previousEventType;

        private RectTransform rectTransform;
        private UIView uiView;

        private bool wasInteractable;

        private bool IsColorTintingChildrenGraphics =>
            applyColorTintToChildrenGraphics && button.transition == Selectable.Transition.ColorTint;

        private void Awake()
        {
            uiView = GetComponent<UIView>();
            button = GetComponent<Button>();

            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();

            if (applyColorTintToChildrenGraphics)
                childrenGraphics = GetComponentsInChildren<Graphic>(true);

            rectTransform = (RectTransform)transform;

            pressAnim.Initialize(rectTransform);
            releaseAnim.Initialize(rectTransform);
            hoverAnim.Initialize(rectTransform);
        }

        private void Start()
        {
            if (!IsColorTintingChildrenGraphics)
                return;

            if (button.interactable)
                CrossFadeAllGraphics(button.colors.normalColor);
            else
                CrossFadeAllGraphics(button.colors.disabledColor);
        }

        private void Update()
        {
            wasInteractable = isInteractable;
            isInteractable = button.interactable;

            if (!IsColorTintingChildrenGraphics)
                return;

            if (!wasInteractable && isInteractable)
                CrossFadeAllGraphics(button.colors.normalColor);
            else if (wasInteractable && !isInteractable)
                CrossFadeAllGraphics(button.colors.disabledColor);
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.normalColor);

            previousEventType = EventTriggerType.Cancel;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (previousEventType is EventTriggerType.Select or EventTriggerType.PointerDown)
                if (hoverAnim.loops > 0)
                    SetTween(-hoverAnim);

            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.normalColor);

            previousEventType = EventTriggerType.Deselect;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSubmit(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPress(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnSelect(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnDeselect(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!tweensBypassInteractableStatus && !button.interactable)
                return;

            SetTween(hoverAnim);

            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.selectedColor);

            previousEventType = EventTriggerType.Select;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!tweensBypassInteractableStatus && !button.interactable)
                return;

            SetTween(releaseAnim);

            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.pressedColor);

            previousEventType = EventTriggerType.Submit;
        }

        private void OnPress(PointerEventData eventData)
        {
            if (!tweensBypassInteractableStatus && !button.interactable)
                return;

            SetTween(pressAnim);

            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.pressedColor);

            previousEventType = EventTriggerType.PointerDown;
        }

        private void SetTween(TweenData tween)
        {
            if (tween.ease == Ease.Unset || tween.duration <= 0)
                return;

            CompleteCurrentTween();

            currentTween = tween.GetTweenFrom(rectTransform, canvas, canvasGroup);
            currentTween.SetUpdate(true);
        }

        private void CompleteCurrentTween()
        {
            if (currentTween == null)
                return;

            currentTween.OnComplete(null);
            currentTween.Kill(true);
            currentTween = null;
        }

        private void CrossFadeAllGraphics(Color color)
        {
            foreach (var graphic in childrenGraphics)
                graphic.CrossFadeColor(color, button.colors.fadeDuration, true, true);
        }
    }
}