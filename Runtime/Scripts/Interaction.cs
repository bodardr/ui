using System;
using UnityEngine;

public abstract class Interaction : MonoBehaviour
{
    protected Interactor interactor;
    protected virtual string InitialText { get; }

    public string Text { get; set; } = "Interact";

    protected void Awake()
    {
        Text = InitialText;
        OnTextUpdated += UpdateText;
    }

    private void OnDestroy()
    {
        OnTextUpdated = null;
    }

    private void UpdateText(string value) => Text = value;
    public virtual bool CanInteract(Interactor interactor) => true;

    /// <summary>
    /// Fires the interaction
    /// </summary>
    /// <returns>Returns true if it interrupts the interactor's actions, false if it is a single, uninterrupted interaction</returns>
    public abstract bool Interact(Interactor interactor);

    public void LiberateInteractor() => interactor.FreeFromInteraction();

    protected void InvokeOnTextUpdated(string text) => OnTextUpdated?.Invoke(text);
    public event Action<string> OnTextUpdated;
}