using System.Collections.Generic;
using UnityEngine;

public class WorldData : ScriptableObject {
    public List<TileData> Tiles;

    public WorldData() {
        Tiles = new List<TileData>();
    }
}
