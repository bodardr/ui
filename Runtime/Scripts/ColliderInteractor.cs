using UnityEngine;

class ColliderInteractor : Interactor
{
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
}