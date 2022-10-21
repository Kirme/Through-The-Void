using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginReset : MonoBehaviour
{
    
    public void ResetChildrenOrigin(Vector3 offset)
    {
        foreach (Transform child in transform)
        {
            child.position -= offset;
        }
    }

    public void ResetOrigin(Vector3 offset)
    {

        transform.position -= offset;
    }

}
