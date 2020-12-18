using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    protected Interactor interactor;
    protected InteractionPrompt prompt;

    public abstract string InteractionName { get; }

    public abstract bool CanInteract(Interactor interactor);

    /// <summary>
    /// Fires the interaction
    /// </summary>
    /// <returns>Returns true if it interrupts the interactor's actions, false if it is a single, uninterrupted interaction</returns>
    public abstract bool Interact();

    public void SetInteractor(Interactor interactor)
    {
        this.interactor = interactor;
    }

    public void UnassignInteractor()
    {
        interactor = null;
    }

    public void LiberateInteractor()
    {
        interactor.FreeFromInteraction();
    }

    public void SetPrompt(InteractionPrompt interactionPrompt)
    {
        prompt = interactionPrompt;
    }

    public void UnassignPrompt()
    {
        prompt = null;
    }
}