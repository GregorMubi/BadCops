using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class CarAIData {
    public int CarIndex = 0;
    public float MotorMultiplier = 1.0f;
    public float WheelMultiplier = 1.0f;
    public List<Vector3> Keys = new List<Vector3>();

    public Vector3 GetKey(int index) {
        return new Vector3(Keys[index].x, 1, Keys[index].z);
    }
}
