using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAxleData : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] public bool Motor = false;
    [SerializeField] public bool Steering = false;
    [SerializeField] public bool Breaks = false;

    [Header("References")]
    [SerializeField] public WheelCollider LeftWheel = null;
    [SerializeField] public WheelCollider RightWheel = null;
    [SerializeField] public Transform LeftWheelTransform = null;
    [SerializeField] public Transform RightWheelTransform = null;
}
