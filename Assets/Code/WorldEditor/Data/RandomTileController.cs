using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTileController : TileController {

    [SerializeField] private List<GameObject> Positions = null;
    [SerializeField] private List<GameObject> TreePrefabs = null;
    [SerializeField] private List<GameObject> RockPrefabs = null;
    [Range(0, 1)] public float TreesRocksRatio = 0.9f;

    void Start() {
        for (int i = 0; i < Positions.Count; i++) {
            List<GameObject> Prefabs;
            if (Random.value < TreesRocksRatio) {
                Prefabs = TreePrefabs;
            } else {
                Prefabs = RockPrefabs;
            }
            int random = Random.Range(0, Prefabs.Count);
            GameObject go = Instantiate(Prefabs[random]);
            go.transform.SetParent(Positions[i].transform, false);
            go.transform.localPosition = Vector3.zero;
        }
    }

}
