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
            DiscardCurrentInteraction();

            current = value;

            if (current)
                AssignNewInteraction();

            primaryPrompt.UpdateVisibility();
            secondaryPrompt.UpdateVisibility();
        }
    }

    private void DiscardCurrentInteraction()
    {
        if (current)
            current.Disable();
        
        primaryPrompt.UnbindInteraction();
        secondaryPrompt.UnbindInteraction();
    }

    private void AssignNewInteraction()
    {
        current.Enable(this);

        if (current.Primary)
        {
            current.Primary.SetInteractor(this);
            primaryPrompt.BindInteraction(current.Primary);
        }

        if (current.Secondary)
        {
            current.Secondary.SetInteractor(this);
            secondaryPrompt.BindInteraction(current.Secondary);
        }
    }

    public UnityEvent OnInterruption => onInterruption;

    public UnityEvent OnInteractionFreed => onInteractionFreed;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();

        InstantiatePrompts();
        playerInput.onControlsChanged += UpdatePromptIcons;
    }

    private void InstantiatePrompts()
    {
        primaryPrompt = Instantiate(promptPrefab, promptCanvas).GetComponent<InteractionPrompt>();
        secondaryPrompt = Instantiate(secondaryPromptPrefab, promptCanvas).GetComponent<InteractionPrompt>();

        primaryPrompt.gameObject.SetActive(false);
        secondaryPrompt.gameObject.SetActive(false);
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

        primaryPrompt.UpdateScheme(isGamepad);
        secondaryPrompt.UpdateScheme(isGamepad);
    }
}