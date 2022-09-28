using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    Scene currentScene;

    void Start() {
        currentScene = SceneManager.GetActiveScene();
    }

    public void StartGame() {
        int index = currentScene.buildIndex + 1;
        SceneManager.LoadScene(index);
    }
}
