using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {
    public Camera Camera;
    public RenderTexture rt;
    public TileSetData TileSetData;

    private static string TileIconPath = "Assets/Resources/Prefabs/TileIcons/";
    private static string BuildingIconPath = "Assets/Resources/Prefabs/BuildingIcons/";

    void Start() {
        //CreateRenderTexturesOfTiles(TileSetData.Tiles, TileIconPath);
        //CreateRenderTexturesOfTiles(TileSetData.BuildingTiles, BuildingIconPath);
    }

    private void CreateRenderTexturesOfTiles(TileController[] tiles, string path) {
        for (int i = 0; i < tiles.Length; i++) {
            TileController tileController = Instantiate<TileController>(tiles[i]);
            Camera.Render();
            string prefabName = tileController.name;
            prefabName = prefabName.Replace("(Clone)", "");
            string saveToPath = path + prefabName + ".png";
            SaveRtToFile(saveToPath);
            tiles[i].Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(saveToPath);
            DestroyImmediate(tileController.gameObject);
        }
    }

    private void SaveRtToFile(String path) {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
    }

}

