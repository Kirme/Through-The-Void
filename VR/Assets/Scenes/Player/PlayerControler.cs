using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerControler : MonoBehaviour
{

    private SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    private SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Pose poseAction;
    public GameObject rightController, leftController, joystick;

    public float maxSpeed = 25f, acceleration = 2.5f, maxTurnSpeed = 12.5f, turnAcceleration = 2.5f;
    
    // Use these to change max speed dynamically, for example if a fault causes rotation speed to decrease.
    private float maxSpeedModifier = 1.0f, maxTurnSpeedModifier = 1.0f;

    public float xRotDeadzone = 10.0f, zRotDeadzone = 10.0f, yRotDeadzone = 10.0f;
    public float maxSteeringRot = 70.0f;
    
    private float speed = 0f;
    private Vector3 rotationSpeed = new Vector3(0,0,0);
    
    private bool grabbingRight = false;


    private Quaternion defaultControllerRot = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {

        grabAction[rightHand].onChange += OnGrabChanged;
        poseAction[rightHand].onTrackingChanged += OnTrackPadChanged;

    }

    // Update is called once per frame
    void Update()
    {

        float xRot = 0;
        float yRot = 0;
        float zRot = 0;

        // Only rotate if grabbing
        if(grabbingRight){
            //Debug.Log(GetRightControllerRotation());

            Quaternion relativeRotation = Quaternion.Inverse(GetRightControllerRotation()) * defaultControllerRot;

            xRot = relativeRotation.eulerAngles.x;
            yRot = relativeRotation.eulerAngles.y;
            zRot = relativeRotation.eulerAngles.z;
            joystick.transform.localRotation = Quaternion.Euler(-xRot, -zRot, yRot);


            // Handle movement around the x-axis
            if (xRot > 180){
                xRot -= 360;
            }

            xRot = Mathf.Clamp(xRot, -maxSteeringRot, maxSteeringRot);
            if(Mathf.Abs(xRot) > xRotDeadzone){
                if(xRot > xRotDeadzone){
                    xRot = (xRot - xRotDeadzone)/(maxSteeringRot - xRotDeadzone);
                } else if(xRot < -xRotDeadzone){
                    xRot = (xRot + xRotDeadzone)/(maxSteeringRot - xRotDeadzone);
                }

            } else {
                xRot = 0;
            }

            // Handle movement around the y-axis
            if(yRot > 180){
                yRot -= 360;
            }

            yRot = Mathf.Clamp(yRot, -maxSteeringRot, maxSteeringRot);
            if(Mathf.Abs(yRot) > yRotDeadzone){
                if(yRot > yRotDeadzone){
                    yRot = (yRot - yRotDeadzone)/(maxSteeringRot - yRotDeadzone);
                } else if(yRot < -yRotDeadzone){
                    yRot = (yRot + yRotDeadzone)/(maxSteeringRot - yRotDeadzone);
                }

            } else {
                yRot = 0;
            }

            // Handle movement around the z-axis
            if(zRot > 180){
                zRot -= 360;
            }

            zRot = Mathf.Clamp(zRot, -maxSteeringRot, maxSteeringRot);
            if(Mathf.Abs(zRot) > zRotDeadzone){
                if(zRot > zRotDeadzone){
                    zRot = (zRot - zRotDeadzone)/(maxSteeringRot - zRotDeadzone);
                } else if(zRot < -zRotDeadzone){
                    zRot = (zRot + zRotDeadzone)/(maxSteeringRot - zRotDeadzone);
                }

            }else {
                zRot = 0;
            }


        } else
        {
            joystick.transform.localRotation = Quaternion.identity;
        }

        rotationSpeed.x = Mathf.Lerp(rotationSpeed.x, -maxTurnSpeed * xRot * Time.deltaTime, turnAcceleration * Time.deltaTime);
        rotationSpeed.y = Mathf.Lerp(rotationSpeed.y, -maxTurnSpeed * yRot * Time.deltaTime, turnAcceleration * Time.deltaTime);
        rotationSpeed.z = Mathf.Lerp(rotationSpeed.z, maxTurnSpeed/5 * zRot * Time.deltaTime, turnAcceleration * Time.deltaTime);

        speed = Mathf.Lerp(speed, maxSpeed * GetRightSqueeze() * maxSpeedModifier, acceleration * Time.deltaTime * maxSpeedModifier);

        transform.Rotate(rotationSpeed.x * maxTurnSpeedModifier, rotationSpeed.y * maxTurnSpeedModifier, rotationSpeed.z * maxTurnSpeedModifier, Space.Self);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void FaultsChanged()
    {

    }

    public void OnGrabChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (newState)
        {
            //Reset Default Rotation of controllers (in order to allow individual default rotations of controllers for players.)
            defaultControllerRot = GetRightControllerRotation();
        }

        grabbingRight = newState;
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