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
    public UnityEvent<InteractableScript> onEndInteract;
    public bool interacting = false;

    protected bool disabled = false;

    protected Quaternion initialRotation = Quaternion.identity, initialControllerRotation;

    public virtual void Reset()
    {

    }

    public virtual void Start()
    {
        GetComponent<Light>().range = idleLightRange;
        GetComponent<Light>().intensity = 7f;
        GetComponent<Light>().color = new Color(0f, 1f, 1f, 1f);
    }
    public virtual bool Interact(GameObject controller)
    {
        if (disabled)
        {
            return false;
        }

        initialRotation = transform.localRotation;
        initialControllerRotation = controller.transform.localRotation;
        onInteracted.Invoke();
        interacting = true;
        return true;
    }

    public virtual void EndInteract()
    {
        if (disabled)
        {
            return;
        }

        initialRotation = transform.rotation;
        interacting = false;
        onEndInteract.Invoke(this);
    }

    public virtual void Hover()
    {
        if (disabled)
        {
            return;
        }
        GetComponent<Light>().range = hoverLightRange;
        GetComponent<Light>().intensity = 8f;
        GetComponent<Light>().color = new Color(1f, 1f, 1f, 1f);
    }

    public virtual void ExitHover()
    {
        if (disabled)
        {
            return;
        }
        GetComponent<Light>().range = idleLightRange;
        GetComponent<Light>().intensity = 7f;
        GetComponent<Light>().color = new Color(0f, 1f, 1f, 1f);
    }

    public virtual void UpdateInteractable(GameObject controller)
    {

    }

    public virtual void SetDisabled(bool val)
    {
        if(disabled == val)
        {
            return;
        }

        disabled = val;

        if (disabled)
        {
            GetComponent<Light>().intensity = 0f;
        } else
        {
            GetComponent<Light>().intensity = 7f;
        }
    }
}
