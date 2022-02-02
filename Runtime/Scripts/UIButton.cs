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
        private UIView uiView;
        private Button button;
        
        private EventTriggerType previousEventType;

        private Graphic[] childrenGraphics;

        private bool wasInteractable;
        private bool isInteractable;

        private bool IsColorTintingChildrenGraphics =>
            applyColorTintToChildrenGraphics && button.transition == Selectable.Transition.ColorTint;

        private static DotweenAnim toDefaultTween = new DotweenAnim()
        {
            value = new Vector2(.8f, 1),
            duration = .25f,
            ease = Ease.InOutSine,
            loops = 0,
            transformationType = TransformationType.Scale
        };

        [SerializeField]
        private bool applyColorTintToChildrenGraphics = true;

        [SerializeField]
        private DotweenAnim hoverAnim = new DotweenAnim
        {
            value = new Vector2(1, 1)
        };

        [SerializeField]
        private DotweenAnim pressAnim = new DotweenAnim()
        {
            value = new Vector2(1, 0.8f),
            ease = Ease.OutBack
        };

        [FormerlySerializedAs("clickAnim")]
        [SerializeField]
        private DotweenAnim releaseAnim = new DotweenAnim()
        {
            value = new Vector2(0.8f, 1),
            ease = Ease.InBack,
        };

        private void Awake()
        {
            uiView = GetComponent<UIView>();
            button = GetComponent<Button>();

            if (applyColorTintToChildrenGraphics)
                childrenGraphics = GetComponentsInChildren<Graphic>(true);

            var rectTransform = (RectTransform)transform;
            
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

        public void OnSelect(BaseEventData eventData)
        {
            if (!button.interactable)
                return;

            uiView.SetTween(hoverAnim);

            if (IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.selectedColor);

            previousEventType = EventTriggerType.Select;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (previousEventType is EventTriggerType.Select or EventTriggerType.PointerDown)
                if (hoverAnim.loops <= 0)
                    uiView.SetTween(toDefaultTween);
                else
                    uiView.SetTween(-hoverAnim);

            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.normalColor);

            previousEventType = EventTriggerType.Deselect;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!button.interactable)
                return;

            uiView.SetTween(releaseAnim);

            if (IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.pressedColor);

            previousEventType = EventTriggerType.Submit;
        }

        private void OnPress(PointerEventData eventData)
        {
            if (!button.interactable)
                return;

            uiView.SetTween(pressAnim);
            
            if (IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.pressedColor);

            previousEventType = EventTriggerType.PointerDown;
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (button.interactable && IsColorTintingChildrenGraphics)
                CrossFadeAllGraphics(button.colors.normalColor);

            previousEventType = EventTriggerType.Cancel;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSubmit(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnSelect(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPress(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnDeselect(eventData);
        }

        private void CrossFadeAllGraphics(Color color)
        {
            foreach (var graphic in childrenGraphics)
            {
                graphic.CrossFadeColor(color, button.colors.fadeDuration, true, true);
            }
        }
    }
}