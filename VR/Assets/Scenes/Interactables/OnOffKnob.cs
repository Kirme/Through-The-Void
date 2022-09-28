using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnOffKnob : KnobInteractable
{
    public bool snap = true;
    public UnityEvent<bool> valueSwitched;
    public bool on = false;

    public override void Start()
    {
        if (on)
        {
            initialTurnPosition = 0.5f;
        }
        base.Start();
    }
    public override void EndInteract()
    {
        if (val < 0.25  || val > 0.75)
        {
            val = 0.0f;
            on = false;
            valueSwitched.Invoke(false);
        } else
        {
            val = 0.5f;
            on = true;
            valueSwitched.Invoke(true);
        }
        if (snap)
        {
            transform.localRotation = Quaternion.Euler(0f, val * 360, 0f);
        }
        base.EndInteract();
    }

}
