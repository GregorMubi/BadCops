﻿using UnityEngine;

public class TileSetData : ScriptableObject {
    public TileController[] Tiles;
    public bool[] ShowTileInEditor;
    public TileController[] BuildingTiles;
    public TileController[] TilesThatCanHaveBuildingsOnThem;
    public TileController[] TreeTiles;
    public TileController[] TilesThatCanHaveTreesOnThem;
}
