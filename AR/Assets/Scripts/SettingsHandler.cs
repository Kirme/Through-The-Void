using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] SpawnableManager spawnableManager;
    [SerializeField] GameObject settings;
    private bool settingsEnabled = false;

    void Start() {
        
    }

    public void EnableSettings() {
        settingsEnabled = !settingsEnabled;
        settings.SetActive(settingsEnabled);
    }

    public void ResetShip() {
        spawnableManager.ResetShip();
    }
}
