using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StaticGameHelper {

    private static readonly Color[] Colors = { Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };

    public static Color GetRandomColor() {
        int random = Random.Range(0, Colors.Length);
        return Colors[random];
    }

    public static double AngleBetweenVectors(Vector3 v1, Vector3 v2) {
        double sin = v1.x * v2.z - v2.x * v1.z;
        double cos = v1.x * v2.x + v1.z * v2.z;
        double angle = Math.Atan2(sin, cos) * Mathf.Rad2Deg;
        return angle;
    }
}
