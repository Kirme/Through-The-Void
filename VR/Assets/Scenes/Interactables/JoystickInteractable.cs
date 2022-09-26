using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JoystickInteractable : InteractableScript
{
    private PlayerController player;
    public Vector3 attitude = Vector3.zero;

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
        return base.Interact(controller);
    }

    public override void EndInteract()
    {
        transform.localRotation = Quaternion.identity;
        base.EndInteract();
    }

    public override void UpdateInteractable(GameObject controller)
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
