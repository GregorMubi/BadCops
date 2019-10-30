using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameController : MonoBehaviour {

    [SerializeField] private PlayableDirector CompletedTimeline = null;

    public static GameController Instance;

    private bool InputEnabled = false;
    private bool DidWin = false;
    private bool GoToMainMenu = false;

    void Start() {
        Instance = this;
    }

    public void SetEnabledInput(bool enabled) {
        InputEnabled = enabled;
    }

    public void Update() {
        UpdateInput();
        CheckWinConditions();
    }

    private void UpdateInput() {
        if (!InputEnabled) {
            return;
        }
        if (Input.GetKey(KeyCode.Escape)) {
            PauseGame();
        }
    }

    public void PauseGame() {
        if (!GameManager.Instance.IsPaused()) {
            GameManager.Instance.PauseGame(true);
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }
    }

    public void OnLevelRestart() {
        SceneManager.LoadScene("Game");
    }

    private void CheckWinConditions() {
        if (DidWin) {
            return;
        }
        if (GameManager.Instance.GetBadAssSlider() >= 1.0f) {
            DidWin = true;
            SetEnabledInput(false);
            PlayerInputController.Instance.SetControllEnabled(false);
            PlayerInputController.Instance.GetCarController().ForceBreak();
            CompletedTimeline.Play();

            if (LevelManager.Instance.IsLastLevel()) {
                GoToMainMenu = true;
            } else {
                LevelManager.Instance.NextLevel();
            }
        }
    }

    public void OnLoadNextLevel() {
        if (GoToMainMenu) {
            SceneManager.LoadScene("MainMenu");
        } else {
            SceneManager.LoadScene("Game");
        }
    }
}
