using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public Animator anim;
    public KeyCode interactKey = KeyCode.E;
    private IInteractable currentInteractable;
    private bool isInteracting = false;

    void Start()
    {
        //Application.targetFrameRate = 30;

    }

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            if (isInteracting)
            {
                currentInteractable.StopInteraction();
                isInteracting = false;
                anim.enabled = true;
            }
            else
            {
                currentInteractable.Interact();
                isInteracting = true;
                anim.enabled = false;

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
