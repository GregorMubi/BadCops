using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoaderController : MonoBehaviour {

    [SerializeField] private WorldData WorldData = null;
    [SerializeField] private Transform PrefabParent = null;
    [SerializeField] private TrackMoveController TrackMoveController = null;

    private Vector3 TileOffset = new Vector3(0.67f, -0.335f, 0);

    void Start() {
        if (WorldData == null) {
            return;
        }

        List<List<TileData>> tileData = WorldData.GetTileData();
        for (int i = 0; i < tileData.Count; i++) {
            Vector3 prefabPosition = new Vector3(-TileOffset.x, TileOffset.y, 0) * i;
            for (int j = 0; j < tileData[i].Count; j++) {
                prefabPosition += TileOffset;

                Sprite sprite = tileData[i][j].TilePrefab.GetSpriteForRotation(tileData[i][j].Rotation);
                Texture tex = sprite.texture;
                Vector3 tilePosition = prefabPosition - new Vector3(0, -tex.height * 0.005f, j * 0.001f + i * 0.001f);

                TilePrefabController tileController = Instantiate<TilePrefabController>(tileData[i][j].TilePrefab, PrefabParent);
                tileController.transform.localPosition = tilePosition;
                tileController.SetRotation(tileData[i][j].Rotation);
            }
        }

        TrackMoveController.SetTrack(WorldData.TrackData);
    }

    void Update() {

    }
}
