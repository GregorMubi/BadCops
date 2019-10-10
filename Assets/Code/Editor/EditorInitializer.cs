using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EditorInittializer : MonoBehaviour {

    const string FirstScenePath = "Assets/Scenes/MainMenu.unity";
    private const string PreviousSceneKey = "EditorInittializer-PreviousSceneKey";
    static HashSet<string> excludedScenes = new HashSet<string>();

    static EditorInittializer() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        excludedScenes.Add("Assets/Scenes/LevelEditor.unity");
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange) {
        //change to FirstScene when play is pressed
        if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying && !EditorApplication.isPaused) {
            PlayerPrefs.SetString(PreviousSceneKey, EditorApplication.currentScene);
            PlayerPrefs.Save();

            if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
                EditorApplication.isPlaying = false;
                return;
            }

            if (!excludedScenes.Contains(EditorApplication.currentScene)) {
                EditorSceneManager.OpenScene(FirstScenePath);
            }
        }

        //change to the scene that was launched from
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying && !EditorApplication.isPaused) {
            string scene = PlayerPrefs.GetString(PreviousSceneKey);
            if (!string.IsNullOrEmpty(scene)) {
                EditorSceneManager.OpenScene(scene);
            }
        }
    }
}
