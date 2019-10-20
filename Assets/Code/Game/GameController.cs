using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            PauseGame();
        }

    }

    public void PauseGame()
    {
        if (!GameManager.Instance.GamePaused)
        {
            GameManager.Instance.PauseGame(true);
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }
    }
}
