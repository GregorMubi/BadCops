public class IsometricTileData {
    public IsometricTilePrefabController TilePrefab = null;
    public int Rotation = 0;

    public IsometricTileData(IsometricTilePrefabController tilePrefabController, int rotation) {
        TilePrefab = tilePrefabController;
        Rotation = rotation;
    }
}
