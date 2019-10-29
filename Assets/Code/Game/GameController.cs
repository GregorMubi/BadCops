using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GameController Instance;
    private bool InputEnabled = false;

    void Start() {
        Instance = this;
    }

    public void SetEnabledInput(bool enabled) {
        InputEnabled = enabled;
    }

    public void Update() {
        UpdateInput();
    }

    private void UpdateInput() {
        if (!InputEnabled) {
            return;
        }
        if (Input.GetKey(KeyCode.Escape)) {
            PauseGame();
        }
    }

    public void PauseGame() {
        if (!GameManager.Instance.IsPaused()) {
            GameManager.Instance.PauseGame(true);
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }
    }

    public void OnLevelCompleted() {
        //TODO
    }

    public void OnLevelRestart() {
        SceneManager.LoadScene("Game");
    }
}
