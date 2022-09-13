using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerControler : MonoBehaviour
{

    private SteamVR_Input_Sources handType = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Single squeezeAction;

    public float maxSpeed = 25f;
    public float acceleration = 2.5f;

    private float speed = 0f;



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


        speed = Mathf.Lerp(speed, maxSpeed * GetSqueeze(), acceleration * Time.deltaTime);

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public float GetSqueeze()
    {
        return squeezeAction.GetAxis(handType);
    }
}