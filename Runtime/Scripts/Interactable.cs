using System.Collections.Generic;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    private Interactor interactor;

    [SerializeField]
    private List<InteractionElement> interactions;

    private Interaction current;

    public Interaction Current
    {
        get => current;
        set
        {
            if (enabled && current)
                current.HidePrompt();

            current = value;

            if (enabled && current)
                current.ShowPrompt();
        }
    }

    public void Enable(Interactor interactor)
    {
        this.interactor = interactor;
        UpdateInteractions();

        enabled = true;

        if (Current)
            Current.ShowPrompt();
    }

    public void Disable()
    {
        interactor = null;

        enabled = false;

        if (Current)
            Current.HidePrompt();
    }

    private void UpdateInteractions()
    {
        interactions.Sort((x, y) => x.weight.CompareTo(y.weight));

        foreach (var i in interactions)
        {
            if (i.interaction.CanInteract(interactor))
            {
                Current = i.interaction;
                break;
            }
        }
    }

    public bool Interact()
    {
        Current.AssignInteractor(interactor);
        return Current.Interact();
    }
}