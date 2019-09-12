using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelEditor : EditorWindow {

    // const parameters
    private const String WorldDataPath = "Assets/Resources/Data/Worlds/";
    private const int LeftWidth = 200;
    private const int RightWidth = 340;
    private const int LineHeight = 20;
    private const int Offset = 5;
    private const int LineWidth = 2;

    // local parameters
    private bool IsRightClick = false;
    private int Rotation = 0;
    private Vector2 SscrollPos = Vector2.zero;
    private Vector2 PrevMousePosition = Vector2.zero;
    private Vector2 DrawTileOffset = new Vector2(67, 33.5f);
    private Vector2 StartDrawWorldPosition = Vector2.zero;
    private WorldData WorldData = null;
    private TileSetsData TileSetsData = null;
    private TilePrefabController SelectedTile = null;
    private List<List<Vector2>> TileCenterPositions = new List<List<Vector2>>();

    [MenuItem("Level Editor/Open Editor")]
    static void Init() {
        LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.minSize = new Vector2(1000, 600);
        window.Show();
    }

    void OnGUI() {
        if (TileSetsData == null) {
            TileSetsData = AssetDatabase.LoadAssetAtPath<TileSetsData>("Assets/Resources/Data/TileData/TileDataSet.asset");
        }

        DrawLeftSide();
        GUI.Box(new Rect(LeftWidth, 0, LineWidth, position.height), "");
        DrawMiddle();
        DrawAddRemoveButtons();
        GUI.Box(new Rect(position.width - RightWidth, 0, LineWidth, position.height), "");
        DrawRightSide();
    }

    private void DrawLeftSide() {
        int y = Offset;
        GUI.Label(new Rect(Offset, y, 180, LineHeight), "Level Name:");
        y += LineHeight;
        if (WorldData == null) {
            GUI.Label(new Rect(Offset, y, 180, LineHeight), "Create NEW or LOAD world!");
        } else {
            WorldData.WorldName = GUI.TextField(new Rect(Offset, y, 150, LineHeight), WorldData.WorldName);
        }
        y += LineHeight + Offset;

        WorldData = (WorldData)EditorGUI.ObjectField(new Rect(Offset, y, 180, LineHeight), WorldData, typeof(WorldData), false);
        y += LineHeight + Offset;

        int numberOfButtons = 2;
        float button_width = (LeftWidth - (numberOfButtons + 1) * Offset) / numberOfButtons;
        if (GUI.Button(new Rect(Offset, y, button_width, LineHeight), "NEW")) {
            SaveWorld();
            EnterNameWindow.Open("Enter world name:", CreateNewWorld);
        }
        if (GUI.Button(new Rect(Offset * 2 + button_width, y, button_width, LineHeight), "DELETE")) {
            AssetDatabase.DeleteAsset(WorldDataPath + WorldData.name + ".asset");
            WorldData = null;
            SaveWorld();
        }
        y += LineHeight + Offset;


        EditorGUI.BeginDisabledGroup(true);
        if (GUI.Button(new Rect(Offset, y, button_width, LineHeight), "LOAD")) {
            //TODO
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(WorldData == null);
        if (GUI.Button(new Rect(Offset * 2 + button_width, y, button_width, LineHeight), "SAVE")) {
            SaveWorld();
        }
        EditorGUI.EndDisabledGroup();
        y += LineHeight + Offset;

        GUI.Box(new Rect(0, y, LeftWidth, LineWidth), "");
        y += Offset;
    }

    private void CreateNewWorld(String name) {
        WorldData = new WorldData();
        WorldData.WorldName = name;
        TilePrefabController initialBlock = TileSetsData.EmptyTile;
        List<TileData> line = new List<TileData>();
        List<List<TileData>> row = new List<List<TileData>>();
        TileData tile = new TileData(initialBlock, 0);
        line.Add(tile);
        row.Add((line));
        WorldData.SetTileData(row);
        AssetDatabase.CreateAsset(WorldData, WorldDataPath + name + ".asset");
        SaveWorld();
    }

    private void SaveWorld() {
        if (WorldData != null) {
            EditorUtility.SetDirty(WorldData);
            AssetDatabase.SaveAssets();
        }
    }

    private void DrawMiddle() {
        if (WorldData == null) {
            return;
        }
        HandleTouch();
        GUI.BeginGroup(new Rect(LeftWidth, 0, position.width - LeftWidth - RightWidth, position.height));

        TileCenterPositions.Clear();
        float minBorder = 200;
        float posx = Mathf.Clamp(StartDrawWorldPosition.x, -(WorldData.Columns * 0.5f * DrawTileOffset.x + WorldData.Rows * 0.5f * DrawTileOffset.x), position.width - RightWidth - LeftWidth - minBorder);
        float posy = Mathf.Clamp(StartDrawWorldPosition.y, -(WorldData.Columns * 0.5f * DrawTileOffset.y + WorldData.Rows * 0.5f * DrawTileOffset.y), position.height - DrawTileOffset.y * 4);
        StartDrawWorldPosition = new Vector2(posx, posy);
        List<List<TileData>> tileData = WorldData.GetTileData();
        for (int i = 0; i < tileData.Count; i++) {
            List<Vector2> tileCenterRow = new List<Vector2>();
            Vector2 draw_pos = StartDrawWorldPosition + new Vector2(-DrawTileOffset.x, DrawTileOffset.y) * i;
            for (int j = 0; j < tileData[i].Count; j++) {
                draw_pos += DrawTileOffset;
                Sprite sprite = tileData[i][j].TilePrefab.GetSpriteForRotation(tileData[i][j].Rotation);
                Texture tex = sprite.texture;
                GUI.DrawTexture(new Rect(draw_pos.x, draw_pos.y, tex.width, tex.height), tex);

                Vector2 centerPos = draw_pos + DrawTileOffset + Vector2.right * LeftWidth;
                tileCenterRow.Add(centerPos);
                //Debug.Log("tile " + i + "/" + j + " pos: " + centerPos.x + "/" + centerPos.y);
            }
            TileCenterPositions.Add(tileCenterRow);
        }

        GUI.EndGroup();
    }

    private void HandleTouch() {
        Event e = Event.current;
        if (e == null) {
            return;
        }

        if (e.type == EventType.MouseUp && e.button == 1) {
            IsRightClick = false;
        } else if (e.type == EventType.MouseDown && e.button == 1) {
            IsRightClick = true;
            PrevMousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
        }

        if (IsRightClick) {
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
            Vector2 mouseDelta = mousePosition - PrevMousePosition;
            StartDrawWorldPosition += mouseDelta;
            PrevMousePosition = mousePosition;
            Repaint();
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R) {
            Rotation = (Rotation + 1) % 4;
            Repaint();
        }

        //dont process click if you click outside main window
        if (e.mousePosition.x < LeftWidth || e.mousePosition.y < 0 || e.mousePosition.x > position.width - RightWidth || e.mousePosition.y > position.height) {
            return;
        }

        if (e.type == EventType.MouseDown && e.button == 0) {
            if (SelectedTile != null) {
                Vector2 mousePosition = e.mousePosition;
                //Debug.Log("mouse pos: " + mousePosition.x + "/" + mousePosition.y);
                int closestX = 0;
                int closestY = 0;
                float min_dist = (mousePosition - TileCenterPositions[0][0]).magnitude;
                for (int i = 0; i < TileCenterPositions.Count; i++) {
                    for (int j = 0; j < TileCenterPositions[i].Count; j++) {
                        float dist = (mousePosition - TileCenterPositions[i][j]).magnitude;
                        if (dist < min_dist) {
                            min_dist = dist;
                            closestX = j;
                            closestY = i;
                        }
                    }
                }
                if (min_dist < DrawTileOffset.magnitude) {
                    TileData tile = new TileData(SelectedTile, Rotation);
                    WorldData.SetTile(closestX, closestY, tile);
                    Repaint();
                }
            }
        }
    }

    private void DrawAddRemoveButtons() {
        int buttonSize = 23;

        if (GUI.Button(new Rect(LeftWidth + Offset, Offset, buttonSize, buttonSize), "+")) {
            TileData tile = new TileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertWest(tile);
            StartDrawWorldPosition -= DrawTileOffset;
        }
        if (GUI.Button(new Rect(LeftWidth + Offset + buttonSize + Offset, Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteWest();
            StartDrawWorldPosition += DrawTileOffset;
        }

        if (GUI.Button(new Rect(position.width - RightWidth - Offset * 2 - buttonSize * 2, Offset, buttonSize, buttonSize), "+")) {
            TileData tile = new TileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertNorth(tile);
            StartDrawWorldPosition += new Vector2(DrawTileOffset.x, -DrawTileOffset.y);
        }
        if (GUI.Button(new Rect(position.width - RightWidth - Offset - buttonSize, Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteNorth();
            StartDrawWorldPosition -= new Vector2(DrawTileOffset.x, -DrawTileOffset.y);
        }

        if (GUI.Button(new Rect(LeftWidth + Offset, position.height - buttonSize - Offset, buttonSize, buttonSize), "+")) {
            TileData tile = new TileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertSouth(tile);
        }
        if (GUI.Button(new Rect(LeftWidth + Offset + buttonSize + Offset, position.height - buttonSize - Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteSouth();
        }

        if (GUI.Button(new Rect(position.width - RightWidth - Offset * 2 - buttonSize * 2, position.height - buttonSize - Offset, buttonSize, buttonSize), "+")) {
            TileData tile = new TileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertEast(tile);
        }
        if (GUI.Button(new Rect(position.width - RightWidth - Offset - buttonSize, position.height - buttonSize - Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteEast();
        }
    }

    private void DrawRightSide() {
        float posY = Offset;
        float posX = position.width - RightWidth + Offset;
        float buttonSize = 100;

        SscrollPos = GUI.BeginScrollView(new Rect(position.width - RightWidth, 0, RightWidth, position.height), SscrollPos, new Rect(position.width - RightWidth, 0, RightWidth - 20, Mathf.CeilToInt((float)TileSetsData.Tiles.Length / 3) * (buttonSize + Offset)));

        for (int i = 0; i < TileSetsData.Tiles.Length; i++) {
            Sprite sprite = TileSetsData.Tiles[i].GetSpriteForRotation(Rotation);
            Texture tex = sprite.texture;

            Color startColor = GUI.color;
            if (SelectedTile == TileSetsData.Tiles[i]) {
                GUI.color = Color.green;
            }
            if (GUI.Button(new Rect(posX, posY, buttonSize, buttonSize), tex)) {
                SelectedTile = TileSetsData.Tiles[i];
            }
            GUI.color = startColor;

            posX += buttonSize + Offset;
            if (posX > position.width - 50) {
                posX = position.width - RightWidth + Offset;
                posY += buttonSize + Offset;
            }
        }

        GUI.EndScrollView();
    }



}
