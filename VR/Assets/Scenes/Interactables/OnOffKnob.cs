using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnOffKnob : KnobInteractable
{
    public bool snap = true;
    public UnityEvent<bool> valueSwitched;
    public bool startOn = false;
    private bool on = false;

    public override void Reset()
    {
        on = startOn;
        base.Reset();
        transform.localRotation = Quaternion.identity;
    }

    public override void Start()
    {
        initialVal = 0.0f;
        on = startOn;
        base.Start();
    }
    public override void EndInteract()
    {
        if (val < 0.25  || val > 0.75)
        {
            val = 0.0f;
            on = startOn;
            valueSwitched.Invoke(startOn);
        } else
        {
            val = 0.5f;
            on = !startOn;
            valueSwitched.Invoke(!startOn);
        }
        if (snap)
        {
            transform.localRotation = Quaternion.Euler(0f, val * 360, 0f);
        }
        base.EndInteract();
    }

}
