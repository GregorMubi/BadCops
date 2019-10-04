using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileData {
    public TilePrefabController TilePrefab = null;
    public int Rotation = 0;

    public TileData(TilePrefabController tilePrefabController, int rotation) {
        TilePrefab = tilePrefabController;
        Rotation = rotation;
    }
}
