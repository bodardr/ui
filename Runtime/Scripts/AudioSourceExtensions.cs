using DG.Tweening;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void Crossfade(this AudioSource a, AudioSource b, float duration)
    {
        if (!b.isPlaying)
            b.Play();

        b.DOFade(1, duration).From(0);
        a.DOFade(0, duration).From(1).OnComplete(a.Stop);
    }
}