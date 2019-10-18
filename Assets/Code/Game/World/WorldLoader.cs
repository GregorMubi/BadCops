using System.Collections.Generic;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    [SerializeField] private Transform TileParent = null;
    [SerializeField] private Transform HumanSpawnerParent = null;
    [SerializeField] private Transform CarAIParent = null;
    [SerializeField] private PlayerInputController PlayerInputController = null;
    [SerializeField] private CameraController CameraController = null;

    private WorldData WorldData = null;
    private List<TileController> LoadedTiles = new List<TileController>();
    private List<HumanSpawner> HumanSpawners = new List<HumanSpawner>();
    private List<CarAIController> CarAIControllers = new List<CarAIController>();
    private int LoadCarIndex = 6;

    void Start() {
        //order here is important
        WorldData = LevelManager.Instance.GetWorld();
        SetupPlayer();
        LoadWorld(WorldData);
    }
    private void SetupPlayer() {
        List<SimpleCarController> cars = LevelManager.Instance.GetCarsList();
        SimpleCarController car = Instantiate(cars[LoadCarIndex]);
        car.transform.SetParent(PlayerInputController.transform);
        PlayerInputController.Init(car);
        SetPlayerPoistion();
        CameraController.Init(car.gameObject);
    }

    public void SetPlayerPoistion() {
        PlayerInputController.GetCarController().SetTransform(WorldData.SpawnPosition, WorldData.SpawnRotation);
    }

    private void LoadWorld(WorldData worldData) {
        //LOAD TILES
        foreach (TileData tile in worldData.Tiles) {
            TileController tileController = Instantiate(tile.TilePrefab);
            tileController.transform.SetParent(TileParent);
            tileController.transform.eulerAngles = new Vector3(0, tile.Rotation, 0);
            tileController.transform.position = tile.Position;
            LoadedTiles.Add(tileController);
        }

        //LOAD HUMANS
        HumanSpawner humanSpawnerPrefab = LevelManager.Instance.GetHumanSpawnerPrefab();
        foreach (HumanSpawnerData data in worldData.HumanSpawnerDatas) {
            HumanSpawner humanSpawner = Instantiate(humanSpawnerPrefab);
            humanSpawner.transform.SetParent(HumanSpawnerParent);
            humanSpawner.Init(data);
            HumanSpawners.Add(humanSpawner);
        }

        //LOAD CAR AI
        CarAIController carAIControllerPrefab = LevelManager.Instance.GetCarAiControllerPrefab();
        foreach (CarAIData data in worldData.CarAIDatas) {
            CarAIController carAiController = Instantiate(carAIControllerPrefab);
            carAiController.transform.SetParent(CarAIParent);
            carAiController.Init(data);
            CarAIControllers.Add(carAiController);
        }
    }

    private void UnloadWorld() {
        //UNLOAD TILES
        foreach (TileController tile in LoadedTiles) {
            Destroy(tile.gameObject);
        }
        LoadedTiles.Clear();

        //UNLOAD HUMANS
        foreach (HumanSpawner spawner in HumanSpawners) {
            spawner.DestroyHumans();
            Destroy(spawner.gameObject);
        }
        HumanSpawners.Clear();

        //UNLOAD CAR AI
        foreach (CarAIController carAIController in CarAIControllers) {
            Destroy(carAIController.gameObject);
        }
        CarAIControllers.Clear();
    }

    public void LevelCompleted() {
        UnloadWorld();
        LevelManager.Instance.NextLevel();
        WorldData = LevelManager.Instance.GetWorld();
        LoadWorld(WorldData);
        SetPlayerPoistion();
    }

    public void LoadNextCar() {
        Destroy(PlayerInputController.GetCarController().gameObject);
        LoadCarIndex = (LoadCarIndex + 1) % LevelManager.Instance.GetCarsList().Count;
        SetupPlayer();
    }
}
