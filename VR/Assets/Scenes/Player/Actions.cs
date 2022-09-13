using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Actions : MonoBehaviour
{

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Single squeezeAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetSqueeze() > 0)
        {
            print("squeeze " + GetSqueeze() + handType);
        }
    }

    public float GetSqueeze()
    {
        return squeezeAction.GetAxis(handType);
    }
}
