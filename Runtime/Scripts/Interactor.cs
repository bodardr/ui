using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Interactor : MonoBehaviour
{
    protected HashSet<Interactable> interactables = new HashSet<Interactable>();
    private Interactable current;

    private PlayerInput playerInput;

    private InteractionPrompt primaryPrompt;
    private InteractionPrompt secondaryPrompt;

    [SerializeField]
    private Transform promptCanvas = null;

    [SerializeField]
    private GameObject promptPrefab = null;

    [SerializeField]
    private GameObject secondaryPromptPrefab = null;

    [SerializeField]
    private UnityEvent onInterruption;

    [SerializeField]
    private UnityEvent onInteractionFreed;

    protected Interactable Current
    {
        get => current;
        set
        {
            if (current)
                current.Disable();

            current = value;

            if (current)
                current.Enable(this);
        }
    }

    public UnityEvent OnInterruption => onInterruption;

    public UnityEvent OnInteractionFreed => onInteractionFreed;

    public InteractionPrompt PrimaryPrompt => primaryPrompt;
    public InteractionPrompt SecondaryPrompt => secondaryPrompt;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();

        InstantiatePrompts();
        playerInput.onControlsChanged += UpdatePromptIcons;
        UpdatePromptIcons(playerInput);
    }

    private void InstantiatePrompts()
    {
        primaryPrompt = Instantiate(promptPrefab, promptCanvas).GetComponent<InteractionPrompt>();
        secondaryPrompt = Instantiate(secondaryPromptPrefab, promptCanvas).GetComponent<InteractionPrompt>();

        PrimaryPrompt.gameObject.SetActive(false);
        SecondaryPrompt.gameObject.SetActive(false);
    }

    private void OnInteract()
    {
        if (!Current || !Current.Primary)
            return;

        if (Current.Primary.Interact())
            OnInterruption?.Invoke();
    }

    private void OnSecondaryInteract()
    {
        if (!Current || !Current.Secondary)
            return;

        if (Current.Secondary.Interact())
            OnInterruption?.Invoke();
    }

    public void FreeFromInteraction()
    {
        OnInteractionFreed?.Invoke();
    }

    private void UpdatePromptIcons(PlayerInput obj)
    {
        bool isGamepad = obj.currentControlScheme == "Gamepad";

        PrimaryPrompt.UpdateScheme(isGamepad);
        SecondaryPrompt.UpdateScheme(isGamepad);
    }
}