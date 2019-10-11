﻿using System.Collections.Generic;
using UnityEngine;

public class WorldData : ScriptableObject {
    public Vector3 SpawnPosition = Vector3.zero;
    public float SpawnRotation = 0f;
    public List<TileData> Tiles;
    public WorldData() {
        Tiles = new List<TileData>();
    }
}
