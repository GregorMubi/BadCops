using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GameStartUIController : MonoBehaviour {
    [SerializeField] private Text LevelNumberText = null;
    [SerializeField] private Text LevelNameText = null;
    [SerializeField] private PlayableDirector Timeline = null;

    public void Init(int level, string levelName) {
        LevelNumberText.text = "LEVEL " + (level + 1);
        LevelNameText.text = levelName;
        Timeline.Play();
    }

}
