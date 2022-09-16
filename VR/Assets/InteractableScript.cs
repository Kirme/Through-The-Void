using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableScript : MonoBehaviour
{

    public string interactableName = "Button A";

    private Quaternion initialRotation = Quaternion.identity; 

    public void Interact(GameObject controller)
    {
        initialRotation = controller.transform.localRotation;
        Debug.Log("pressed " + initialRotation);
    }

    public void EndInteract()
    {
        Debug.Log("pressed " + interactableName);
    }

    public void UpdateInteractable(GameObject controller)
    {
        Quaternion relativeRotation = Quaternion.Inverse(controller.transform.rotation) * initialRotation;

        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, relativeRotation.eulerAngles.z, transform.localRotation.eulerAngles.z);
    }
}
