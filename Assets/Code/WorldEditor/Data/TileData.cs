using System;
using UnityEngine;

[Serializable]
public class TileData {
    public float Rotation = 0;
    public Vector3 Position = Vector3.zero;
    public TileController TilePrefab = null;

    public TileData(TileController prefab) {
        TilePrefab = prefab;
    }

    public TileData(TileController prefab, Vector3 position) {
        TilePrefab = prefab;
        Position = position;
    }

    public TileData(TileController prefab, Vector3 position, float rotation) {
        TilePrefab = prefab;
        Position = position;
        Rotation = rotation;
    }
}
