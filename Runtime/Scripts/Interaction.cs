using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    private Interactor interactor;

    public abstract bool CanInteract(Interactor interactor);

    /// <summary>
    /// Fires the interaction
    /// </summary>
    /// <returns>Returns true if it interrupts the interactor's actions, false if it is a single, uninterrupted interaction</returns>
    public abstract bool Interact();

    public void AssignInteractor(Interactor interactor)
    {
        this.interactor = interactor;
    }

    public void LiberateInteractor()
    {
        interactor.FreeFromInteraction();
    }

    public abstract void ShowPrompt();

    public abstract void HidePrompt();
}