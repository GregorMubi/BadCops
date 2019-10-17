using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    [SerializeField] private Transform TileParent = null;
    [SerializeField] private Transform HumanSpawnerParent = null;
    [SerializeField] private Rigidbody PlayerRigidBody = null;

    private WorldData WorldData = null;
    private List<TileController> LoadedTiles = new List<TileController>();
    private List<HumanSpawner> HumanSpawners = new List<HumanSpawner>();

    void Start() {
        WorldData = LevelManager.Instance.GetWorld();
        LoadWorld(WorldData);
        SetPlayerPosition();
    }

    void Update() {

    }

    public void SetPlayerPosition() {
        PlayerRigidBody.transform.position = WorldData.SpawnPosition;
        PlayerRigidBody.transform.eulerAngles = new Vector3(0, WorldData.SpawnRotation, 0);
        PlayerRigidBody.velocity = Vector3.zero;
        PlayerRigidBody.angularVelocity = Vector3.zero;
    }

    private void LoadWorld(WorldData worldData) {
        foreach (TileData tile in worldData.Tiles) {
            TileController tileController = Instantiate(tile.TilePrefab);
            tileController.transform.SetParent(TileParent);
            tileController.transform.eulerAngles = new Vector3(0, tile.Rotation, 0);
            tileController.transform.position = tile.Position;
            LoadedTiles.Add(tileController);
        }

        HumanSpawner prefab = LevelManager.Instance.GetHumanSpawnerPrefab();
        foreach (HumanSpawnerData data in worldData.HumanSpawnerDatas) {
            HumanSpawner humanSpawner = Instantiate(prefab);
            humanSpawner.transform.SetParent(HumanSpawnerParent);
            humanSpawner.Init(data);
            HumanSpawners.Add(humanSpawner);
        }
    }

    private void UnloadWorld() {
        foreach (TileController tile in LoadedTiles) {
            Destroy(tile.gameObject);
        }
        LoadedTiles.Clear();

        foreach (HumanSpawner spawner in HumanSpawners) {
            spawner.DestroyHumans();
            Destroy(spawner);
        }
        HumanSpawners.Clear();
    }

    public void LevelCompleted() {
        UnloadWorld();
        LevelManager.Instance.NextLevel();
        WorldData = LevelManager.Instance.GetWorld();
        LoadWorld(WorldData);
        SetPlayerPosition();
    }
}
