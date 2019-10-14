using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CustomEditor((typeof(WorldEditor)))]
public class WorldEditorInspector : Editor {

    private const String WorldDataPath = "Assets/Resources/Worlds/";
    private WorldEditor WorldEditor;
    private List<TileController> LoadedTiles = new List<TileController>();

    public override void OnInspectorGUI() {
        if (WorldEditor == null) {
            WorldEditor = (WorldEditor)target;
            if (WorldEditor == null) {
                return;
            }
        }
        GUILayout.Space(10f);

        WorldEditor.TileSetData = (TileSetData)EditorGUILayout.ObjectField(WorldEditor.TileSetData, typeof(TileSetData), false);
        WorldEditor.WorldData = (WorldData)EditorGUILayout.ObjectField(WorldEditor.WorldData, typeof(WorldData), false);

        GUILayout.Space(10f);
        GUILayout.Label("World data functions:");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("NEW")) {
            SaveWorld();
            EnterNameWindow.Open("Enter world name:", CreateNewWorld);
        }

        Color startColor = GUI.color;

        if (WorldEditor.WorldData != null && LoadedTiles.Count <= 0) {
            GUI.color = Color.green;
        }
        EditorGUI.BeginDisabledGroup(WorldEditor.WorldData == null);
        if (GUILayout.Button("LOAD")) {
            LoadWorld();
        }
        GUI.color = startColor;

        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(WorldEditor.WorldData == null || LoadedTiles.Count == 0);
        if (GUILayout.Button("UNLOAD")) {
            SaveWorld();
            UnloadWorld();
        }
        if (GUILayout.Button("SAVE")) {
            SaveWorld();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        if (WorldEditor.WorldData == null || LoadedTiles.Count <= 0) {
            return;
        }

        GameObject selection = null;
        try {
            selection = (GameObject)Selection.activeObject; ;
        } catch {
        }
        TileController selectedTile = null;
        if (selection != null) {
            selectedTile = selection.GetComponent<TileController>();
            if (selectedTile == null) {
                if (selection.transform.parent != null) {
                    selectedTile = selection.transform.parent.GetComponent<TileController>();
                }
            }
        }

        GUILayout.Space(10f);
        GUILayout.Label("Tile functions:");
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(selectedTile == null);
        if (GUILayout.Button("DUPLICATE")) {
            TileController newTile = Instantiate(GetPrefab(selectedTile.name));
            newTile.transform.SetParent(WorldEditor.transform);
            newTile.transform.position = selectedTile.transform.position;
            newTile.transform.eulerAngles = selectedTile.transform.eulerAngles;
            LoadedTiles.Add(newTile);
            Selection.activeObject = newTile;
        }

        if (GUILayout.Button("Delete TILE")) {
            LoadedTiles.Remove(selectedTile);
            DestroyImmediate(selectedTile.gameObject);
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        float iconSize = 100.0f;
        int iconsPerRow = Mathf.FloorToInt((Screen.width - 50) / iconSize);
        int iconIndex = 0;
        while (iconIndex < WorldEditor.TileSetData.Tiles.Length) {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < iconsPerRow; i++) {
                if (iconIndex >= WorldEditor.TileSetData.Tiles.Length) {
                    break;
                }
                TileController tileController = WorldEditor.TileSetData.Tiles[iconIndex];
                if (tileController.Icon == null) {
                    LinkIconsToPrefabs();
                }

                if (GUILayout.Button(tileController.Icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize))) {
                    if (selectedTile != null) {
                        TileController newTile = Instantiate(tileController);
                        newTile.transform.SetParent(WorldEditor.transform);
                        newTile.transform.position = selectedTile.transform.position;
                        int tileIndex = LoadedTiles.IndexOf(selectedTile);
                        LoadedTiles[tileIndex] = newTile;
                        DestroyImmediate(selectedTile.gameObject);
                        Selection.activeObject = newTile;
                    } else {
                        TileController newTile = Instantiate(tileController);
                        newTile.transform.SetParent(WorldEditor.transform);
                        newTile.transform.position = WorldEditor.LastSelectedPosition;
                        LoadedTiles.Add(newTile);
                        Selection.activeObject = newTile;
                    }
                    Repaint();

                }
                iconIndex++;
            }
            EditorGUILayout.EndHorizontal();
        }

    }

    public static void LinkIconsToPrefabs() {
        TileSetData tsd = AssetDatabase.LoadAssetAtPath<TileSetData>("Assets/Resources/Prefabs/tileSet.asset");
        for (int i = 0; i < tsd.Tiles.Length; i++) {
            string prefabName = tsd.Tiles[i].name;
            string path = "Assets/Resources/Prefabs/TileIcons/" + prefabName + ".png";
            tsd.Tiles[i].Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }

    private void CreateNewWorld(String name) {
        WorldEditor.WorldData = new WorldData();
        TileData initialBlock = new TileData(WorldEditor.TileSetData.EmptyTile);
        WorldEditor.WorldData.Tiles.Add(initialBlock);

        AssetDatabase.CreateAsset(WorldEditor.WorldData, WorldDataPath + name + ".asset");
        SaveWorld();
        LoadWorld();
    }

    private void SaveWorld() {
        if (LoadedTiles.Count == 0) {
            return;
        }

        if (WorldEditor != null && WorldEditor.WorldData != null) {
            WorldEditor.WorldData.Tiles.Clear();
            for (int i = 0; i < LoadedTiles.Count; i++) {
                WorldEditor.WorldData.Tiles.Add(new TileData(GetPrefab(LoadedTiles[i].name), LoadedTiles[i].transform.position, LoadedTiles[i].transform.eulerAngles.y));
            }

            EditorUtility.SetDirty(WorldEditor.WorldData);
            AssetDatabase.SaveAssets();
        }
    }

    private TileController GetPrefab(string name) {
        name = name.Replace("(Clone)", "");
        for (int i = 0; i < WorldEditor.TileSetData.Tiles.Length; i++) {
            if (WorldEditor.TileSetData.Tiles[i].name == name) {
                return WorldEditor.TileSetData.Tiles[i];
            }
        }

        return null;
    }

    private void LoadWorld() {
        if (WorldEditor.WorldData == null) {
            return;
        }

        UnloadWorld();

        foreach (TileData tile in WorldEditor.WorldData.Tiles) {
            TileController tileController = Instantiate(tile.TilePrefab);
            tileController.transform.SetParent(WorldEditor.transform);
            tileController.transform.eulerAngles = new Vector3(0, tile.Rotation, 0);
            tileController.transform.position = tile.Position;
            LoadedTiles.Add(tileController);
        }

    }

    private void UnloadWorld() {
        if (WorldEditor.WorldData == null) {
            return;
        }

        foreach (TileController tile in LoadedTiles) {
            DestroyImmediate(tile.gameObject);
        }
        LoadedTiles.Clear();

        List<GameObject> demons = new List<GameObject>();
        for (int i = 0; i < WorldEditor.transform.childCount; i++) {
            demons.Add(WorldEditor.transform.GetChild(i).gameObject);
        }
        foreach (GameObject demon in demons) {
            DestroyImmediate(demon);
        }
    }
}
