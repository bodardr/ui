using DG.Tweening;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void CrossfadeTo(this AudioSource origin, AudioSource destination, float duration = 1)
    {
        if (!destination.isPlaying)
            destination.Play();

        destination.DOFade(1, duration).From(0);
        origin.DOFade(0, duration).From(1).OnComplete(origin.Stop);
    }
}