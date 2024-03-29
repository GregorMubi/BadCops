﻿using System.Collections.Generic;
using UnityEngine;

public class LevelData : ScriptableObject {
    public List<WorldData> Worlds = new List<WorldData>();
    public HumanSpawner HumanSpawnerPrefab;
    public CarAIController CarAIControllerPrefab;
}
