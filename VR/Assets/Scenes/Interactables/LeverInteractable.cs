using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverInteractable : InteractableScript
{

    public float maxPull = 0.1f, maxRot = 75f;
    public bool allowNegative = true;
    private Vector3 initialControllerPosition = Vector3.zero;
    private float initialVal = 0.0f, val = 0.0f;


    public override void Interact(GameObject controller)
    {
        base.Interact(controller);
        initialControllerPosition = controller.transform.localPosition;
        initialVal = val;
    }

    public override void UpdateInteractable(GameObject controller)
    {
        
        Vector3 relativePosition = controller.transform.localPosition - initialControllerPosition;
        Vector3 projected = Vector3.Project(relativePosition, Vector3.forward);
        val = Mathf.Clamp((projected.magnitude * Mathf.Sign(Vector3.Dot(projected.normalized, Vector3.forward)))/ maxPull + initialVal, allowNegative?-1:0, 1);

        transform.localRotation = Quaternion.Euler((val - initialVal)*maxRot, 0f, 0f) * initialRotation;

        onValueChange.Invoke(val);
    }
}
