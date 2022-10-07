using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobInteractable : InteractableScript
{
    //Currently only turning with no stop works, turniung with a stop is complicated :(
    //public bool turnInfinitely = false; //Set to true if the knob shouldn't stop anytime and just turns forever
    public float initialTurnPosition = 0.0f;//, turnRange = 1.0f; // initial position and max range of turning the knob (if "turnInfinitely" not set)

    protected float val = 0.0f, initialVal = 0.0f;

    public override void Reset()
    {
        val = initialTurnPosition;
        transform.localRotation = Quaternion.identity;
        base.Reset();
    }

    public override void Start()
    {
        initialVal = 0.0f;
        val = initialTurnPosition;
        base.Start();
    }

    public override bool Interact(GameObject controller)
    {
        base.Interact(controller);
        initialVal = val;

        return false;
    }

    public override void UpdateInteractable(GameObject controller)
    {
        Quaternion relativeRotation = Quaternion.Inverse(controller.transform.localRotation) * initialControllerRotation;

        val = ((relativeRotation.eulerAngles.z * 2f)%360f) / 360f + initialVal;
        //if (turnInfinitely)
        //{
            val = val % 1;
        //}
        transform.localRotation = Quaternion.Euler(0f, (val - initialVal) * 360, 0f) * initialRotation;
        onValueChange.Invoke(val);
    }
}
