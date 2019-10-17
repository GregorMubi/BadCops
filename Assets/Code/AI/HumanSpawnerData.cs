using System;
using UnityEngine;

[Serializable]
public class HumanSpawnerData {
    public int MaxNumberOfHumans = 0;
    public float SpawnRadius = 0.0f;
    public float SpawnCooldown = 0.0f;
    public float MaxDistanceFromSpawn = 0.0f;
    public Vector3 Position = Vector3.zero;
}
