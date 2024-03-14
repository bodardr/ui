using System;
using DG.Tweening;
using UnityEngine;

namespace Bodardr.UI
{
    [Serializable]
    public class TweenData
    {
        public TransformationType transformationType;
        private readonly bool isReversed = false;

        public Direction direction;

        public Vector2 value;
        private Vector2 initialOffset;

        public float duration;

        public Ease ease;

        public bool advanced;

        public int loops = 0;
        public LoopType loopType;
        public float delay;

        public TweenData()
        {
        }

        public TweenData(TweenData copyFrom, bool reverse = false) : this()
        {
            duration = copyFrom.duration;
            delay = copyFrom.delay;
            ease = copyFrom.ease;
            value = copyFrom.value;
            transformationType = copyFrom.transformationType;
            isReversed = reverse ? !copyFrom.isReversed : copyFrom.isReversed;
            advanced = copyFrom.advanced;
            loops = copyFrom.loops;
            loopType = copyFrom.loopType;
        }

        public static TweenData operator -(TweenData a) => new(a, true);

        public void Initialize(RectTransform rectTransform)
        {
            initialOffset = rectTransform.anchoredPosition;
        }

        public Tween GetTweenFrom(RectTransform transform, Canvas canvas, CanvasGroup canvasGroup,
            TweenAnimType tweenType = TweenAnimType.Additive)
        {
            Tweener tween = null;

            //Sets the duration to a minimal amount to avoid NaNs.
            duration = Mathf.Max(duration, 0.01f);

            switch (transformationType)
            {
                case TransformationType.Scale:
                    tween = transform.DOScale(value.y, duration);

                    if (tweenType == TweenAnimType.Override && loops >= 0)
                        tween.ChangeStartValue(value.x * Vector3.one);
                    break;

                case TransformationType.Fade:
                    tween = canvasGroup.DOFade(value.y, duration);

                    if (tweenType == TweenAnimType.Override && loops >= 0)
                        tween.ChangeStartValue(value.x);
                    break;

                case TransformationType.Move:
                    var fromPos = GetOffPosition(transform, canvas);

                    var a = Vector2.Lerp(fromPos, initialOffset, value.x);
                    var b = Vector2.Lerp(fromPos, initialOffset, value.y);

                    tween = transform.DOAnchorPos(b, duration).From(a);
                    break;
            }

            if (tween == null)
                return null;

            tween.SetEase(ease);
            tween.SetDelay(delay);

            if (isReversed)
                tween.IsBackwards();

            if (loops != 0)
                tween.SetLoops(loops, loopType);

            return tween;
        }

        public Vector2 GetOffPosition(RectTransform transform, Canvas canvas)
        {
            // 1         2
            //   corners
            // 0         3
            var rect = transform.rect;

            if (canvas == null)
                canvas = transform.GetComponentInParent<Canvas>();
            
            var cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, Vector2.zero,
                cam, out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform,
                new Vector2(Screen.currentResolution.width, Screen.currentResolution.height), cam, out var max);

            var fromPos = Vector2.zero;

            switch (direction)
            {
                case Direction.Up:
                    fromPos = new Vector2(0, min.y - rect.height);
                    break;
                case Direction.Down:
                    fromPos = new Vector2(0, max.y + rect.height);
                    break;
                case Direction.Left:
                    fromPos = new Vector2(max.x + rect.width, 0);
                    break;
                case Direction.Right:
                    fromPos = new Vector2(min.x - rect.width, 0);
                    break;
            }

            return fromPos + transform.anchoredPosition;
        }
    }

    public enum TweenAnimType
    {
        Additive,
        Override
    }

    public enum TransformationType
    {
        None,
        Scale,
        Fade,
        Move,
    }

    public enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }
}