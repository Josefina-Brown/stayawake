using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private IInteractable currentInteractable;
    private bool isInteracting = false;

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            if (isInteracting)
            {
                currentInteractable.StopInteraction();
                isInteracting = false;
            }
            else
            {
                currentInteractable.Interact();
                isInteracting = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            if (isInteracting)
            {
                interactable.StopInteraction(); 
                isInteracting = false;
            }
            currentInteractable = null;
        }
    }
}
