using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Interactor interactor;
    private bool active;

    private Interaction primary;
    private Interaction secondary;

    private Coroutine updateCoroutine;

    [SerializeField]
    private List<InteractionElement> interactions;

    [SerializeField]
    private List<InteractionElement> secondaryInteractions;

    public Interaction Primary
    {
        get => primary;
        private set
        {
            if (primary == value)
                return;

            if (primary)
                primary.Clear();

            primary = value;

            if (Active)
            {
                if (primary)
                    primary.Initialize(interactor, interactor.PrimaryPrompt);

                interactor.PrimaryPrompt.Interaction = primary;
            }
        }
    }

    public Interaction Secondary
    {
        get => secondary;
        private set
        {
            if (secondary == value)
                return;

            if (secondary)
                secondary.Clear();

            secondary = value;

            if (Active)
            {
                if (secondary)
                    secondary.Initialize(interactor, interactor.SecondaryPrompt);

                interactor.SecondaryPrompt.Interaction = secondary;
            }
        }
    }

    public bool Active
    {
        get => active;
        set
        {
            active = value;

            if (active)
                updateCoroutine = StartCoroutine(UpdateInteractionsCoroutine());
        }
    }

    public void Enable(Interactor interactor)
    {
        this.interactor = interactor;
        Active = true;

        if (Primary)
            interactor.PrimaryPrompt.Interaction = Primary;

        if (Secondary)
            interactor.SecondaryPrompt.Interaction = Secondary;
    }

    public void Disable()
    {
        interactor.PrimaryPrompt.Clear();
        interactor.SecondaryPrompt.Clear();
        
        Active = false;
        interactor = null;
        
    }

    private IEnumerator UpdateInteractionsCoroutine()
    {
        while (active)
        {
            UpdateInteractions();
            yield return new WaitForSeconds(0.3f);
        }

        updateCoroutine = null;
    }

    private void UpdateInteractions()
    {
        interactions.Sort((x, y) => x.weight.CompareTo(y.weight));
        secondaryInteractions.Sort((x, y) => x.weight.CompareTo(y.weight));

        FilterPossibleInteractions(interactions, out var primary);
        FilterPossibleInteractions(secondaryInteractions, out var secondary);

        Primary = primary;
        Secondary = secondary;
    }

    private void FilterPossibleInteractions(List<InteractionElement> elements, out Interaction interaction)
    {
        foreach (var i in elements)
        {
            if (i.interaction.CanInteract(interactor))
            {
                interaction = i.interaction;
                return;
            }
        }

        interaction = null;
    }
}