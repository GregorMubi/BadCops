using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public void Play() {
        SceneManager.LoadScene("Game");
    }
    public void TestWeapons()
    {
        SceneManager.LoadScene("WeaponSandbox");
    }
    public void TestCarMovement()
    {
        SceneManager.LoadScene("CarTest");
    }


    public void Exit() {
        Application.Quit();
    }
}
