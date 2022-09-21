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


    public virtual void Interact(GameObject controller)
    {
        initialRotation = transform.localRotation;
        initialControllerRotation = controller.transform.rotation;
        onInteracted.Invoke();
    }

    public virtual void EndInteract()
    {
        initialRotation = transform.rotation;
    }

    public virtual void UpdateInteractable(GameObject controller)
    {
        Quaternion relativeRotation = Quaternion.Inverse(controller.transform.rotation) * initialControllerRotation;

        transform.localRotation = Quaternion.Euler(0f, relativeRotation.eulerAngles.z, 0f) * initialRotation;
        onValueChange.Invoke(relativeRotation.eulerAngles.z/360f);
    }
}
