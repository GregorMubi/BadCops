using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    [SerializeField] private WorldData WorldData = null;
    [SerializeField] private Transform TileParent = null;

    //TODO move this to player/car controller
    [SerializeField] private Transform PlayerPositionTransform = null;
    [SerializeField] private Transform PlayerRotationTransform = null;

    private List<TileController> LoadedTiles = new List<TileController>();

    void Start() {
        LoadWorld(WorldData);

        PlayerPositionTransform.position = WorldData.SpawnPosition;
        PlayerRotationTransform.eulerAngles = new Vector3(0, WorldData.SpawnRotation, 0);
    }

    void Update() {

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
