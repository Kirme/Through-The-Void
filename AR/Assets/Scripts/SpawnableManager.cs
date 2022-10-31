using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour {
    [SerializeField] ARRaycastManager _RaycastManager;
    [SerializeField] ARPlaneManager _ARPlaneManager;
    List<ARRaycastHit> _Hits = new List<ARRaycastHit>();
    [SerializeField] GameObject spawnablePrefab;

    [SerializeField] Camera arCam;


    private bool notPlaced = true;

    void Update() {
        if (notPlaced && Input.touchCount > 0) {
            PlaceShip();
        }

        TogglePlane();
    }

    private void TogglePlane() {
        if (_ARPlaneManager.enabled != notPlaced)
            _ARPlaneManager.enabled = notPlaced;
    }

    private void PlaceShip() {
        Vector2 touchPos = Input.GetTouch(0).position;
        if (_RaycastManager.Raycast(touchPos, _Hits)) {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                Vector3 touchWorldPos = arCam.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, arCam.nearClipPlane));
                Vector3 hitPos = _Hits[0].pose.position;

                spawnablePrefab.transform.position = new Vector3(hitPos.x, touchWorldPos.y, hitPos.z);
                spawnablePrefab.SetActive(true);
                notPlaced = false;
            }
        }
    }

    public void ResetShip() {
        spawnablePrefab.SetActive(false);
        notPlaced = true;
    }
}
