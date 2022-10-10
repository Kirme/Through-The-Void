using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class JoystickInteractable : InteractableScript
{
    private PlayerController player;
    public GameObject joystickMesh;
    public Vector3 attitude = Vector3.zero;

    public bool tiltControls = true;

    public float maxPull = 0.1f, maxRot = 75f;
    private Vector3 initialControllerPosition = Vector3.zero;
    private Vector2 initialPosVal = Vector2.zero, val = Vector2.zero;
    public override void Reset()
    {
        val = Vector2.zero;
        transform.localRotation = Quaternion.identity;
        base.Reset();
    }
    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    public override bool Interact(GameObject controller)
    {
        if(player == null)
        {
            Debug.LogError("PlayerController not set in joystick.");
        }
        initialControllerPosition = controller.transform.localPosition;
        return base.Interact(controller);
    }

    public override void EndInteract()
    {
        transform.localRotation = Quaternion.identity;
        base.EndInteract();
    }

    public override void UpdateInteractable(GameObject controller)
    {
        if (tiltControls)
        {
            TiltControls(controller);
        } else
        {
            PullControls(controller);
        }
    }

    private void PullControls(GameObject controller)
    {
        float zRot = 0;

        Quaternion relativeRotation = Quaternion.Inverse(controller.transform.localRotation) * initialControllerRotation;


        zRot = relativeRotation.eulerAngles.z;

        Vector3 relativePosition = controller.transform.localPosition - initialControllerPosition;
        Vector3 projectedNS = Vector3.Project(relativePosition, Vector3.forward);
        Vector3 projectedWE = Vector3.Project(relativePosition, Vector3.left);
        val.x = Mathf.Clamp((projectedNS.magnitude * Mathf.Sign(Vector3.Dot(projectedNS.normalized, Vector3.forward))) / maxPull + initialPosVal.x, -1, 1);
        val.y = Mathf.Clamp((projectedWE.magnitude * Mathf.Sign(Vector3.Dot(projectedWE.normalized, Vector3.left))) / maxPull + initialPosVal.y, -1, 1);

        transform.localRotation = Quaternion.Euler(val.x * maxRot, 0f, val.y * maxRot);
        joystickMesh.transform.localRotation = Quaternion.Euler(0f, -zRot, 0f);

        if (zRot > 180)
        {
            zRot -= 360;
        }
        zRot /= 180.0f;


        attitude = new Vector3(-val.x, -zRot, val.y);
    }

    private void TiltControls(GameObject controller)
    {
        float xRot = 0;
        float yRot = 0;
        float zRot = 0;

        Quaternion relativeRotation = Quaternion.Inverse(controller.transform.localRotation) * initialControllerRotation;

        xRot = relativeRotation.eulerAngles.x;
        yRot = relativeRotation.eulerAngles.y;
        zRot = relativeRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(-xRot, -zRot, yRot);


        // Handle movement around the x-axis
        if (xRot > 180)
        {
            xRot -= 360;
        }

        xRot = Mathf.Clamp(xRot, -player.maxSteeringRot, player.maxSteeringRot);
        if (Mathf.Abs(xRot) > player.xRotDeadzone)
        {
            if (xRot > player.xRotDeadzone)
            {
                xRot = (xRot - player.xRotDeadzone) / (player.maxSteeringRot - player.xRotDeadzone);
            }
            else if (xRot < -player.xRotDeadzone)
            {
                xRot = (xRot + player.xRotDeadzone) / (player.maxSteeringRot - player.xRotDeadzone);
            }

        }
        else
        {
            xRot = 0;
        }

        // Handle movement around the y-axis
        if (yRot > 180)
        {
            yRot -= 360;
        }

        yRot = Mathf.Clamp(yRot, -player.maxSteeringRot, player.maxSteeringRot);
        if (Mathf.Abs(yRot) > player.yRotDeadzone)
        {
            if (yRot > player.yRotDeadzone)
            {
                yRot = (yRot - player.yRotDeadzone) / (player.maxSteeringRot - player.yRotDeadzone);
            }
            else if (yRot < -player.yRotDeadzone)
            {
                yRot = (yRot + player.yRotDeadzone) / (player.maxSteeringRot - player.yRotDeadzone);
            }

        }
        else
        {
            yRot = 0;
        }

        // Handle movement around the z-axis
        if (zRot > 180)
        {
            zRot -= 360;
        }

        zRot = Mathf.Clamp(zRot, -player.maxSteeringRot, player.maxSteeringRot);
        if (Mathf.Abs(zRot) > player.zRotDeadzone)
        {
            if (zRot > player.zRotDeadzone)
            {
                zRot = (zRot - player.zRotDeadzone) / (player.maxSteeringRot - player.zRotDeadzone);
            }
            else if (zRot < -player.zRotDeadzone)
            {
                zRot = (zRot + player.zRotDeadzone) / (player.maxSteeringRot - player.zRotDeadzone);
            }

        }
        else
        {
            zRot = 0;
        }

        attitude = new Vector3(xRot, yRot, zRot);
    }
}
