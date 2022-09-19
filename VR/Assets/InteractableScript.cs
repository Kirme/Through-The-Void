using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableScript : MonoBehaviour
{

    public string interactableName = "Button A";

    private Quaternion initialControllerRotation = Quaternion.identity, initialRotation = Quaternion.identity;

    public void Interact(GameObject controller)
    {
        initialRotation = transform.localRotation;
        initialControllerRotation = controller.transform.localRotation;
    }

    public void EndInteract()
    {
        initialRotation = transform.rotation;
    }

    public void UpdateInteractable(GameObject controller)
    {
        Quaternion relativeRotation = Quaternion.Inverse(controller.transform.rotation) * initialControllerRotation;

        transform.localRotation = Quaternion.Euler(0f, relativeRotation.eulerAngles.z, 0f) * initialRotation;
    }
}
