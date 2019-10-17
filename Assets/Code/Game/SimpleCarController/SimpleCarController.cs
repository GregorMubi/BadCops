using System;
using UnityEngine;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour {

    [SerializeField] private Rigidbody RigidBody = null;
    [Header("Settings")]
    [SerializeField] private List<SimpleAxleData> Axels = new List<SimpleAxleData>();
    [SerializeField] private float MaxMotorTorque = 400f;
    [SerializeField] private float MaxSteeringAngle = 30f;
    [SerializeField] private float BreakForce = 1;

    private static SimpleCarController Instance = null;

    public static Vector3 GetPosition() {
        return Instance.transform.position;
    }

    void Start() {
        Instance = this;
        RigidBody.centerOfMass += new Vector3(0, -0.2f, 0);
    }

    void Update() {
    }

    void FixedUpdate() {
        // INPUT
        float motor = MaxMotorTorque * Input.GetAxis("Vertical");
        float steering = MaxSteeringAngle * Input.GetAxis("Horizontal");
        double angle = AngleBetweenVectors(RigidBody.velocity, RigidBody.transform.forward);
        bool isMovingForward = angle < 90 && angle > -90;

        foreach (SimpleAxleData axle in Axels) {
            if (axle.Steering) {
                axle.LeftWheel.steerAngle = steering;
                axle.RightWheel.steerAngle = steering;
            }

            if (isMovingForward) {
                if (motor >= 0) {
                    if (axle.Motor) {
                        axle.LeftWheel.motorTorque = motor;
                        axle.RightWheel.motorTorque = motor;
                    }
                    if (axle.Breaks) {
                        axle.LeftWheel.brakeTorque = 0;
                        axle.RightWheel.brakeTorque = 0;
                    }
                } else {
                    if (axle.Breaks) {
                        axle.LeftWheel.brakeTorque = BreakForce;
                        axle.RightWheel.brakeTorque = BreakForce;
                    }
                    if (axle.Motor) {
                        axle.LeftWheel.motorTorque = 0;
                        axle.RightWheel.motorTorque = 0;
                    }
                }
            } else {
                if (motor > 0) {
                    if (axle.Breaks) {
                        axle.LeftWheel.brakeTorque = BreakForce;
                        axle.RightWheel.brakeTorque = BreakForce;
                    }
                    if (axle.Motor) {
                        axle.LeftWheel.motorTorque = 0;
                        axle.RightWheel.motorTorque = 0;
                    }
                } else {
                    if (axle.Motor) {
                        axle.LeftWheel.motorTorque = motor * 0.5f;
                        axle.RightWheel.motorTorque = motor * 0.5f;
                    }
                    if (axle.Breaks) {
                        axle.LeftWheel.brakeTorque = 0;
                        axle.RightWheel.brakeTorque = 0;
                    }
                }
            }

            ApplyLocalPositionToVisuals(axle.LeftWheel, axle.LeftWheelTransform);
            ApplyLocalPositionToVisuals(axle.RightWheel, axle.RightWheelTransform);
        }
    }

    // FINDS THE VISUAL WHEEL, CORRECTLY APPLIES THE TRANSFORM
    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform wheelTransform) {
        // GET POS/ROT
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        // APPLY POS/ROT
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    private double AngleBetweenVectors(Vector3 v1, Vector3 v2) {
        double sin = v1.x * v2.z - v2.x * v1.z;
        double cos = v1.x * v2.x + v1.z * v2.z;
        return Math.Atan2(sin, cos) * Mathf.Rad2Deg;
    }

}