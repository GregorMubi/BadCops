using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseDialogController : MonoBehaviour {
    public void Update() {
        //if (Input.GetKey(KeyCode.Escape))
        //{
        //    Resume();
        //}
    }

    public void Restart() {
        GameManager.Instance.PauseGame(false);
        SceneManager.UnloadSceneAsync("PauseMenu");
        GameController.Instance.OnLevelRestart();
    }

    public void Resume() {
        GameManager.Instance.PauseGame(false);
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    public void Exit() {
        GameManager.Instance.PauseGame(false);
        SceneManager.UnloadSceneAsync("PauseMenu");
        SceneManager.LoadScene("MainMenu");
    }
}
