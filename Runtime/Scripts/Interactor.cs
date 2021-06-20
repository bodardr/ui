using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Interactor : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onInterruption;

    [SerializeField]
    private UnityEvent onInteractionFreed;

    private Interactable current;

    protected HashSet<Interactable> interactables = new HashSet<Interactable>();
    private PlayerInput playerInput;

    protected Interactable Current
    {
        get => current;
        set
        {
            if (current == value)
                return;

            if (current)
                current.Disable();

            current = value;

            if (current)
                current.Enable(this);
        }
    }

    public UnityEvent OnInterruption => onInterruption;

    public UnityEvent OnInteractionFreed => onInteractionFreed;

    public Interaction Primary { get; set; }
    public Interaction Secondary { get; set; }

    private void OnInteract()
    {
        if (!Primary)
            return;

        if (Primary.Interact(this))
            OnInterruption?.Invoke();
    }

    private void OnSecondaryInteract()
    {
        if (!Secondary)
            return;

        if (Secondary.Interact(this))
            OnInterruption?.Invoke();
    }

    public void FreeFromInteraction()
    {
        OnInteractionFreed?.Invoke();
    }
}