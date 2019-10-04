using System;
using System.Collections.Generic;
using UnityEngine;

public class IsometricWorldData : ScriptableObject {

    [SerializeField] public String WorldName = "world";
    [SerializeField] public int Rows = 0;
    [SerializeField] public int Columns = 0;
    [SerializeField] private List<IsometricTilePrefabController> TilePrefabs;
    [SerializeField] private List<int> Rotations;
    [SerializeField] public IsometricTrackData TrackData = null;

    public void SetTile(int column, int row, IsometricTileData tile) {
        TilePrefabs[row * Columns + column] = tile.TilePrefab;
        Rotations[row * Columns + column] = tile.Rotation;
    }

    public void InsertWest(IsometricTileData tile) {
        List<List<IsometricTileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].Insert(0, tile);
        }
        SetTileData(data);
    }

    public void InsertNorth(IsometricTileData tile) {
        List<List<IsometricTileData>> data = GetTileData();
        List<IsometricTileData> newRow = new List<IsometricTileData>();
        for (int i = 0; i < Columns; i++) {
            newRow.Add(tile);
        }
        data.Insert(0, newRow);
        SetTileData(data);
    }

    public void InsertEast(IsometricTileData tile) {
        List<List<IsometricTileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].Add(tile);
        }
        SetTileData(data);
    }

    public void InsertSouth(IsometricTileData tile) {
        List<List<IsometricTileData>> data = GetTileData();
        List<IsometricTileData> newRow = new List<IsometricTileData>();
        for (int i = 0; i < Columns; i++) {
            newRow.Add(tile);
        }
        data.Add(newRow);
        SetTileData(data);
    }

    public void DeleteWest() {
        if (Columns <= 1) {
            return;
        }
        List<List<IsometricTileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].RemoveAt(0);
        }
        SetTileData(data);
    }

    public void DeleteNorth() {
        if (Rows <= 1) {
            return;
        }
        List<List<IsometricTileData>> data = GetTileData();
        data.RemoveAt(0);
        SetTileData(data);
    }

    public void DeleteEast() {
        if (Columns <= 1) {
            return;
        }
        List<List<IsometricTileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].RemoveAt(data[i].Count - 1);
        }
        SetTileData(data);
    }
    public void DeleteSouth() {
        if (Rows <= 1) {
            return;
        }
        List<List<IsometricTileData>> data = GetTileData();
        data.RemoveAt(data.Count - 1);
        SetTileData(data);
    }

    public void SetTileData(List<List<IsometricTileData>> tileData) {
        List<IsometricTilePrefabController> list = new List<IsometricTilePrefabController>();
        List<int> rotations = new List<int>();
        for (int i = 0; i < tileData.Count; i++) {
            for (int j = 0; j < tileData[i].Count; j++) {
                list.Add(tileData[i][j].TilePrefab);
                rotations.Add(tileData[i][j].Rotation);
            }
        }
        Rows = tileData.Count;
        Columns = tileData[0].Count;
        TilePrefabs = list;
        Rotations = rotations;
    }

    public List<List<IsometricTileData>> GetTileData() {
        List<List<IsometricTileData>> list = new List<List<IsometricTileData>>();
        int index = 0;
        for (int i = 0; i < Rows; i++) {
            List<IsometricTileData> row = new List<IsometricTileData>();
            for (int j = 0; j < Columns; j++) {
                row.Add(new IsometricTileData(TilePrefabs[index], Rotations[index]));
                index++;
            }
            list.Add(row);
        }
        return list;
    }

    public void Rotate() {
        List<List<IsometricTileData>> oldData = GetTileData();
        List<List<IsometricTileData>> newData = new List<List<IsometricTileData>>();
        for (int i = 0; i < Columns; i++) {
            List<IsometricTileData> row = new List<IsometricTileData>();
            for (int j = Rows - 1; j >= 0; j--) {
                oldData[j][i].Rotation = (oldData[j][i].Rotation + 1) % 4;
                row.Add(oldData[j][i]);
            }
            newData.Add(row);
        }
        SetTileData(newData);
    }
}
