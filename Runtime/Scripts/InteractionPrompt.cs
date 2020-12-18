using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    private Interaction interaction;
    private ScreenSpaceUI screenSpaceUI;

    [SerializeField]
    private Image controlImage;

    [SerializeField]
    private TextMeshProUGUI promptText;

    [SerializeField]
    private Sprite keyboardIcon;

    [SerializeField]
    private Sprite gamepadIcon;

    public bool IsHidden { get; private set; } = true;
    public bool HasInteraction => interaction;

    private void Awake()
    {
        screenSpaceUI = GetComponent<ScreenSpaceUI>();
    }

    public void UpdateScheme(bool isGamepad)
    {
        controlImage.sprite = isGamepad ? gamepadIcon : keyboardIcon;
    }

    public virtual void BindInteraction(Interaction currentInteraction)
    {
        interaction = currentInteraction;
        interaction.SetPrompt(this);
        
        if (screenSpaceUI && currentInteraction is ITargetableInteraction targetableInteraction)
            UpdateInteractionTarget(targetableInteraction);
        
        UpdateText();
    }

    private void UpdateInteractionTarget(ITargetableInteraction t)
    {
        screenSpaceUI.Target = t.Target;
    }

    public void UnbindInteraction()
    {
        if (interaction)
            interaction.UnassignPrompt();

        interaction = null;
    }

    public void UpdateText()
    {
        promptText.text = interaction.InteractionName;
    }

    public void UpdateVisibility()
    {
        if (HasInteraction)
            Show();
        else
            Hide();
    }

    protected virtual void Show()
    {
        if (!IsHidden)
            return;

        IsHidden = false;
        gameObject.SetActive(true);
    }

    protected virtual void Hide()
    {
        if (IsHidden)
            return;

        IsHidden = true;
        gameObject.SetActive(false);
    }
}

public interface ITargetableInteraction
{
    Transform Target { get; }
}