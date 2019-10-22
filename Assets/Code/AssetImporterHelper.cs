using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AssetImporterHelper : MonoBehaviour {

    [SerializeField] private bool DoIt = false;
    [SerializeField] private GameObject[] objects;

    private String path = "Assets/Resources/Prefabs/Tiles/";

    void Update() {
        if (DoIt) {
            DoIt = false;
            CreateDataFromOBJ();
        }
    }

    //[MenuItem("Cool guy menu/Create data from .obj")]
    private void CreateDataFromOBJ() {
        foreach (GameObject obj in objects) {
            for (int i = 0; i < obj.transform.childCount; i++) {
                Transform child = obj.transform.GetChild(i);
                child.position = new Vector3(0.5f, 0, 0.5f);
            }
            //PrefabUtility.SaveAsPrefabAsset(obj, path + obj.name + ".prefab");
        }
    }


}
