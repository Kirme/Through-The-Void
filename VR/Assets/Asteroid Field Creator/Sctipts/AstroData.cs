﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------
// ASTEROID FIELD CREATOR
//-----------------------------------------------------
// By: jandd661
// Contact: jandd661@gmail.com
//-----------------------------------------------------

//-----------------------------------------------------
// INSTRUCTIONS:
//-----------------------------------------------------
// 1. See the comments in AsteroidFieldCreator.cs or
// the PDF included with this package.
//-----------------------------------------------------

[ExecuteInEditMode]
public class AstroData : MonoBehaviour
{
    public float BufferZoneDiameter = 2f;
    public Rigidbody RigidbodyToScale = null;
    public bool ShowBufferGizmo = true;

    [SerializeField]
    private Color BufferZoneColor = Color.magenta;

    public bool DoNotDelete = false;
    public bool DoNotScale = false;
    public bool DoNotRotate = false;

    [SerializeField]
    private List<MeshRenderer> meshesToHide = new List<MeshRenderer>();
    public bool ShowMesh = true;
    private bool currentShowMesh = true;
    [SerializeField]
    private List<Collider> collidersToHide = new List<Collider>();
    public bool ShowColliders = true;
    private bool currentShowColliders = true;

    [HideInInspector]
    public bool HasCollision = false;

    private List<GameObject> deleteList = new List<GameObject>();

    //Egen
    private Transform playerTransform;
    private int distanceCap = 500;
    //Leniency was required to some extent to prevent flickering, but might not longer be (or perhaps only needed to a very small degree)
    private float distanceLeniency = 1f;
    private float speed = 2f;
    //As the update runs even when the application is not running and persists between sessions, creating new variables on update can presumably create a memory leak
    //that will not fix itself until the entire asteroid field is recreated.
    private float xRot;
    private float yRot;
    private float zRot;
    private float distanceFactor;
    private bool startUp = true;

    public void Update()
    {
        if(currentShowMesh != ShowMesh)
        {
            ToggleMesh(ShowMesh);
        }
        if(currentShowColliders != ShowColliders)
        {
            ToggleColliders(ShowColliders); 
        }

        //Egen
        
        if(playerTransform == null){
            playerTransform = GameObject.Find("Player").transform;
        }
                            
        //Disable movement if application is not playing
        if(!Application.isPlaying){
            startUp = true;
        }
        else{
            //Done once on application start
            if(startUp){
                xRot = Random.Range(0, 360);
                yRot = Random.Range(0, 360);
                zRot = Random.Range(0, 360);
                transform.Rotate(xRot, yRot, zRot, Space.Self);
                startUp = false;
            }
            //Move the asteroid - TODO: How does this interact with movement from bumping into them?
            transform.position += transform.forward * speed * Time.deltaTime;
            //Check distance, if it's 5% over cap then move
            distanceFactor = GetDistance(this.transform.position, playerTransform.position) / distanceCap;
            if (distanceFactor >= distanceLeniency)
            {
                //Vector3 newpos = playerTransform.position - this.transform.position;
                Vector3 newpos = this.transform.position +  ((playerTransform.position-this.transform.position) * 2 * (1/(distanceFactor)));
                //Should always occur and move it a distance of 2x distance cap towards the player UNLESS the created asteroid field is enormous.
                if(GetDistance(newpos, playerTransform.position) / distanceCap <= 1){
                    this.transform.position = newpos;
                }
                //In which case, it will simply destroy it.
                else{
                    Destroy(gameObject);
                }
                //TODO: Due to fixes in the code, this way of doing it is presumably no longer neccessary (as it was originally done due to a bug), but I cannot test right now.
                //This if/else clause and the content of the else clause can almost certainly just be removed (and then have the content of the if clause always occur).
            }
        }
    }
        

    

    //Egen
    private float GetDistance(Vector3 a, Vector3 b){
        return Mathf.Sqrt(
            Mathf.Pow((a.x - b.x), 2) + 
            Mathf.Pow((a.y - b.y), 2) + 
            Mathf.Pow((a.z - b.z), 2)
        );
    }


    public void CheckPlacement()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetBufferZoneRadius());
        if (hitColliders.Length > 0f)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].transform.IsChildOf(transform) == false)
                {
                    HasCollision = true;
                }
            }
        }

        if (HasCollision == true)
        {
            DestroyImmediate(gameObject);
        }
    }

    public void EnforceMyBufferZone()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, GetBufferZoneRadius());
        if (hitColliders.Length > 0f)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].transform.IsChildOf(transform) == false)
                    {
                    AstroData otherAstroData = hitColliders[i].gameObject.GetComponent<AstroData>();
                    if(otherAstroData != null)
                    {
                        if(otherAstroData.DoNotDelete == false)
                        {
                            deleteList = AddToDeleteList(hitColliders[i].gameObject, deleteList);
                        }
                    }
                    else
                    {
                        Transform parentTransform = hitColliders[i].transform.parent;
                        int loopCount = 0;
                        while (parentTransform != null && loopCount < 100)
                        {
                            AstroData parentAstroData = parentTransform.gameObject.GetComponent<AstroData>();
                            if(parentAstroData != null)
                            {
                                deleteList = AddToDeleteList(parentTransform.gameObject, deleteList);
                                parentTransform = null;
                            }
                            else
                            {
                                parentTransform = parentTransform.parent;
                            }
                            loopCount++;
                        }
                    }
                }
            }
            if(deleteList.Count > 0)
            {
                for (int i = 0; i < deleteList.Count; i++)
                {
                    DestroyImmediate(deleteList[i]);
                }
            }
        }
        deleteList.Clear();
    }

    private List<GameObject> AddToDeleteList(GameObject objectToDelete, List<GameObject> inDeleteList)
    {
        if(inDeleteList.Contains(objectToDelete) == false)
        {
            inDeleteList.Add(objectToDelete);
        }
        return inDeleteList;
    }

    private void ToggleMesh(bool enabled)
    {
        for (int i = 0; i < meshesToHide.Count; i++)
        {
            meshesToHide[i].enabled = enabled;
        }
        currentShowMesh = enabled;
    }

    private void ToggleColliders(bool enabled)
    {
        for (int i = 0; i < collidersToHide.Count; i++)
        {
            collidersToHide[i].enabled = enabled;
        }
        currentShowColliders = enabled;
    }

    private float GetBufferZoneRadius()
    {
        float outSize = BufferZoneDiameter;
        float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        outSize = (BufferZoneDiameter / 2f) * maxScale;
        return outSize;
    }

    private void OnDrawGizmos()
    {
        if(ShowBufferGizmo == true)
        {
            float bufferZone = GetBufferZoneRadius();
            Gizmos.color = BufferZoneColor;
            Gizmos.DrawWireSphere(transform.position, bufferZone);
        }
    }
}
#endif
