using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager _RaycastManager;
    List<ARRaycastHit> _Hits = new List<ARRaycastHit>();
    [SerializeField] GameObject spawnablePrefab;

    [SerializeField] Camera arCam;

    private bool notPlaced = true;

    void Update() {
        if (notPlaced && Input.touchCount > 0) {
            PlaceShip();
        }
    }

    private void PlaceShip() {
        if (_RaycastManager.Raycast(Input.GetTouch(0).position, _Hits)) {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                spawnablePrefab.transform.position = _Hits[0].pose.position;
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
