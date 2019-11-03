using System.Collections.Generic;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    [SerializeField] private Transform TileParent = null;
    [SerializeField] private Transform HumanSpawnerParent = null;
    [SerializeField] private Transform CarAIParent = null;
    [SerializeField] private PlayerInputController PlayerInputController = null;
    [SerializeField] private CameraController CameraController = null;
    [SerializeField] private TileSetData TileSetData = null;
    [SerializeField] private GameStartUIController GameStartUIController = null;

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

        //play animation at start
        PlayerInputController.SetControllEnabled(false);
        GameStartUIController.Init(LevelManager.Instance.GetLevelNumber(), WorldData.LevelName);

        LevelManager.Instance.worldLoader = this;
    }
    private void SetupPlayer() {
        List<SimpleCarController> cars = LevelManager.Instance.GetCarsList();
        SimpleCarController car = Instantiate(cars[LoadCarIndex]);
        car.transform.SetParent(PlayerInputController.transform);
        PlayerInputController.Init(car);
        SetPlayerPoistion();
        CameraController.Init(car.gameObject);
    }

    public void OnGameStarted() {
        GameController.Instance.SetEnabledInput(true);
        PlayerInputController.SetControllEnabled(true);
    }

    public void SetPlayerPoistion() {
        PlayerInputController.GetCarController().SetTransform(WorldData.SpawnPosition, WorldData.SpawnRotation);
    }

    private void LoadWorld(WorldData worldData) {
        GameManager.Instance.SetGoalAtStart(worldData.BadAssGoal);

        //LOAD TILES
        foreach (TileData tile in worldData.Tiles) {
            TileController tileController = Instantiate(tile.TilePrefab);
            tileController.transform.SetParent(TileParent);
            tileController.transform.eulerAngles = new Vector3(0, tile.Rotation, 0);
            tileController.transform.position = tile.Position;
            LoadedTiles.Add(tileController);

            bool canHaveBuilding = false;
            foreach (TileController tileC in TileSetData.TilesThatCanHaveBuildingsOnThem) {
                if (tile.TilePrefab.name == tileC.name) {
                    canHaveBuilding = true;
                    break;
                }
            }

            if (canHaveBuilding) {
                int chanceToSpawnHouse = 70;
                if (chanceToSpawnHouse > Random.Range(0, 100)) {
                    int randomInt = Random.Range(0, TileSetData.BuildingTiles.Length);
                    bool rotate = Random.Range(0, 2) > 0;
                    TileController buildingTileController = Instantiate(TileSetData.BuildingTiles[randomInt]);
                    buildingTileController.transform.SetParent(tileController.transform);
                    buildingTileController.transform.eulerAngles = new Vector3(0, rotate ? 90 : 0, 0);
                    buildingTileController.transform.localPosition = new Vector3(-0.5f, 0.6f, -0.5f);
                }
            }

            bool canHaveTrees = false;
            foreach (TileController tileC in TileSetData.TilesThatCanHaveTreesOnThem) {
                if (tile.TilePrefab.name == tileC.name) {
                    canHaveTrees = true;
                    break;
                }
            }

            if (canHaveTrees) {
                int chanceToSpawnTrees = 80;
                if (chanceToSpawnTrees > Random.Range(0, 100)) {
                    int randomInt = Random.Range(0, TileSetData.TreeTiles.Length);
                    int rotate = Random.Range(0, 4);
                    TileController treeTileController = Instantiate(TileSetData.TreeTiles[randomInt]);
                    treeTileController.transform.SetParent(tileController.transform, false);
                    treeTileController.transform.eulerAngles = new Vector3(0, 90 * rotate, 0);
                }
            }
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

    public List<GameObject> GetAllNPC() {
        List<GameObject> npcTransforms = new List<GameObject>();
        
        foreach (HumanSpawner spawner in HumanSpawners) {
            foreach (HumanController spawnedHuman in spawner.GetHumanControllers()) {
                npcTransforms.Add(spawnedHuman.gameObject);
            }
        }

        foreach (CarAIController carAIController in CarAIControllers) {
            npcTransforms.Add(carAIController.GetCarController().gameObject);
        }

        return npcTransforms;
    }
}
