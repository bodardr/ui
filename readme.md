Interaction System :

The Interactor : Contains logic to detect Interactables and assigns the Primary and Secondary interaction according to it.
OnInteract and OnSecondaryInteract launches the interactables. Can work with the new Input System

Hierarchy : Interactor -> ColliderInteractor

Interactable : An object that we can interact with. It contains the interactions, but is **not** an interaction.
See InteractionElement for this. 
It sorts the different available interactions using individual weights (customizable inside the Inspector).
Updates itself.

Interaction Element : The interaction itself! Has an overrideable method CanInteract to hide/unhide it,
it updates the prompt's text automatically.
When Interact is called, the return value (a bool) will indicate if it freezes the interactor in place or not. On the interactor's side, it will disable its controls.

InteractionPrompt
