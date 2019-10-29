using System.Collections.Generic;
using UnityEngine;

public class WorldData : ScriptableObject {
    public Vector3 SpawnPosition = Vector3.zero;
    public float SpawnRotation = 0f;
    public List<TileData> Tiles;
    public List<HumanSpawnerData> HumanSpawnerDatas;
    public List<CarAIData> CarAIDatas;
    public float BadAssGoal = 10.0f;
    public string LevelName = "level_name";
    public WorldData() {
        Tiles = new List<TileData>();
        HumanSpawnerDatas = new List<HumanSpawnerData>();
        CarAIDatas = new List<CarAIData>();
    }
}
