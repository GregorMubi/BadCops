using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    [SerializeField] private Transform TileParent = null;
    [SerializeField] private Rigidbody PlayerRigidBody = null;
    private WorldData WorldData = null;

    private List<TileController> LoadedTiles = new List<TileController>();

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
    }

    private void UnloadWorld() {
        foreach (TileController tile in LoadedTiles) {
            Destroy(tile.gameObject);
        }
        LoadedTiles.Clear();
    }

    public void LevelCompleted() {
        UnloadWorld();
        LevelManager.Instance.NextLevel();
        WorldData = LevelManager.Instance.GetWorld();
        LoadWorld(WorldData);
        SetPlayerPosition();
    }
}
