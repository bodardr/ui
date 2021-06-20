using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField]
    private Image controlImage = null;

    [SerializeField]
    private TextMeshProUGUI promptText = null;

    [SerializeField]
    private Sprite keyboardIcon = null;

    [SerializeField]
    private Sprite gamepadIcon = null;

    private Canvas canvas;

    private ContentSizeFitter contentSizeFitter;
    private Interaction interaction;
    private ScreenSpaceUI screenSpaceUI;

    protected bool IsHidden { get; private set; } = true;

    public Transform Target
    {
        set => screenSpaceUI.Target = value;
    }

    private void Awake()
    {
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        canvas = GetComponent<Canvas>();
        screenSpaceUI = GetComponent<ScreenSpaceUI>();
    }

    public void UpdateScheme(bool isGamepad)
    {
        controlImage.sprite = isGamepad ? gamepadIcon : keyboardIcon;
    }

    private void UpdateText(string newText)
    {
        StartCoroutine(UpdateLayout());
        promptText.text = newText;
    }

    public virtual IEnumerator Show()
    {
        if (!IsHidden)
            yield return null;

        IsHidden = false;
        gameObject.SetActive(true);

        yield return null;
    }

    public virtual IEnumerator Hide()
    {
        if (IsHidden)
            yield return null;

        IsHidden = true;
        gameObject.SetActive(false);

        yield return null;
    }

    private IEnumerator UpdateLayout()
    {
        contentSizeFitter.enabled = false;
        yield return null;
        contentSizeFitter.enabled = true;
    }

    public void Initialize(Interaction interaction)
    {
        UpdateText(interaction.Text);
        interaction.OnTextUpdated += UpdateText;
    }

    public void Clear()
    {
        Target = null;
        interaction.OnTextUpdated -= UpdateText;
        interaction = null;
    }

    public void SetOffset(Vector2 offset)
    {
        screenSpaceUI.SetOffset(offset);
    }

    public void SetOffset(float offsetX, float offsetY)
    {
        SetOffset(new Vector2(offsetX, offsetY));
    }
}