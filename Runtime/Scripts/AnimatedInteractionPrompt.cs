using DG.Tweening;

class AnimatedInteractionPrompt : InteractionPrompt
{
    protected override void Show()
    {
        base.Show();

        if (!IsHidden)
            return;
        
        transform.DOKill();
        transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack);
    }

    protected override void Hide()
    {
        base.Hide();

        if (IsHidden)
            return;
        
        transform.DOKill();
        transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
    }
}