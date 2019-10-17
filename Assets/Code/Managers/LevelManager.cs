using UnityEditor;

public class LevelManager {

    private static LevelManager instance;
    private LevelData LevelData = null;
    private int Level = 0;

    private const string LevelDataPath = "Assets/Resources/LevelData.asset";

    public static LevelManager Instance {
        get {
            if (instance == null) {
                instance = new LevelManager();
            }
            return instance;
        }
    }
    public void Init() {
        LevelData = AssetDatabase.LoadAssetAtPath<LevelData>(LevelDataPath);
    }

    public WorldData GetWorld() {
        return LevelData.Worlds[Level];
    }

    public HumanSpawner GetHumanSpawnerPrefab() {
        return LevelData.HumanSpawnerPrefab;
    }

    public void ResetLevel() {
        Level = 0;
    }

    public void NextLevel() {
        Level = (Level + 1) % LevelData.Worlds.Count;
    }

}
