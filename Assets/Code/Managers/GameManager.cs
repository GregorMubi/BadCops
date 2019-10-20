using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public float LifePercentage = 0f;
    public int KillCount = 0;
    public bool GamePaused = false;

    public void PauseGame(bool pause) {
        GamePaused = pause;
    }
}
