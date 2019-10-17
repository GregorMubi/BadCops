using Boo.Lang;
using UnityEngine;

public static class StaticGameHelper {

    private static readonly Color[] Colors = { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };

    public static Color GetRandomColor() {
        int random = Random.Range(0, Colors.Length);
        return Colors[random];
    }
}
