using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField]
    private Image controlImage;

    [SerializeField]
    private TextMeshProUGUI promptText;

    [SerializeField]
    private Sprite keyboardIcon;

    [SerializeField]
    private Sprite gamepadIcon;

    public void UpdateScheme(bool isGamepad)
    {
        controlImage.sprite = isGamepad ? gamepadIcon : keyboardIcon;
    }
}