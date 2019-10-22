using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

[CustomEditor((typeof(WorldEditor)))]
public class WorldEditorInspector : Editor {

    public enum EditType {
        World = 0,
        HumanAI = 1,
        CarAI = 2,
    }

    private const String WorldDataPath = "Assets/Resources/Worlds/";
    private WorldEditor WorldEditor;
    private List<TileController> LoadedTiles = new List<TileController>();
    private EditType CurrentEditType = EditType.World;
    private List<GameObject> HumanSpanerGameObjects = new List<GameObject>();
    private List<List<GameObject>> CarAiGameObjets = new List<List<GameObject>>();
    private int CurrentHumanSpawnerIndex = -1;
    private int CurrentCarAiIndex = -1;

    public override void OnInspectorGUI() {
        if (WorldEditor == null) {
            WorldEditor = (WorldEditor)target;
            if (WorldEditor == null) {
                return;
            }
        }

        GUILayout.Space(10f);

        WorldEditor.TileSetData =
            (TileSetData)EditorGUILayout.ObjectField(WorldEditor.TileSetData, typeof(TileSetData), false);
        WorldEditor.WorldData =
            (WorldData)EditorGUILayout.ObjectField(WorldEditor.WorldData, typeof(WorldData), false);

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

        if (WorldEditor.TileSetData.ShowTileInEditor == null || WorldEditor.TileSetData.ShowTileInEditor.Length !=
            WorldEditor.TileSetData.Tiles.Length) {
            WorldEditor.TileSetData.ShowTileInEditor = new bool[WorldEditor.TileSetData.Tiles.Length];
        }

        GUILayout.Space(5f);
        GUILayout.Label("Select edit type:");
        GUILayout.BeginHorizontal();
        foreach (EditType type in Enum.GetValues(typeof(EditType))) {
            if (type == CurrentEditType) {
                GUI.color = Color.green;
            }
            if (GUILayout.Button(type.ToString())) {
                CurrentEditType = type;
                if (CurrentHumanSpawnerIndex < HumanSpanerGameObjects.Count && CurrentHumanSpawnerIndex >= 0) {
                    if (type == EditType.HumanAI) {
                        HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(true);
                    } else {
                        HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(false);
                    }
                }
                if (CurrentCarAiIndex < CarAiGameObjets.Count && CurrentCarAiIndex >= 0) {
                    if (type == EditType.CarAI) {
                        foreach (GameObject locator in CarAiGameObjets[CurrentCarAiIndex]) {
                            locator.SetActive(true);
                        }
                    } else {
                        foreach (GameObject locator in CarAiGameObjets[CurrentCarAiIndex]) {
                            locator.SetActive(false);
                        }
                    }
                }
            }
            GUI.color = startColor;
        }
        GUILayout.EndHorizontal();

        switch (CurrentEditType) {
            case EditType.World:
                DrawWorldEdit();
                break;
            case EditType.HumanAI:
                DrawHumanAIEdit();
                break;
            case EditType.CarAI:
                DrawCarAiEdit();
                break;
        }
    }

    private void DrawWorldEdit() {
        GameObject selection = null;
        try {
            selection = (GameObject)Selection.activeObject;
            ;
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

        if (CurrentEditType == EditType.World) {
            GUILayout.Space(10f);
            WorldEditor.ShowMode = EditorGUILayout.Toggle("Show Mode:", WorldEditor.ShowMode);
            if (WorldEditor.ShowMode) {
                if (GUILayout.Button("Set all ON")) {
                    for (int i = 0; i < WorldEditor.TileSetData.ShowTileInEditor.Length; i++) {
                        WorldEditor.TileSetData.ShowTileInEditor[i] = true;
                        EditorUtility.SetDirty(WorldEditor.TileSetData);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }

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

        TileController[] tiles = WorldEditor.TileSetData.Tiles;
        while (iconIndex < tiles.Length) {
            EditorGUILayout.BeginHorizontal();
            int drawnButtons = 0;
            while (drawnButtons < iconsPerRow) {
                if (iconIndex >= tiles.Length) {
                    break;
                }
                TileController tileController = tiles[iconIndex];
                if (tileController.Icon == null) {
                    LinkIconsToPrefabs();
                }

                Color startColor = GUI.color;
                if (WorldEditor.ShowMode || WorldEditor.TileSetData.ShowTileInEditor[iconIndex]) {
                    if (!WorldEditor.TileSetData.ShowTileInEditor[iconIndex]) {
                        GUI.color = Color.red;
                    }
                    if (GUILayout.Button(tileController.Icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize))) {
                        if (WorldEditor.ShowMode) {
                            WorldEditor.TileSetData.ShowTileInEditor[iconIndex] = !WorldEditor.TileSetData.ShowTileInEditor[iconIndex];
                            EditorUtility.SetDirty(WorldEditor.TileSetData);
                            AssetDatabase.SaveAssets();
                        } else {
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
                        }
                        Repaint();
                    }
                    drawnButtons++;
                    GUI.color = startColor;
                }
                iconIndex++;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawBuildingsEdit() {
        GUILayout.Space(5f);
        GUILayout.Label("Work in progress...");
    }

    private void DrawHumanAIEdit() {

        GUILayout.Space(35f);
        GUILayout.Label("--- HUMAN SPAWNER EDIT SECTION ---");
        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("NEW")) {
            if (CurrentHumanSpawnerIndex < HumanSpanerGameObjects.Count && CurrentHumanSpawnerIndex >= 0) {
                HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(false);
            }
            GameObject locator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            locator.transform.SetParent(WorldEditor.transform);
            locator.transform.localScale = Vector3.one * 0.5f;
            locator.transform.position = Vector3.up;
            HumanSpanerGameObjects.Add(locator);
            WorldEditor.WorldData.HumanSpawnerDatas.Add(new HumanSpawnerData());
            CurrentHumanSpawnerIndex = HumanSpanerGameObjects.Count - 1;
            Repaint();
        }

        EditorGUI.BeginDisabledGroup(CurrentHumanSpawnerIndex < 0 || CurrentHumanSpawnerIndex >= HumanSpanerGameObjects.Count);
        if (GUILayout.Button("DELETE")) {
            HumanSpanerGameObjects.Remove(HumanSpanerGameObjects[CurrentHumanSpawnerIndex]);
            WorldEditor.WorldData.HumanSpawnerDatas.RemoveAt(CurrentHumanSpawnerIndex);
            DestroyImmediate(HumanSpanerGameObjects[CurrentHumanSpawnerIndex]);
            Repaint();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        if (WorldEditor.WorldData.HumanSpawnerDatas.Count == 0) {
            return;
        }

        if (CurrentHumanSpawnerIndex >= HumanSpanerGameObjects.Count || CurrentHumanSpawnerIndex < 0) {
            CurrentHumanSpawnerIndex = 0;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawners " + (CurrentHumanSpawnerIndex + 1) + "/" + WorldEditor.WorldData.HumanSpawnerDatas.Count);
        if (GUILayout.Button("<")) {
            HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(false);
            CurrentHumanSpawnerIndex -= 1;
            if (CurrentHumanSpawnerIndex < 0) {
                CurrentHumanSpawnerIndex = WorldEditor.WorldData.HumanSpawnerDatas.Count - 1;
            }
            HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(true);
            Repaint();
        }
        if (GUILayout.Button(">")) {
            HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(false);
            CurrentHumanSpawnerIndex = (CurrentHumanSpawnerIndex + 1) % WorldEditor.WorldData.HumanSpawnerDatas.Count;
            HumanSpanerGameObjects[CurrentHumanSpawnerIndex].SetActive(true);
            Repaint();
        }
        GUILayout.EndHorizontal();

        HumanSpawnerData data = WorldEditor.WorldData.HumanSpawnerDatas[CurrentHumanSpawnerIndex];
        EditorGUI.BeginDisabledGroup(true);
        data.Position = EditorGUILayout.Vector3Field("Position:", data.Position);
        EditorGUI.EndDisabledGroup();
        data.SpawnRadius = EditorGUILayout.FloatField("Radius:", data.SpawnRadius);
        data.SpawnCooldown = EditorGUILayout.FloatField("Cooldown:", data.SpawnCooldown);
        data.MaxNumberOfHumans = EditorGUILayout.IntField("MaxNumberOfHumans:", data.MaxNumberOfHumans);
        data.MaxDistanceFromSpawn = EditorGUILayout.FloatField("MaxDistanceFromSpawn:", data.MaxDistanceFromSpawn);
    }

    private void DrawCarAiEdit() {

        GUILayout.Space(35f);
        GUILayout.Label("--- CAR AI EDIT SECTION ---");
        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("NEW CAR")) {
            if (CurrentCarAiIndex < CarAiGameObjets.Count && CurrentCarAiIndex >= 0) {
                for (int i = 0; i < CarAiGameObjets[CurrentCarAiIndex].Count; i++) {
                    CarAiGameObjets[CurrentCarAiIndex][i].SetActive(false);
                }
            }
            WorldEditor.WorldData.CarAIDatas.Add(new CarAIData());
            CarAiGameObjets.Add(new List<GameObject>());
            CurrentCarAiIndex = CarAiGameObjets.Count - 1;
            Repaint();
        }

        EditorGUI.BeginDisabledGroup(CurrentCarAiIndex < 0 || CurrentCarAiIndex >= CarAiGameObjets.Count);
        if (GUILayout.Button("DELETE CAR")) {
            for (int i = 0; i < CarAiGameObjets[CurrentCarAiIndex].Count; i++) {
                DestroyImmediate(CarAiGameObjets[CurrentCarAiIndex][i]);
            }
            CarAiGameObjets.Remove(CarAiGameObjets[CurrentCarAiIndex]);
            WorldEditor.WorldData.CarAIDatas.RemoveAt(CurrentCarAiIndex);
            Repaint();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        if (WorldEditor.WorldData.CarAIDatas.Count == 0) {
            return;
        }

        if (CurrentCarAiIndex >= CarAiGameObjets.Count || CurrentCarAiIndex < 0) {
            CurrentCarAiIndex = 0;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Car ai: " + (CurrentCarAiIndex + 1) + "/" + WorldEditor.WorldData.CarAIDatas.Count);
        if (GUILayout.Button("<")) {
            for (int i = 0; i < CarAiGameObjets[CurrentCarAiIndex].Count; i++) {
                CarAiGameObjets[CurrentCarAiIndex][i].SetActive(false);
            }
            CurrentCarAiIndex -= 1;
            if (CurrentCarAiIndex < 0) {
                CurrentCarAiIndex = WorldEditor.WorldData.CarAIDatas.Count - 1;
            }
            for (int i = 0; i < CarAiGameObjets[CurrentCarAiIndex].Count; i++) {
                CarAiGameObjets[CurrentCarAiIndex][i].SetActive(true);
            }
            Repaint();
        }
        if (GUILayout.Button(">")) {
            for (int i = 0; i < CarAiGameObjets[CurrentCarAiIndex].Count; i++) {
                CarAiGameObjets[CurrentCarAiIndex][i].SetActive(false);
            }
            CurrentCarAiIndex = (CurrentCarAiIndex + 1) % WorldEditor.WorldData.CarAIDatas.Count;
            for (int i = 0; i < CarAiGameObjets[CurrentCarAiIndex].Count; i++) {
                CarAiGameObjets[CurrentCarAiIndex][i].SetActive(true);
            }
            Repaint();
        }
        GUILayout.EndHorizontal();


        GUILayout.Space(10);
        GUILayout.Label("Current number of keys: " + CarAiGameObjets[CurrentCarAiIndex].Count);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ADD KEY")) {
            GameObject locator = GameObject.CreatePrimitive(PrimitiveType.Cube);
            locator.transform.SetParent(WorldEditor.transform);
            locator.transform.localScale = Vector3.one * 0.5f;
            if (CarAiGameObjets[CurrentCarAiIndex].Count == 0) {
                locator.transform.position = Vector3.up;
            } else {
                locator.transform.position = CarAiGameObjets[CurrentCarAiIndex][CarAiGameObjets[CurrentCarAiIndex].Count - 1].transform.position + Vector3.left + Vector3.forward;
            }
            Selection.activeObject = locator;
            CarAiGameObjets[CurrentCarAiIndex].Add(locator);
            WorldEditor.WorldData.CarAIDatas[CurrentCarAiIndex].Keys.Add(locator.transform.position);
        }

        if (GUILayout.Button("DELETE ALL KEYS")) {
            foreach (GameObject locator in CarAiGameObjets[CurrentCarAiIndex]) {
                DestroyImmediate(locator);
            }
            CarAiGameObjets[CurrentCarAiIndex].Clear();
            WorldEditor.WorldData.CarAIDatas[CurrentCarAiIndex].Keys.Clear();
        }
        GUILayout.EndHorizontal();


        CarAIData data = WorldEditor.WorldData.CarAIDatas[CurrentCarAiIndex];
        data.CarIndex = EditorGUILayout.IntField("Car index:", data.CarIndex);
        data.MotorMultiplier = EditorGUILayout.FloatField("Motor multiplier:", data.MotorMultiplier);
        data.WheelMultiplier = EditorGUILayout.FloatField("Steering multiplier:", data.WheelMultiplier);
    }

    public static void LinkIconsToPrefabs() {
        TileSetData tsd = AssetDatabase.LoadAssetAtPath<TileSetData>("Assets/Resources/Data/tileSet.asset");
        for (int i = 0; i < tsd.Tiles.Length; i++) {
            string prefabName = tsd.Tiles[i].name;
            string path = "Assets/Resources/Prefabs/TileIcons/" + prefabName + ".png";
            tsd.Tiles[i].Icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }

    private void CreateNewWorld(String name) {
        WorldEditor.WorldData = new WorldData();
        TileData initialBlock = new TileData(WorldEditor.TileSetData.Tiles[1]);
        WorldEditor.WorldData.Tiles.Add(initialBlock);

        AssetDatabase.CreateAsset(WorldEditor.WorldData, WorldDataPath + name + ".asset");
        SaveWorld();
        LoadWorld();
    }

    private void SaveWorld() {
        if (WorldEditor == null || WorldEditor.WorldData == null) {
            return;
        }
        //TILES
        if (LoadedTiles.Count > 0) {
            WorldEditor.WorldData.Tiles.Clear();
            for (int i = 0; i < LoadedTiles.Count; i++) {
                WorldEditor.WorldData.Tiles.Add(new TileData(GetPrefab(LoadedTiles[i].name), LoadedTiles[i].transform.position, LoadedTiles[i].transform.eulerAngles.y));
            }
        }
        //HUMAN SPAWNERS
        if (HumanSpanerGameObjects.Count > 0) {
            for (int i = 0; i < HumanSpanerGameObjects.Count; i++) {
                if (WorldEditor.WorldData.HumanSpawnerDatas.Count > i) {
                    WorldEditor.WorldData.HumanSpawnerDatas[i].Position = HumanSpanerGameObjects[i].transform.position;
                }
            }
        }
        //CAR AI
        if (CarAiGameObjets.Count > 0) {
            for (int i = 0; i < CarAiGameObjets.Count; i++) {
                if (WorldEditor.WorldData.CarAIDatas.Count > i) {
                    for (int j = 0; j < CarAiGameObjets[i].Count; j++) {
                        if (WorldEditor.WorldData.CarAIDatas[i].Keys.Count > j) {
                            WorldEditor.WorldData.CarAIDatas[i].Keys[j] = CarAiGameObjets[i][j].transform.position;
                        }
                    }
                }
            }
        }

        EditorUtility.SetDirty(WorldEditor.WorldData);
        AssetDatabase.SaveAssets();
    }

    private TileController GetPrefab(string name) {
        name = name.Replace("(Clone)", "");
        for (int i = 0; i < WorldEditor.TileSetData.Tiles.Length; i++) {
            if (WorldEditor.TileSetData.Tiles[i].name == name) {
                return WorldEditor.TileSetData.Tiles[i];
            }
        }
        for (int i = 0; i < WorldEditor.TileSetData.BuildingTiles.Length; i++) {
            if (WorldEditor.TileSetData.BuildingTiles[i].name == name) {
                return WorldEditor.TileSetData.BuildingTiles[i];
            }
        }
        return null;
    }

    private void LoadWorld() {
        if (WorldEditor.WorldData == null) {
            return;
        }

        UnloadWorld();

        //TILES
        foreach (TileData tile in WorldEditor.WorldData.Tiles) {
            TileController tileController = Instantiate(tile.TilePrefab);
            tileController.transform.SetParent(WorldEditor.transform);
            tileController.transform.eulerAngles = new Vector3(0, tile.Rotation, 0);
            tileController.transform.position = tile.Position;
            LoadedTiles.Add(tileController);
        }

        //HUMAN SPAWNERS
        foreach (HumanSpawnerData data in WorldEditor.WorldData.HumanSpawnerDatas) {
            GameObject locator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            locator.transform.SetParent(WorldEditor.transform);
            locator.transform.localScale = Vector3.one * 0.5f;
            locator.transform.position = data.Position;
            locator.SetActive(false);
            HumanSpanerGameObjects.Add(locator);
        }

        //CAR AI
        foreach (CarAIData data in WorldEditor.WorldData.CarAIDatas) {
            CarAiGameObjets.Add(new List<GameObject>());
            foreach (Vector3 key in data.Keys) {
                GameObject locator = GameObject.CreatePrimitive(PrimitiveType.Cube);
                locator.transform.SetParent(WorldEditor.transform);
                locator.transform.localScale = Vector3.one * 0.5f;
                locator.transform.position = key;
                locator.SetActive(false);
                CarAiGameObjets[CarAiGameObjets.Count - 1].Add(locator);
            }
        }
    }

    private void UnloadWorld() {
        if (WorldEditor.WorldData == null) {
            return;
        }

        //TILES
        foreach (TileController tile in LoadedTiles) {
            DestroyImmediate(tile.gameObject);
        }
        LoadedTiles.Clear();

        //HUMAN SPAWNERS
        foreach (GameObject spawnerGameObject in HumanSpanerGameObjects) {
            DestroyImmediate(spawnerGameObject);
        }
        HumanSpanerGameObjects.Clear();

        //CAR AI
        foreach (List<GameObject> list in CarAiGameObjets) {
            foreach (GameObject key in list) {
                DestroyImmediate(key);
            }
        }
        CarAiGameObjets.Clear();

        //cleanup any demons that might be lurking around (with a silver sword)
        List<GameObject> demons = new List<GameObject>();
        for (int i = 0; i < WorldEditor.transform.childCount; i++) {
            demons.Add(WorldEditor.transform.GetChild(i).gameObject);
        }
        foreach (GameObject demon in demons) {
            DestroyImmediate(demon);
        }
    }
}
