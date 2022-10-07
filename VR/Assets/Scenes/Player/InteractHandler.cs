using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{

    private void Start()
    {
        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Interactable"))
        {
            gameObject.GetComponent<InteractableScript>().onEndInteract.AddListener(InteractEnded);
        }
    }

    public void ResetAll()
    {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Interactable"))
        {
            gameObject.GetComponent<InteractableScript>().Reset();
        }
    }

    private void InteractEnded(InteractableScript interactable)
    {
        Debug.Log("Ended: " + interactable.interactableName);
    }

}
