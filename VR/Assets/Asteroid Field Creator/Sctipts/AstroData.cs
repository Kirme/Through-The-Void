#if UNITY_EDITOR
using System.Collections.Generic;
using System.Collections;
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
// the PDF included with this package.-
//-----------------------------------------------------

[ExecuteInEditMode]
public class AstroData : MonoBehaviour
{
    public float BufferZoneDiameter = 2f;
    public Rigidbody RigidbodyToScale = null;
    public bool ShowBufferGizmo = true;
    //private bool HitboxEnabled = true;

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
    private bool resettingPosition = false;
    private Vector3 respawnOffset;
    private double remainingOffset = 1.0;
    float resetTimer = 3;
    float currTimer = 0f;
    float asteroidSpawnDistanceFactor = 10f;

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

        
        if(playerTransform == null){
            playerTransform = GameObject.Find("Player").transform;
        }
                            
        //Disable movement if application is not playing
        if(!Application.isPlaying){
            startUp = true;
        }
        else{
            if(resettingPosition){
                float t = Mathf.Min(1, Time.deltaTime / resetTimer);
                this.transform.position -= Vector3.Lerp(Vector3.zero, respawnOffset, t);
                remainingOffset -= t;

                currTimer += Time.deltaTime;
            }
            else{

            
                //Done once on application start
                if(startUp){
                    xRot = Random.Range(0, 360);
                    yRot = Random.Range(0, 360);
                    zRot = Random.Range(0, 360);
                    transform.Rotate(xRot, yRot, zRot, Space.Self);
                    startUp = false;
                    speed = 2f;
                }
                //Move the asteroid - TODO: How does this interact with movement from bumping into them?
                transform.position += transform.forward * speed * Time.deltaTime;
                //Check distance, if it's 5% over cap then move
                distanceFactor = GetDistance(this.transform.position, playerTransform.position) / distanceCap;
                if (distanceFactor >= distanceLeniency)
                {
                    //Vector3 newpos = playerTransform.position - this.transform.position;
                    Vector3 newPos = this.transform.position + ((playerTransform.position-this.transform.position) * 2 * (1/(distanceFactor)));
                    

                    //Should always occur and move it a distance of 2x distance cap towards the player UNLESS the created asteroid field is enormous.
                    if(GetDistance(newPos, playerTransform.position) / distanceCap <= 1){ //Can probably be removed?
                        respawnOffset = ((playerTransform.position - this.transform.position) * 2 * asteroidSpawnDistanceFactor * (1 / (distanceFactor))) ;
                        remainingOffset = 1.0;
                        this.transform.position = newPos + respawnOffset;
                        xRot = Random.Range(0, 360);
                        yRot = Random.Range(0, 360);
                        zRot = Random.Range(0, 360);
                        transform.Rotate(xRot, yRot, zRot, Space.Self);
                        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                        speed = 2f;
                        resettingPosition = true;
                    }
                }
            }
            if(currTimer >= resetTimer){
                this.transform.position -= Vector3.Lerp(Vector3.zero, respawnOffset, ((float) remainingOffset));
                resettingPosition = false;
                currTimer = 0f;
            }
        }
    }
        


    

    //Egen
    /*
    public void OnCollisionEnter(Collision col){
        if(HitboxEnabled){
            if(col.gameObject.tag == "PlayerCollider"){
                //TODO: Adjust e.g. movement on collision with player.
                HitboxEnabled = false;
                StartCoroutine(EnableHitbox()); //Reactivates hitbox after 3 seconds currently
            }
        }
    }

    IEnumerator EnableHitbox()
    {
        yield return new WaitForSeconds(3f);
        HitboxEnabled = true;
        yield break;
    }
    */


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
