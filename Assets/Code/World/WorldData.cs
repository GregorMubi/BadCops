using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Lang;

public class WorldData : ScriptableObject {

    [SerializeField] public String WorldName = "world";
    [SerializeField] public int Rows = 0;
    [SerializeField] public int Columns = 0;
    [SerializeField] private List<TilePrefabController> TilePrefabs;
    [SerializeField] private List<int> Rotations;

    public void SetTile(int column, int row, TileData tile) {
        TilePrefabs[row * Columns + column] = tile.TilePrefab;
        Rotations[row * Columns + column] = tile.Rotation;
    }

    public void InsertWest(TileData tile) {
        List<List<TileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].Insert(0, tile);
        }
        SetTileData(data);
    }

    public void InsertNorth(TileData tile) {
        List<List<TileData>> data = GetTileData();
        List<TileData> newRow = new List<TileData>();
        for (int i = 0; i < Columns; i++) {
            newRow.Add(tile);
        }
        data.Insert(0, newRow);
        SetTileData(data);
    }

    public void InsertEast(TileData tile) {
        List<List<TileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].Add(tile);
        }
        SetTileData(data);
    }

    public void InsertSouth(TileData tile) {
        List<List<TileData>> data = GetTileData();
        List<TileData> newRow = new List<TileData>();
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
        List<List<TileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].RemoveAt(0);
        }
        SetTileData(data);
    }

    public void DeleteNorth() {
        if (Rows <= 1) {
            return;
        }
        List<List<TileData>> data = GetTileData();
        data.RemoveAt(0);
        SetTileData(data);
    }

    public void DeleteEast() {
        if (Columns <= 1) {
            return;
        }
        List<List<TileData>> data = GetTileData();
        for (int i = 0; i < data.Count; i++) {
            data[i].RemoveAt(data[i].Count - 1);
        }
        SetTileData(data);
    }
    public void DeleteSouth() {
        if (Rows <= 1) {
            return;
        }
        List<List<TileData>> data = GetTileData();
        data.RemoveAt(data.Count - 1);
        SetTileData(data);
    }

    public void SetTileData(List<List<TileData>> tileData) {
        List<TilePrefabController> list = new List<TilePrefabController>();
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

    public List<List<TileData>> GetTileData() {
        List<List<TileData>> list = new List<List<TileData>>();
        int index = 0;
        for (int i = 0; i < Rows; i++) {
            List<TileData> row = new List<TileData>();
            for (int j = 0; j < Columns; j++) {
                row.Add(new TileData(TilePrefabs[index], Rotations[index]));
                index++;
            }
            list.Add(row);
        }
        return list;
    }
}
