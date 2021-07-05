using System.Collections;
using DG.Tweening;

class AnimatedInteractionPrompt : InteractionPrompt
{
    public override IEnumerator Show()
    {
        if (!IsHidden)
            yield return null;

        transform.DOKill();
        yield return transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack).WaitForCompletion();

        yield return base.Show();
    }

    public override IEnumerator Hide()
    {
        if (IsHidden)
            yield return null;

        transform.DOKill();
        yield return transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false))
            .WaitForCompletion();

        yield return base.Hide();
    }
}