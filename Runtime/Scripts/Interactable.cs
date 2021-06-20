using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private PromptPool promptPool;

    [SerializeField]
    private List<InteractionElement> interactions;

    [SerializeField]
    private List<InteractionElement> secondaryInteractions;

    [Header("Targets")]
    [SerializeField]
    private Transform customPrimaryTarget;

    [SerializeField]
    private Transform customSecondaryTarget;

    [Tooltip("Offset in screen-space (0,1)")]
    [SerializeField]
    private float offsetY = 0.05f;

    private Interactor interactor;

    private PoolableObject<InteractionPrompt> primaryPrompt;
    private PoolableObject<InteractionPrompt> secondaryPrompt;

    private Coroutine updateCoroutine;


    public void Enable(Interactor interactor)
    {
        this.interactor = interactor;
        updateCoroutine = StartCoroutine(UpdateInteractionsCoroutine());
    }

    public void Disable()
    {
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);

        primaryPrompt?.Release();
        secondaryPrompt?.Release();

        primaryPrompt = null;
        secondaryPrompt = null;

        interactor = null;
    }

    /// <summary>
    /// Coroutine that updates interactions every 0.3s.
    /// </summary>
    /// <returns>The coroutine.</returns>
    private IEnumerator UpdateInteractionsCoroutine()
    {
        while (isActiveAndEnabled)
        {
            interactions.Sort((x, y) => x.weight.CompareTo(y.weight));
            secondaryInteractions.Sort((x, y) => x.weight.CompareTo(y.weight));

            foreach (var i in interactions)
            {
                if (!i.interaction.CanInteract(interactor))
                    continue;

                SetInteraction(true, i.interaction);
                break;
            }

            foreach (var i in secondaryInteractions)
            {
                if (!i.interaction.CanInteract(interactor))
                    continue;

                SetInteraction(false, i.interaction);
                break;
            }

            yield return new WaitForSeconds(0.3f);
        }

        updateCoroutine = null;
    }

    private void SetInteraction(bool isPrimary, Interaction newInteraction)
    {
        var currentPrompt = isPrimary ? primaryPrompt : secondaryPrompt;

        if (newInteraction == null)
        {
            currentPrompt?.Release();

            if (isPrimary)
                primaryPrompt = null;
            else
                secondaryPrompt = null;

            return;
        }

        if (currentPrompt == null)
        {
            if (isPrimary)
                currentPrompt = primaryPrompt = promptPool.Get();
            else
                currentPrompt = secondaryPrompt = promptPool.Get();

            StartCoroutine(currentPrompt.Content.Show());
        }

        currentPrompt.Content.Initialize(newInteraction);
        var customTarget = isPrimary ? customPrimaryTarget : customSecondaryTarget;
        currentPrompt.Content.Target = customTarget ? customTarget : transform;
        currentPrompt.Content.SetOffset(0, isPrimary ? offsetY : -offsetY);

        if (isPrimary)
            interactor.Primary = newInteraction;
        else
            interactor.Secondary = newInteraction;
    }
}