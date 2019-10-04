using System;
using UnityEngine;

[Serializable]
public class IsometricTrackKeyframeData {
    public Vector2 Position;
    public float Rotation;
    public float RotationPower;
    public float CarRotation;

    public IsometricTrackKeyframeData(Vector2 position, float rotation, float rotationPower, float carRotation) {
        Position = position;
        Rotation = rotation;
        RotationPower = rotationPower;
        CarRotation = carRotation;
    }

    public IsometricTrackKeyframeData(IsometricTrackKeyframeData keyframe) {
        Position = keyframe.Position;
        Rotation = keyframe.Rotation;
        RotationPower = keyframe.RotationPower;
        CarRotation = keyframe.CarRotation;
    }
}
