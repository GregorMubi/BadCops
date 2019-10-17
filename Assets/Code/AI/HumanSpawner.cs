using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour {

    [SerializeField] private HumanController HumanPrefab = null;

    private float Timer = 0.0f;
    private HumanSpawnerData Data;
    private List<HumanController> SpawnedHumans = new List<HumanController>();

    public void Init(HumanSpawnerData data) {
        Data = data;

        transform.position = data.Position;
        Timer = Data.SpawnCooldown;
    }

    public void DestroyHumans() {
        foreach (HumanController human in SpawnedHumans) {
            Destroy(human);
        }
        SpawnedHumans.Clear();
    }

    void Update() {
        if (Timer > 0) {
            Timer -= Time.deltaTime;
            if (Timer < 0) {
                Timer = Data.SpawnCooldown;
                SpawnHuman();
            }
        }
    }

    private void SpawnHuman() {
        if (SpawnedHumans.Count >= Data.MaxNumberOfHumans) {
            return;
        }
        HumanController newHuman = Instantiate(HumanPrefab);
        newHuman.Init(Data.MaxDistanceFromSpawn);
        newHuman.transform.SetParent(transform);
        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        offset.Normalize();
        offset *= Random.Range(0, Data.SpawnRadius);
        newHuman.transform.position = transform.position + offset;
        newHuman.transform.position = new Vector3(newHuman.transform.position.x, 0.9f, newHuman.transform.position.z);
        SpawnedHumans.Add(newHuman);
    }
}
