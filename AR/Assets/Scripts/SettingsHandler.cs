using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] GameObject handler;
    [SerializeField] GameObject settings;

    private SpawnableManager spawnableManager;
    private Client client;

    private bool settingsEnabled = false;

    void Start() {
        spawnableManager = handler.GetComponent<SpawnableManager>();
        client = handler.GetComponent<Client>();
    }

    public void EnableSettings() {
        settingsEnabled = !settingsEnabled;
        settings.SetActive(settingsEnabled);
    }

    public void ResetShip() {
        spawnableManager.ResetShip();
    }

    public void Reconnect() {
        client.Reconnect();
    }
}
