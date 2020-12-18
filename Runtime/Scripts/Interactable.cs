using System.Collections.Generic;
using UnityEngine;


public class Interactable : MonoBehaviour
{
    private Interactor interactor;

    [SerializeField]
    private List<InteractionElement> interactions;

    [SerializeField]
    private List<InteractionElement> secondaryInteractions;

    public Interaction Primary { get; private set; }
    public Interaction Secondary { get; private set; }

    public void Enable(Interactor interactor)
    {
        this.interactor = interactor;
        UpdateInteractions();
    }

    public void Disable()
    {
        interactor = null;

        if (Primary)
            Primary.UnassignInteractor();

        if (Secondary)
            Secondary.UnassignInteractor();
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