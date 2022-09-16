using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR;

public class PlayerInteractController : MonoBehaviour
{

    private GameObject interactable;
    private bool grabbing = false;

    public SteamVR_Input_Sources hand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Action_Boolean grabAction;

    private void Start()
    {

        grabAction[hand].onChange += OnGrabChanged;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<InteractableScript>() != null)
        {
            interactable = other.gameObject;
            other.gameObject.GetComponent<Light>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!grabbing && other.gameObject.GetComponent<InteractableScript>() != null)
        {
            CancelInteract(other);
        }

        
    }

    private void CancelInteract(Collider other)
    {
        other.gameObject.GetComponent<Light>().enabled = false;
        interactable = null;
    }


    private void Update()
    {
        if (grabbing)
        {
            interactable.GetComponent<InteractableScript>().UpdateInteractable(gameObject);
        }
    }


    public void OnGrabChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool pressed)
    {
        if (interactable != null)
        {
            if (pressed)
            {
                interactable.GetComponent<InteractableScript>().Interact(gameObject);
                //transform.Find("Model").gameObject.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                interactable.GetComponent<InteractableScript>().EndInteract();
                //transform.Find("Model").gameObject.GetComponent<Renderer>().enabled = true;
                CancelInteract(interactable.GetComponent<Collider>());
                
            }
            grabbing = pressed;
        }
    }


}
