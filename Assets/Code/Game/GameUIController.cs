using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {
    [SerializeField] private Text Timer = null;
    [SerializeField] private Text Life = null;
    [SerializeField] private Slider BadAssSlider = null;
    [SerializeField] private AudioSource WinAudio = null;

    private float GameDuration = 0.0f;
    private bool DidWin = false;

    public void Start() {
        GameDuration = 0f;
    }

    public void Update() {
        Life.text = GameManager.Instance.LifePercentage.ToString();

        GameManager.Instance.AddBadAssPoints(-0.001f);
        BadAssSlider.value = GameManager.Instance.GetBadAssSlider();

        if (!GameManager.Instance.IsPaused()) {
            GameDuration += Time.deltaTime;
        }
        Timer.text = GameDuration.ToString("n2");

        CheckWinConditions();
    }

    private void CheckWinConditions() {
        if (DidWin) {
            return;
        }
        if (GameManager.Instance.GetBadAssSlider() >= 1.0f) {
            DidWin = true;
            WinAudio.Play();
        }
    }
}
