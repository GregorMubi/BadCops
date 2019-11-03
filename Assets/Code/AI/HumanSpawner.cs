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

        for (int i = 0; i < data.MaxNumberOfHumans; i++) {
            SpawnHuman();
        }
    }

    public void DestroyHumans() {
        foreach (HumanController human in SpawnedHumans) {
            Destroy(human.gameObject);
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
        Vector3 offset = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        offset.Normalize();
        offset *= Random.Range(0, Data.SpawnRadius);
        Vector3 spawnPosition = transform.position + offset;

        if ((PlayerInputController.GetPlayerCarPosition() - spawnPosition).magnitude < 7.0f) {
            return;
        }

        HumanController newHuman = Instantiate(HumanPrefab);
        newHuman.Init(Data.MaxDistanceFromSpawn, OnHumanDeath);
        newHuman.transform.SetParent(transform);

        newHuman.transform.position = spawnPosition;
        newHuman.transform.position = new Vector3(newHuman.transform.position.x, 0.88f, newHuman.transform.position.z);
        SpawnedHumans.Add(newHuman);
    }

    public void OnHumanDeath(HumanController controller) {
        SpawnedHumans.Remove(controller);
        Destroy(controller.gameObject);
    }

    public List<HumanController> GetHumanControllers() {
        return SpawnedHumans;
    }
}
