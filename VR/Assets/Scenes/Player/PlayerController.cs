using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class PlayerController : MonoBehaviour
{

    private SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    private SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;

    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Pose poseAction;
    public GameObject rightController, leftController, joystick, warningLight, faultDisplay;

    public float maxSpeed = 25f, acceleration = 2.5f, maxTurnSpeed = 12.5f, turnAcceleration = 2.5f;


    audioManager sn;

    // Use these to change max speed dynamically, for example if a fault causes rotation speed to decrease.
    private float maxSpeedModifier = 1.0f, maxTurnSpeedModifier = 1.0f;

    public float xRotDeadzone = 10.0f, zRotDeadzone = 10.0f, yRotDeadzone = 10.0f;
    public float maxSteeringRot = 70.0f;
    
    private float speed = 0f, inputSpeed = 0f;
    private Vector3 rotationSpeed = new Vector3(0,0,0);

    public void Break(Fault fault, int numFaults)
    {
        maxSpeedModifier *= fault.maxSpeedModifier;
        acceleration *= fault.accelerationModifier;
        maxTurnSpeedModifier *= fault.maxTurnSpeedModifier;
        turnAcceleration *= fault.turnAccelerationModifier;


        warningLight.SetActive(true);
        warningLight.GetComponent<WarningLight>().speed = 2.5f + 0.5f * numFaults;
        faultDisplay.GetComponent<FaultDisplay>().SetNumFaults(numFaults);
    }

    public void Fix(Fault fault, int remainingFaults)
    {
        maxSpeedModifier /= fault.maxSpeedModifier;
        acceleration /= fault.accelerationModifier;
        maxTurnSpeedModifier /= fault.maxTurnSpeedModifier;
        turnAcceleration /= fault.turnAccelerationModifier;

        if(remainingFaults  == 0)
        {
            warningLight.SetActive(false);
        } else
        {
            warningLight.GetComponent<WarningLight>().speed = 2.5f + 0.5f * remainingFaults;
        }
        faultDisplay.GetComponent<FaultDisplay>().SetNumFaults(remainingFaults);
    }

    // Start is called before the first frame update
    void Start()
    {
        warningLight.SetActive(false);
        poseAction[rightHand].onTrackingChanged += OnTrackPadChanged;

        joystick.GetComponent<JoystickInteractable>().SetPlayer(this);

        sn = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<audioManager>();
        sn.Play("NASA");
    }

    // Update is called once per frame
    void Update()
    {
        // Only rotate if grabbing

        Vector3 joystickRotation = Vector3.zero;

        bool grabbing = false;
        JoystickInteractable joystickInteractable = joystick.GetComponent<JoystickInteractable>();
        if (joystickInteractable != null)
        {
            grabbing = joystickInteractable.interacting;

            if (grabbing)
            {
                joystickRotation = joystickInteractable.attitude;
            } 

        }

        rotationSpeed.x = Mathf.Lerp(rotationSpeed.x, -maxTurnSpeed * joystickRotation.x * Time.deltaTime * maxTurnSpeedModifier, turnAcceleration * Time.deltaTime * maxTurnSpeedModifier);
        rotationSpeed.y = Mathf.Lerp(rotationSpeed.y, -maxTurnSpeed * joystickRotation.z * Time.deltaTime * maxTurnSpeedModifier, turnAcceleration * Time.deltaTime * maxTurnSpeedModifier);
        rotationSpeed.z = Mathf.Lerp(rotationSpeed.z, maxTurnSpeed / 2 * joystickRotation.y * Time.deltaTime * maxTurnSpeedModifier, turnAcceleration * Time.deltaTime * maxTurnSpeedModifier);

        speed = Mathf.Lerp(speed, maxSpeed * inputSpeed * maxSpeedModifier, acceleration * Time.deltaTime * maxSpeedModifier);
        
        transform.Rotate(rotationSpeed.x * maxTurnSpeedModifier, rotationSpeed.y * maxTurnSpeedModifier, rotationSpeed.z * maxTurnSpeedModifier, Space.Self);
        transform.position += transform.forward * speed * Time.deltaTime;
    }



    public void SetSpeed(float speed)
    {

        const float speedDeadzone = 0.05f;

        if(Mathf.Abs(speed) < speedDeadzone)
        {
            speed = 0;
        } else
        {
            speed = (speed - speedDeadzone) / (1f - speedDeadzone);
        }

        if(speed < 0)
        {
            speed /= 10;
        }
        inputSpeed = speed;
    }

    public void OnTrackPadChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, ETrackingResult trackingChanged)
    {

    }


    public bool GetRightGrab()
    {
        return grabAction.GetStateDown(rightHand);
    }

    public float GetRightSqueeze()
    {
        return squeezeAction.GetAxis(rightHand);
    }

    public float GetLeftSqueeze()
    {
        return squeezeAction.GetAxis(leftHand);
    }

    public Vector3 GetRightControllerPositon()
    {
        return rightController.transform.localPosition;
    }

    public Vector3 GetLeftControllerPositon()
    {
        return leftController.transform.localPosition;
    }

    public Quaternion GetRightControllerRotation()
    {
        return rightController.transform.localRotation;
    }

    public Quaternion GetLeftControllerRotation()
    {
        return leftController.transform.localRotation;
    }
}