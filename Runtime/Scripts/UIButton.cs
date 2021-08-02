using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bodardr.UI
{
    [RequireComponent(typeof(UIView), typeof(Button))]
    public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler, ISubmitHandler
    {
        [SerializeField]
        private DotweenAnim hoverAnim = new DotweenAnim
        {
            value = new Vector2(1, 1)
        };

        [SerializeField]
        private DotweenAnim clickAnim = new DotweenAnim()
        {
            value = new Vector2(1, 0.8f),
            ease = Ease.OutBack,
            loops = 1,
            loopType = LoopType.Yoyo
        };

        private Button button;
        private EventTriggerType previousEventType;

        private UIView uiView;

        private void Awake()
        {
            uiView = GetComponent<UIView>();
            button = GetComponent<Button>();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (previousEventType == EventTriggerType.Select)
                uiView.SetTween(-hoverAnim);

            previousEventType = EventTriggerType.Deselect;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSubmit(eventData);
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
            uiView.SetTween(hoverAnim, TweenAnimType.Override);

            previousEventType = EventTriggerType.Select;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            uiView.SetTween(clickAnim, TweenAnimType.Override);

            previousEventType = EventTriggerType.Submit;
        }
    }
}