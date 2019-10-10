using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class IsometricLevelEditor : EditorWindow {

    // const parameters
    private const String WorldDataPath = "Assets/Resources/IsometricResources/Worlds/";
    private const int LeftWidth = 200;
    private const int RightWidth = 340;
    private const int LineHeight = 20;
    private const int Offset = 5;
    private const int LineWidth = 2;

    // local parameters
    private bool IsRightClick = false;
    private bool IsLeftClick = false;
    private int Rotation = 0;
    private Vector2 ScrollPos = Vector2.zero;
    private Vector2 PrevMousePosition = Vector2.zero;
    private Vector2 DrawTileOffset = new Vector2(67, 33.5f);
    private Vector2 StartDrawWorldPosition = Vector2.zero;
    private IsometricWorldData WorldData = null;
    private EditType SelectedEdit = EditType.Tiles;

    // edit tiles parameters
    private IsometricTileSetsData TileSetsData = null;
    private IsometricTilePrefabController SelectedTile = null;
    private List<List<Vector2>> TileCenterPositions = new List<List<Vector2>>();

    // edit track parameters
    private int SelectedKeyframeIndex = -1;

    private enum EditType {
        Tiles = 0,
        Building = 1,
        Track = 2,
    }

    [MenuItem("Cool guy menu/Open Level Editor")]
    static void Init() {
        IsometricLevelEditor window = (IsometricLevelEditor)EditorWindow.GetWindow(typeof(IsometricLevelEditor));
        window.minSize = new Vector2(1000, 600);
        window.Show();
    }

    void OnGUI() {
        if (TileSetsData == null) {
            TileSetsData = AssetDatabase.LoadAssetAtPath<IsometricTileSetsData>("Assets/Resources/IsometricResources/TileDataSet.asset");
        }

        DrawLeftSide();
        GUI.Box(new Rect(LeftWidth, 0, LineWidth, position.height), "");
        DrawMiddle();
        if (SelectedEdit == EditType.Tiles) {
            DrawAddRemoveButtons();
        }
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

        WorldData = (IsometricWorldData)EditorGUI.ObjectField(new Rect(Offset, y, 180, LineHeight), WorldData, typeof(IsometricWorldData), false);
        y += LineHeight + Offset;

        int numberOfButtons = 2;
        float button_width = (LeftWidth - (numberOfButtons + 1) * Offset) / numberOfButtons;
        if (GUI.Button(new Rect(Offset, y, button_width, LineHeight), "NEW")) {
            SaveWorld();
            EnterNameWindow.Open("Enter world name:", CreateNewWorld);
        }
        EditorGUI.BeginDisabledGroup(WorldData == null);
        if (GUI.Button(new Rect(Offset * 2 + button_width, y, button_width, LineHeight), "DELETE")) {
            AssetDatabase.DeleteAsset(WorldDataPath + WorldData.name + ".asset");
            WorldData = null;
            SaveWorld();
        }
        EditorGUI.EndDisabledGroup();
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
        WorldData = new IsometricWorldData();
        WorldData.WorldName = name;
        IsometricTilePrefabController initialBlock = TileSetsData.EmptyTile;
        List<IsometricTileData> line = new List<IsometricTileData>();
        List<List<IsometricTileData>> row = new List<List<IsometricTileData>>();
        IsometricTileData tile = new IsometricTileData(initialBlock, 0);
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

        // draw tiles
        TileCenterPositions.Clear();
        float minBorder = 200;
        Vector2 worldSize = new Vector2(WorldData.Columns * 0.5f * DrawTileOffset.x + WorldData.Rows * 0.5f * DrawTileOffset.x, WorldData.Columns * 0.5f * DrawTileOffset.y + WorldData.Rows * 0.5f * DrawTileOffset.y);
        float posx = Mathf.Clamp(StartDrawWorldPosition.x, -worldSize.x, position.width - RightWidth - LeftWidth - minBorder + worldSize.x);
        float posy = Mathf.Clamp(StartDrawWorldPosition.y, -worldSize.y, position.height - DrawTileOffset.y * 4 + worldSize.y);
        StartDrawWorldPosition = new Vector2(posx, posy);
        List<List<IsometricTileData>> tileData = WorldData.GetTileData();
        for (int i = 0; i < tileData.Count; i++) {
            List<Vector2> tileCenterRow = new List<Vector2>();
            Vector2 drawPos = StartDrawWorldPosition + new Vector2(-DrawTileOffset.x, DrawTileOffset.y) * i;
            for (int j = 0; j < tileData[i].Count; j++) {
                drawPos += DrawTileOffset;

                Sprite sprite = tileData[i][j].TilePrefab.GetSpriteForRotation(tileData[i][j].Rotation);
                Texture tex = sprite.texture;
                Vector2 tileDrawPos = drawPos - new Vector2(0, tex.height);
                GUI.DrawTexture(new Rect(tileDrawPos.x, tileDrawPos.y, tex.width, tex.height), tex);

                Vector2 centerPos = tileDrawPos + DrawTileOffset + Vector2.right * LeftWidth;
                tileCenterRow.Add(centerPos);
            }
            TileCenterPositions.Add(tileCenterRow);
        }

        //draw track edit if selected
        if (SelectedEdit == EditType.Track) {
            float keySize = 20;
            for (int i = 0; i < WorldData.TrackData.Keyframes.Count; i++) {
                IsometricTrackKeyframeData keyframe1 = WorldData.TrackData.Keyframes[i];
                Vector2 pos = keyframe1.Position + StartDrawWorldPosition - new Vector2(LeftWidth, 0);
                Color startColor = GUI.color;

                if (i > 0) {
                    IsometricTrackKeyframeData keyframe0 = WorldData.TrackData.Keyframes[i - 1];
                    Vector2 prevPos = keyframe0.Position + StartDrawWorldPosition - new Vector2(LeftWidth, 0);
                    Vector3 p1 = new Vector3(prevPos.x, prevPos.y, 0);
                    Vector3 p2 = new Vector3(pos.x, pos.y, 0);
                    Vector3 t1 = p1 - (new Vector3(Mathf.Sin(keyframe0.Rotation * Mathf.Deg2Rad), Mathf.Cos(keyframe0.Rotation * Mathf.Deg2Rad), 0)).normalized * keyframe0.RotationPower;
                    Vector3 t2 = p2 + (new Vector3(Mathf.Sin(keyframe1.Rotation * Mathf.Deg2Rad), Mathf.Cos(keyframe1.Rotation * Mathf.Deg2Rad), 0)).normalized * keyframe1.RotationPower;
                    Handles.DrawBezier(p1, p2, t1, t2, Color.blue, null, 3);
                }

                GUI.color = i == SelectedKeyframeIndex ? Color.green : Color.red;
                GUI.Box(new Rect(pos.x - keySize * 0.5f, pos.y - keySize * 0.5f, keySize, keySize), "");
                GUI.color = startColor;
            }
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

        if (e.type == EventType.MouseUp && e.button == 0) {
            IsLeftClick = false;
        } else if (e.type == EventType.MouseDown && e.button == 0) {
            IsLeftClick = true;
            PrevMousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
        }

        // stop clicking if mouse exits window
        if (e.type == EventType.MouseLeaveWindow) {
            IsRightClick = false;
            IsLeftClick = false;
        }

        // handle move map
        if (IsRightClick) {
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
            Vector2 mouseDelta = mousePosition - PrevMousePosition;
            StartDrawWorldPosition += mouseDelta;
            PrevMousePosition = mousePosition;
            Repaint();
        }

        // handle rotate map
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R) {
            if (e.shift) {
                WorldData.Rotate();
            } else {
                Rotation = (Rotation + 1) % 4;
            }
            Repaint();
        }


        //dont process click if you click outside main window
        if (e.mousePosition.x < LeftWidth || e.mousePosition.y < 0 || e.mousePosition.x > position.width - RightWidth || e.mousePosition.y > position.height) {
            IsRightClick = false;
            IsLeftClick = false;
            return;
        }

        // handle edit tyles
        if (SelectedEdit == EditType.Tiles) {
            if (e.type == EventType.MouseDown && e.button == 0) {
                if (SelectedTile != null) {
                    Vector2 mousePosition = e.mousePosition;
                    int closestX = 0;
                    int closestY = 0;
                    float minDist = (mousePosition - TileCenterPositions[0][0]).magnitude;
                    for (int i = 0; i < TileCenterPositions.Count; i++) {
                        for (int j = 0; j < TileCenterPositions[i].Count; j++) {
                            float dist = (mousePosition - TileCenterPositions[i][j]).magnitude;
                            if (dist < minDist) {
                                minDist = dist;
                                closestX = j;
                                closestY = i;
                            }
                        }
                    }
                    if (minDist < DrawTileOffset.magnitude) {
                        IsometricTileData tile = new IsometricTileData(SelectedTile, Rotation);
                        WorldData.SetTile(closestX, closestY, tile);
                        Repaint();
                    }
                }
            }
        }

        // handle edit track
        if (SelectedEdit == EditType.Track) {
            if (e.type == EventType.MouseDown && e.button == 0) {
                Vector2 mousePosition = e.mousePosition - StartDrawWorldPosition;
                int keyFrameIndex = -1;
                if (WorldData.TrackData == null) {
                    WorldData.TrackData = new IsometricTrackData();
                }

                if (WorldData.TrackData.Keyframes.Count > 0) {
                    int closestIndex = 0;
                    float minDist = (mousePosition - WorldData.TrackData.Keyframes[0].Position).magnitude;
                    for (int i = 1; i < WorldData.TrackData.Keyframes.Count; i++) {
                        float dist = (mousePosition - WorldData.TrackData.Keyframes[i].Position).magnitude;
                        if (dist < minDist) {
                            minDist = dist;
                            closestIndex = i;
                        }
                    }

                    if (minDist < 30) {
                        keyFrameIndex = closestIndex;
                    }
                }

                if (keyFrameIndex < 0) {
                    WorldData.TrackData.Keyframes.Add(new IsometricTrackKeyframeData(mousePosition, 0, 50, 0));
                    SelectedKeyframeIndex = WorldData.TrackData.Keyframes.Count - 1;
                } else {
                    SelectedKeyframeIndex = keyFrameIndex;
                }
                Repaint();
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape) {
                SelectedKeyframeIndex = -1;
                Repaint();
            }

            if (IsLeftClick && SelectedKeyframeIndex >= 0) {
                Vector2 mousePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
                Vector2 mouseDelta = mousePosition - PrevMousePosition;
                WorldData.TrackData.Keyframes[SelectedKeyframeIndex].Position += mouseDelta;
                PrevMousePosition = mousePosition;
                Repaint();
            }
        }
    }

    private void DrawAddRemoveButtons() {
        int buttonSize = 23;

        if (GUI.Button(new Rect(LeftWidth + Offset, Offset, buttonSize, buttonSize), "+")) {
            IsometricTileData tile = new IsometricTileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertWest(tile);
            StartDrawWorldPosition -= DrawTileOffset;
        }
        if (GUI.Button(new Rect(LeftWidth + Offset + buttonSize + Offset, Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteWest();
            StartDrawWorldPosition += DrawTileOffset;
        }

        if (GUI.Button(new Rect(position.width - RightWidth - Offset * 2 - buttonSize * 2, Offset, buttonSize, buttonSize), "+")) {
            IsometricTileData tile = new IsometricTileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertNorth(tile);
            StartDrawWorldPosition += new Vector2(DrawTileOffset.x, -DrawTileOffset.y);
        }
        if (GUI.Button(new Rect(position.width - RightWidth - Offset - buttonSize, Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteNorth();
            StartDrawWorldPosition -= new Vector2(DrawTileOffset.x, -DrawTileOffset.y);
        }

        if (GUI.Button(new Rect(LeftWidth + Offset, position.height - buttonSize - Offset, buttonSize, buttonSize), "+")) {
            IsometricTileData tile = new IsometricTileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertSouth(tile);
        }
        if (GUI.Button(new Rect(LeftWidth + Offset + buttonSize + Offset, position.height - buttonSize - Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteSouth();
        }

        if (GUI.Button(new Rect(position.width - RightWidth - Offset * 2 - buttonSize * 2, position.height - buttonSize - Offset, buttonSize, buttonSize), "+")) {
            IsometricTileData tile = new IsometricTileData(TileSetsData.EmptyTile, 0);
            WorldData.InsertEast(tile);
        }
        if (GUI.Button(new Rect(position.width - RightWidth - Offset - buttonSize, position.height - buttonSize - Offset, buttonSize, buttonSize), "-")) {
            WorldData.DeleteEast();
        }
    }

    private void DrawRightSide() {
        float posX = position.width - RightWidth + Offset;
        float posY = Offset;

        int editLenght = Enum.GetNames(typeof(EditType)).Length;
        for (int i = 0; i < editLenght; i++) {
            float tabWidth = ((float)(RightWidth - (editLenght + 1) * Offset) / editLenght);
            EditType editType = (EditType)i;
            Color startColor = GUI.color;
            if (SelectedEdit == editType) {
                GUI.color = Color.green;
            }
            if (GUI.Button(new Rect(posX, posY, tabWidth, LineHeight), editType.ToString())) {
                if (SelectedEdit != editType) {
                    SelectedTile = null;
                    SelectedEdit = editType;
                }
            }
            GUI.color = startColor;
            posX += tabWidth + Offset;
        }


        posX = position.width - RightWidth + Offset;
        posY += LineHeight + Offset;

        switch (SelectedEdit) {
            case EditType.Building:
                DrawBuildingEdit(posX, posY);
                break;
            case EditType.Tiles:
                DrawTileEdit(posX, posY);
                break;
            case EditType.Track:
                DrawTrackEdit(posX, posY);
                break;
        }

    }

    private void DrawTileEdit(float posX, float posY) {
        float buttonSize = 100;
        ScrollPos = GUI.BeginScrollView(new Rect(position.width - RightWidth, posY, RightWidth, position.height - posY), ScrollPos, new Rect(position.width - RightWidth, posY, RightWidth - 20, Mathf.CeilToInt((float)TileSetsData.Tiles.Length / 3) * (buttonSize + Offset)));

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


    private void DrawBuildingEdit(float posX, float posY) {
        //TODO

    }

    private void DrawTrackEdit(float posX, float posY) {

        GUI.Box(new Rect(posX, posY + 5, RightWidth - 2 * Offset, 1), "");

        posY += LineHeight;
        if (WorldData == null) {
            EditorGUI.LabelField(new Rect(posX + 20, posY, RightWidth, LineHeight), "Select or create world first!");
            return;
        }

        if (GUI.Button(new Rect(posX, posY, RightWidth - 2 * Offset, LineHeight), "SYNC rotation to carRotation")) {
            for (int i = 0; i < WorldData.TrackData.Keyframes.Count; i++) {
                WorldData.TrackData.Keyframes[i].CarRotation = WorldData.TrackData.Keyframes[i].Rotation;
            }
        }
        posY += LineHeight;

        if (SelectedKeyframeIndex >= WorldData.TrackData.Keyframes.Count) {
            SelectedKeyframeIndex = -1;
        }

        if (SelectedKeyframeIndex < 0) {
            posY += LineHeight;
            EditorGUI.LabelField(new Rect(posX + 20, posY, RightWidth, LineHeight), "Select or add a keyframe!");
            return;
        }

        float tagWidth = 20;
        float intFieldWidth = (((float)RightWidth - Offset) / 2) - tagWidth - Offset;
        float buttonWidth = (float)(RightWidth - Offset) / 2 - Offset;
        IsometricTrackKeyframeData keyframe = WorldData.TrackData.Keyframes[SelectedKeyframeIndex];

        posY += Offset;
        if (GUI.Button(new Rect(posX, posY, buttonWidth, LineHeight), "DELETE")) {
            WorldData.TrackData.Keyframes.Remove(keyframe);
            SelectedKeyframeIndex = -1;
            return;
        }

        EditorGUI.BeginDisabledGroup(SelectedKeyframeIndex == 0);
        if (GUI.Button(new Rect(posX + buttonWidth + Offset, posY, buttonWidth, LineHeight), "INSERT NEW KEY")) {
            IsometricTrackKeyframeData newKey;
            IsometricTrackKeyframeData prevKey = WorldData.TrackData.Keyframes[SelectedKeyframeIndex - 1];
            newKey = new IsometricTrackKeyframeData((prevKey.Position + keyframe.Position) * 0.5f, (prevKey.Rotation + keyframe.Rotation) * 0.5f, (prevKey.RotationPower + keyframe.RotationPower) * 0.5f, (prevKey.Rotation + keyframe.Rotation) * 0.5f);
            WorldData.TrackData.Keyframes.Insert(SelectedKeyframeIndex, newKey);
        }
        EditorGUI.EndDisabledGroup();

        posY += 2 * LineHeight;

        EditorGUI.LabelField(new Rect(posX, posY, RightWidth, LineHeight), "Position");
        posY += LineHeight;
        EditorGUI.LabelField(new Rect(posX, posY, tagWidth, LineHeight), "X:");
        keyframe.Position.x = EditorGUI.FloatField(new Rect(posX + tagWidth, posY, intFieldWidth, LineHeight), keyframe.Position.x);
        EditorGUI.LabelField(new Rect(posX + tagWidth + intFieldWidth + Offset, posY, tagWidth, LineHeight), "Y:");
        keyframe.Position.y = EditorGUI.FloatField(new Rect(posX + 2 * tagWidth + intFieldWidth + Offset, posY, intFieldWidth, LineHeight), keyframe.Position.y);
        posY += 2 * LineHeight + Offset;

        EditorGUI.LabelField(new Rect(posX, posY, RightWidth, LineHeight), "Rotation");
        posY += LineHeight;
        keyframe.Rotation = EditorGUI.Slider(new Rect(posX, posY, RightWidth - 2 * Offset, LineHeight), keyframe.Rotation, 0, 360);
        posY += 2 * LineHeight;
        EditorGUI.LabelField(new Rect(posX, posY, RightWidth, LineHeight), "Power");
        posY += LineHeight;
        keyframe.RotationPower = EditorGUI.Slider(new Rect(posX, posY, RightWidth - 2 * Offset, LineHeight), keyframe.RotationPower, 1, 200);
        posY += 2 * LineHeight;
        EditorGUI.LabelField(new Rect(posX, posY, RightWidth, LineHeight), "CarRotation");
        posY += LineHeight;
        keyframe.CarRotation = EditorGUI.Slider(new Rect(posX, posY, RightWidth - 2 * Offset, LineHeight), keyframe.CarRotation, 0, 360);
    }

}
