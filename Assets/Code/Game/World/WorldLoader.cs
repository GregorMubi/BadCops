using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    [SerializeField] private WorldData WorldData = null;
    [SerializeField] private Transform TileParent = null;
    [SerializeField] private Transform PlayeTransform = null;

    private List<TileController> LoadedTiles = new List<TileController>();

    void Start() {
        LoadWorld(WorldData);
        SetPlayerPosition();
    }

    void Update() {

    }

    public void SetPlayerPosition() {
        PlayeTransform.position = WorldData.SpawnPosition;
        PlayeTransform.eulerAngles = new Vector3(0, WorldData.SpawnRotation, 0);
    }

    public void LoadWorld(WorldData worldData) {
        foreach (TileData tile in worldData.Tiles) {
            TileController tileController = Instantiate(tile.TilePrefab);
            tileController.transform.SetParent(TileParent);
            tileController.transform.eulerAngles = new Vector3(0, tile.Rotation, 0);
            tileController.transform.position = tile.Position;
            LoadedTiles.Add(tileController);
        }
    }
}
