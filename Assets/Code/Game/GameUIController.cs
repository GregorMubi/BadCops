using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour {
    [SerializeField] private Slider BadAssSlider = null;
    [SerializeField] private AudioSource WinAudio = null;

    private float GameDuration = 0.0f;
    private bool DidWin = false;

    public void Start() {
        GameDuration = 0f;
    }

    public void Update() {
        GameManager.Instance.AddBadAssPoints(-0.001f);
        BadAssSlider.value = GameManager.Instance.GetBadAssSlider();

        if (!GameManager.Instance.IsPaused()) {
            GameDuration += Time.deltaTime;
        }

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
