using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class TileEditor : EditorWindow {

    private TileData currentData = null;
    private String DataPath = "Assets/Resources/TileData/";

    private int startY = 160;
    private int image_size = 80;
    private int offset = 10;

    [MenuItem("Level Editor/Tile Editor")]
    static void Init() {
        TileEditor window = (TileEditor)EditorWindow.GetWindow(typeof(TileEditor));
        window.Show();
    }
    void OnGUI() {
        GUILayout.Label("Select tile:", EditorStyles.boldLabel);
        currentData = (TileData)EditorGUILayout.ObjectField(currentData, typeof(TileData), true);

        if (currentData != null) {
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("North", GUILayout.Width(50));
            currentData.SpriteNorth = (Sprite)EditorGUILayout.ObjectField(currentData.SpriteNorth, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("East", GUILayout.Width(50));
            currentData.SpriteEast = (Sprite)EditorGUILayout.ObjectField(currentData.SpriteEast, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("South", GUILayout.Width(50));
            currentData.SpriteSouth = (Sprite)EditorGUILayout.ObjectField(currentData.SpriteSouth, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("West", GUILayout.Width(50));
            currentData.SpriteWest = (Sprite)EditorGUILayout.ObjectField(currentData.SpriteWest, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("North");
            GUILayout.Label("East");
            GUILayout.Label("South");
            GUILayout.Label("West");
            EditorGUILayout.EndHorizontal();

            if (currentData.SpriteNorth != null) {
                GUI.DrawTexture(new Rect(offset, startY, image_size, image_size), currentData.SpriteNorth.texture, ScaleMode.ScaleToFit);
            }
            if (currentData.SpriteEast != null) {
                GUI.DrawTexture(new Rect(offset * 2 + image_size, startY, image_size, image_size), currentData.SpriteEast.texture, ScaleMode.ScaleToFit);
            }
            if (currentData.SpriteSouth != null) {
                GUI.DrawTexture(new Rect(offset * 3 + image_size * 2, startY, image_size, image_size), currentData.SpriteSouth.texture, ScaleMode.ScaleToFit);
            }
            if (currentData.SpriteWest != null) {
                GUI.DrawTexture(new Rect(offset * 4 + image_size * 3, startY, image_size, image_size), currentData.SpriteWest.texture, ScaleMode.ScaleToFit);
            }

            GUILayout.Space(startY - 40);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create new tile data")) {
            EnterNameWindow.Open("Enter tile name:", OnNewTileNameCreated);
        }
    }

    private void OnNewTileNameCreated(String name) {
        TileData data = (TileData)AssetDatabase.LoadAssetAtPath(DataPath + name, typeof(TileData));
        if (data != null) {
            EditorUtility.DisplayDialog("ERROR", "Tile with name: " + name + " already exits!", "OK");
        } else {
            currentData = ScriptableObject.CreateInstance<TileData>();
            currentData.Name = name;
            AssetDatabase.CreateAsset(currentData, DataPath + name + ".asset");
            currentData = (TileData)AssetDatabase.LoadAssetAtPath(DataPath + name, typeof(TileData));
        }
    }
}
