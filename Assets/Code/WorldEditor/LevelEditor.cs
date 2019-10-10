using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {
    public Camera Camera;
    public RenderTexture rt;
    public TileSetData TileSetData;

    private static string Path = "Assets/Resources/Prefabs/TileIcons/";

    void Start() {
        //CreateRenderTexturesOfTiles();
    }

    private void CreateRenderTexturesOfTiles() {
        for (int i = 0; i < TileSetData.Tiles.Length; i++) {
            TileController tileController = Instantiate<TileController>(TileSetData.Tiles[i]);
            Camera.Render();
            string prefabName = tileController.name;
            prefabName = prefabName.Replace("(Clone)", "");
            string path = Path + prefabName + ".png";
            SaveRtToFile(path);
            TileSetData.Tiles[i].Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
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

