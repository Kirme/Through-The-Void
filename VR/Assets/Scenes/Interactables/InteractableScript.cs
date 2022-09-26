using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class InteractableScript : MonoBehaviour
{

    public string interactableName = "ButtonA";
    public float idleLightRange = 0.04f, hoverLightRange = 0.07f;
    public UnityEvent onInteracted;
    public UnityEvent<float> onValueChange;
    public bool interacting = false;

    protected Quaternion initialRotation = Quaternion.identity, initialControllerRotation;

    public void Start()
    {
        GetComponent<Light>().range = idleLightRange;
        GetComponent<Light>().intensity = 7f;
        GetComponent<Light>().color = new Color(0f, 1f, 1f, 1f);
    }
    public virtual bool Interact(GameObject controller)
    {
        initialRotation = transform.localRotation;
        initialControllerRotation = controller.transform.localRotation;
        onInteracted.Invoke();
        interacting = true;
        return true;
    }

    public virtual void EndInteract()
    {
        initialRotation = transform.rotation;
        interacting = false;
    }

    public virtual void Hover()
    {
        GetComponent<Light>().range = hoverLightRange;
        GetComponent<Light>().intensity = 8f;
        GetComponent<Light>().color = new Color(1f, 1f, 1f, 1f);
    }

    public virtual void ExitHover()
    {
        GetComponent<Light>().range = idleLightRange;
        GetComponent<Light>().intensity = 7f;
        GetComponent<Light>().color = new Color(0f, 1f, 1f, 1f);
    }

    public virtual void UpdateInteractable(GameObject controller)
    {

    }
}
