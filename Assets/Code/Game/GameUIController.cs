using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour {
    [SerializeField] private Slider BadAssSlider = null;

    [SerializeField] private List<AudioClip> LevelNumberAudioClips;
    [SerializeField] private AudioSource AudioSource = null;

    public void Start() {
    }

    public void Update() {
        GameManager.Instance.AddBadAssPoints(-0.001f);
        BadAssSlider.value = GameManager.Instance.GetBadAssSlider();
    }

    public void PlayLevelAudio() {
        int currentLevel = LevelManager.Instance.GetLevelNumber();
        AudioSource.clip = LevelNumberAudioClips[currentLevel];
        AudioSource.Play();
    }
}
