using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsHandler : MonoBehaviour
{
    [SerializeField] GameObject handler;
    [SerializeField] GameObject settings;

    private SpawnableManager spawnableManager;
    private Client client;
    private Scene currentScene;

    private bool settingsEnabled = false;

    void Start() {
        spawnableManager = handler.GetComponent<SpawnableManager>();
        client = handler.GetComponent<Client>();
        currentScene = SceneManager.GetActiveScene();
    }

    public void EnableSettings() {
        settingsEnabled = !settingsEnabled;
        settings.SetActive(settingsEnabled);
    }

    public void ResetShip() {
        spawnableManager.ResetShip();
        EnableSettings();
    }

    public void Reconnect() {
        client.Reconnect();
        EnableSettings();
    }

    public void QuitGame() {
        client.Disconnect();
        int index = currentScene.buildIndex - 1;
        SceneManager.LoadScene(index);
    }
}
