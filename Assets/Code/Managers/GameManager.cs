using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager {

    private static GameManager instance;

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = new GameManager();
            }
            return instance;
        }
    }

    // parameters
    private float BadAssProcentage = 0.0f;
    private float BadAssGoal = 0.0f;
    public float LifePercentage = 0f;
    private bool GamePaused = false;

    public void SetGoalAtStart(float goal) {
        BadAssGoal = goal;
        BadAssProcentage = goal * 0.5f;
    }

    public void PauseGame(bool pause) {
        GamePaused = pause;
    }

    public bool IsPaused() {
        return GamePaused;
    }

    public void AddBadAssPoints(float points) {
        BadAssProcentage += points;
    }

    public float GetBadAssSlider() {
        return Mathf.Clamp01(BadAssProcentage / BadAssGoal);
    }
}
