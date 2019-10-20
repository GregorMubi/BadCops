using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseDialogController : MonoBehaviour
{
    public void Update()
    {
        //if (Input.GetKey(KeyCode.Escape))
        //{
        //    Resume();
        //}
    }

    public void Resume() {
        GameManager.Instance.PauseGame(false);
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    public void Exit() {

    }
}
