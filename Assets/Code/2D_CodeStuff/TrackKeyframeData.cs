using System;
using UnityEngine;

[Serializable]
public class TrackKeyframeData {
    public Vector2 Position;
    public float Rotation;
    public float RotationPower;
    public float CarRotation;

    public TrackKeyframeData(Vector2 position, float rotation, float rotationPower, float carRotation) {
        Position = position;
        Rotation = rotation;
        RotationPower = rotationPower;
        CarRotation = carRotation;
    }

    public TrackKeyframeData(TrackKeyframeData keyframe) {
        Position = keyframe.Position;
        Rotation = keyframe.Rotation;
        RotationPower = keyframe.RotationPower;
        CarRotation = keyframe.CarRotation;
    }
}
