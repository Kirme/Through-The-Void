using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerControler : MonoBehaviour
{

    private SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    private SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Action_Single squeezeAction;
    public GameObject rightController, leftController;

    public float maxSpeed = 25f, acceleration = 2.5f, maxTurnSpeed = 10.0f;

    public float xRotDeadzone = 10.0f, zRotDeadzone = 10.0f, yRotDeadzone = 10.0f;
    public float maxSteeringRot = 60.0f;
    
    private float speed = 0f;


    private Quaternion defaultControllerRot = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // If left trigger pressed, disable controls
        if(GetLeftSqueeze() < 0.8){
            //Debug.Log(GetRightControllerRotation());

            Quaternion relativeRotation = Quaternion.Inverse(GetRightControllerRotation()) * defaultControllerRot;

            float xRot = relativeRotation.eulerAngles.x;
            float zRot = relativeRotation.eulerAngles.z;
            float yRot = relativeRotation.eulerAngles.y;


            // Handle movement around the x-axis
            if(xRot > 180){
                xRot -= 360;
            }

            xRot = Mathf.Clamp(xRot, -maxSteeringRot, maxSteeringRot);
            if(Mathf.Abs(xRot) > xRotDeadzone){
                if(xRot > xRotDeadzone){
                    xRot = (xRot - xRotDeadzone)/(maxSteeringRot - xRotDeadzone);
                } else if(xRot < -xRotDeadzone){
                    xRot = (xRot + xRotDeadzone)/(maxSteeringRot - xRotDeadzone);
                }
                Debug.Log(xRot);

                transform.Rotate(-maxTurnSpeed * xRot * Time.deltaTime, 0f, 0f, Space.Self);
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

                transform.Rotate(0f, 0f, maxTurnSpeed * yRot * Time.deltaTime, Space.Self);
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

                transform.Rotate(0f, -maxTurnSpeed * zRot * Time.deltaTime, 0f, Space.Self);
            }


        } else {
            //Reset Default Rotation of controllers (in order to allow individual default rotations of controllers for players.)
            defaultControllerRot = GetRightControllerRotation();
            Debug.Log("default: " + defaultControllerRot);
        }

        
        speed = Mathf.Lerp(speed, maxSpeed * GetRightSqueeze(), acceleration * Time.deltaTime);

        transform.position += transform.forward * speed * Time.deltaTime;
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