using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableScript : MonoBehaviour
{

    public string interactableName = "Button A";
    public UnityEvent onInteracted;
    public UnityEvent<float> onValueChange;


    protected Quaternion initialRotation = Quaternion.identity, initialControllerRotation;


    public virtual bool Interact(GameObject controller)
    {
        initialRotation = transform.localRotation;
        initialControllerRotation = controller.transform.rotation;
        onInteracted.Invoke();
        return true;
    }

    public virtual void EndInteract()
    {
        initialRotation = transform.rotation;
    }

    public virtual void UpdateInteractable(GameObject controller)
    {

    }
}
