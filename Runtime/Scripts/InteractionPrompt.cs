using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    private Interaction interaction;
    private ScreenSpaceUI screenSpaceUI;

    [SerializeField]
    private Image controlImage = null;

    [SerializeField]
    private TextMeshProUGUI promptText = null;

    [SerializeField]
    private Sprite keyboardIcon = null;

    [SerializeField]
    private Sprite gamepadIcon = null;

    public bool IsHidden { get; private set; } = true;

    public Interaction Interaction
    {
        get => interaction;
        set
        {
            if (interaction == value)
                return;

            interaction = value;

            if (screenSpaceUI && interaction is ITargetableInteraction targetableInteraction)
                UpdateInteractionTarget(targetableInteraction);

            UpdateText();
            UpdateVisibility();
        }
    }

    public bool HasInteraction => Interaction != null;

    private void Awake()
    {
        screenSpaceUI = GetComponent<ScreenSpaceUI>();
    }

    public void UpdateScheme(bool isGamepad)
    {
        controlImage.sprite = isGamepad ? gamepadIcon : keyboardIcon;
    }

    private void UpdateInteractionTarget(ITargetableInteraction t)
    {
        screenSpaceUI.Target = t.Target;
    }

    public void UpdateText()
    {
        if (Interaction)
            promptText.text = Interaction.InteractionName;
    }

    public void Clear()
    {
        Interaction = null;
    }

    private void UpdateVisibility()
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