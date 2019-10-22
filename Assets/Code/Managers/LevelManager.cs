using System.Collections.Generic;
using UnityEditor;

public class LevelManager {

    private static LevelManager instance;
    private LevelData LevelData = null;
    private CarsData CarsData = null;
    private WeaponsData WeaponsData = null;
    private int Level = 0;

    private const string LevelDataPath = "Assets/Resources/Data/LevelData.asset";
    private const string CarsDataPath = "Assets/Resources/Data/CarsData.asset";
    private const string WeaponsDataPath = "Assets/Resources/Data/WeaponsData.asset";

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
        CarsData = AssetDatabase.LoadAssetAtPath<CarsData>(CarsDataPath);
        WeaponsData = AssetDatabase.LoadAssetAtPath<WeaponsData>(WeaponsDataPath);
    }

    public WorldData GetWorld() {
        return LevelData.Worlds[Level];
    }

    public List<SimpleCarController> GetCarsList() {
        return CarsData.Cars;
    }

    public HumanSpawner GetHumanSpawnerPrefab() {
        return LevelData.HumanSpawnerPrefab;
    }

    public CarAIController GetCarAiControllerPrefab() {
        return LevelData.CarAIControllerPrefab;
    }

    public void ResetLevel() {
        Level = 0;
    }

    public void NextLevel() {
        Level = (Level + 1) % LevelData.Worlds.Count;
    }

    public WeaponsData GetWeaponDatas() {
        return WeaponsData;
    }
}
