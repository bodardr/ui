using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    private HashSet<Interactable> interactables = new HashSet<Interactable>();
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

    private Interactable Current
    {
        get => current;
        set
        {
            if (current)
            {
                current.Disable();
            }

            current = value;

            if (current)
            {
                current.Enable(this);
            }
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

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();

        if (!interactable)
            return;

        interactables.Add(interactable);
        UpdateInteractables();
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();

        if (interactables.Remove(interactable))
            UpdateInteractables();
    }

    private void OnInteract()
    {
        if (!Current)
            return;

        if (Current.Interact())
            OnInterruption?.Invoke();
    }

    public void FreeFromInteraction()
    {
        OnInteractionFreed?.Invoke();
    }

    private void UpdateInteractables()
    {
        float minDist = float.MaxValue;
        var pos = transform.position;

        Interactable closestInteractable = null;

        foreach (var i in interactables)
        {
            var dist = Vector3.Distance(pos, i.transform.position);
            if (dist > minDist)
                continue;

            minDist = dist;
            closestInteractable = i;
        }

        Current = closestInteractable;
    }

    private void UpdatePromptIcons(PlayerInput obj)
    {
        bool isGamepad = obj.currentControlScheme == "Gamepad";

        primaryPrompt.UpdateScheme(isGamepad);
        secondaryPrompt.UpdateScheme(isGamepad);
    }
}