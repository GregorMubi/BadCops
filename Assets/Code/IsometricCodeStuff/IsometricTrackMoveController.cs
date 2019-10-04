using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricTrackMoveController : MonoBehaviour {

    [SerializeField] private Transform ObjectToMove = null;
    [SerializeField] private Transform ObjectToRotate = null;

    private bool IsMoving = false;
    private int KeyframeIndex = 0;
    private float TrackProgress = 0;
    private Vector3 PrevPosition = Vector3.zero;
    private IsometricTrackData TrackData = null;

    private float MovementSpeed = 1f;

    public void SetTrack(IsometricTrackData trackData) {
        TrackData = trackData;
        StartMoving();

        //for (int i = 0; i < trackData.Keyframes.Count; i++) {
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    go.transform.localScale = Vector3.one * 0.5f;
        //    go.transform.position = GetPointWorldPosition(trackData.Keyframes[i].Position);
        //    go.transform.SetParent(gameObject.transform);
        //}
    }

    void Update() {
        if (IsMoving) {
            TrackProgress += Time.deltaTime * MovementSpeed;

            if (TrackProgress > 1) {
                TrackProgress -= 1;
                KeyframeIndex++;
                if (KeyframeIndex >= TrackData.Keyframes.Count - 1) {
                    IsMoving = false;
                    return;
                }
            }

            IsometricTrackKeyframeData keyframe0 = TrackData.Keyframes[KeyframeIndex];
            IsometricTrackKeyframeData keyframe1 = TrackData.Keyframes[KeyframeIndex + 1];
            Vector3 p1 = keyframe0.Position;
            Vector3 p2 = keyframe1.Position;
            Vector3 t1 =
                p1 - (new Vector3(Mathf.Sin(keyframe0.Rotation * Mathf.Deg2Rad),
                    Mathf.Cos(keyframe0.Rotation * Mathf.Deg2Rad), 0)).normalized * keyframe0.RotationPower;
            Vector3 t2 =
                p2 + (new Vector3(Mathf.Sin(keyframe1.Rotation * Mathf.Deg2Rad),
                    Mathf.Cos(keyframe1.Rotation * Mathf.Deg2Rad), 0)).normalized * keyframe1.RotationPower;
            Vector3 pointOnCurve = GetPointOnBezierCurve(p1, t1, t2, p2, TrackProgress);

            Vector3 position = GetPointWorldPosition(pointOnCurve);
            ObjectToMove.position = position;

            Vector3 delta = position - PrevPosition;
            float rotation = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            PrevPosition = position;

            ObjectToRotate.localEulerAngles = new Vector3(0, GetPointWorldRotation(rotation) + 90, 0);
        }
    }

    private void StartMoving() {
        IsMoving = true;

        PrevPosition = GetPointWorldPosition(TrackData.Keyframes[0].Position);
        ObjectToMove.position = PrevPosition;
        ObjectToRotate.localEulerAngles = new Vector3(0, GetPointWorldRotation(TrackData.Keyframes[0].Rotation), 0);
    }

    private Vector3 GetPointWorldPosition(Vector3 editorPoint) {
        //some space repositioning from 2d editor view coordinates to scene world coordinates
        editorPoint.y = -editorPoint.y;
        editorPoint *= 0.01f;
        editorPoint.x -= 2.7f;
        return editorPoint;
    }

    private float GetPointWorldRotation(float rotation) {
        rotation = -rotation;
        rotation += 135;
        return rotation;
    }

    private Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        float u = 1f - t;
        float t2 = t * t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t3 = t2 * t;
        Vector3 result = (u3) * p0 + (3f * u2 * t) * p1 + (3f * u * t2) * p2 + (t3) * p3;
        return result;
    }
}
