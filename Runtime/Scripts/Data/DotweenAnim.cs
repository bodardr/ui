using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace Bodardr.UI
{
    [Serializable]
    public class DotweenAnim
    {
        public TransformationType transformationType;

        public Vector2 value;

        public float duration;

        public Ease ease;

        public int loops = 0;
        public LoopType loopType;
        private readonly bool isReversed = false;

        public DotweenAnim()
        {
        }

        public DotweenAnim(DotweenAnim copyFrom, bool reverse = false) : this()
        {
            duration = copyFrom.duration;
            ease = copyFrom.ease;
            value = copyFrom.value;
            transformationType = copyFrom.transformationType;
            isReversed = reverse ? !copyFrom.isReversed : copyFrom.isReversed;
            loops = copyFrom.loops;
            loopType = copyFrom.loopType;
        }

        public static DotweenAnim operator -(DotweenAnim a) => new DotweenAnim(a, true);

        public Tween GetTweenFrom(Transform transform, CanvasGroup canvasGroup,
            TweenAnimType tweenType = TweenAnimType.Additive)
        {
            Tween tween;

            switch (transformationType)
            {
                case TransformationType.Scale:
                    var a = transform.DOScale(isReversed ? value.x : value.y, duration)
                        .From(isReversed ? value.y : value.x);

                    if (tweenType == TweenAnimType.Override && loops == 0)
                        a.NoFrom();

                    tween = a;
                    break;

                case TransformationType.Fade:
                    var b = canvasGroup.DOFade(isReversed ? value.x : value.y, duration)
                        .From(isReversed ? value.y : value.x);

                    if (tweenType == TweenAnimType.Override && loops == 0)
                        b.NoFrom();

                    tween = b;
                    break;

                default:
                    var c = canvasGroup.DOFade(isReversed ? value.x : value.y, duration)
                        .From(isReversed ? value.y : value.x);

                    if (tweenType == TweenAnimType.Override && loops == 0)
                        c.NoFrom();

                    tween = c;
                    break;
            }

            tween.SetEase(ease);

            if (loops != 0)
                tween.SetLoops(loops, loopType);

            return tween;
        }
    }

    public enum TweenAnimType
    {
        Additive,
        Override
    }

    public enum TransformationType
    {
        Scale,
        Fade,
    }
}