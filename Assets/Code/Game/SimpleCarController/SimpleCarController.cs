using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour {

    [SerializeField] private Rigidbody RigidBody = null;
    [Header("Settings")]
    [SerializeField] private List<SimpleAxleData> Axels = new List<SimpleAxleData>();
    [SerializeField] private float MaxMotorTorque = 400f;
    [SerializeField] private float MaxSteeringAngle = 30f;
    [SerializeField] private float BreakForce = 1;

    private float CurrentBrakeTorque = 0f;

    void Start() {
        RigidBody.centerOfMass += new Vector3(0, -0.2f, 0);
    }

    void Update() {
        // BRAKE
        if (Input.GetButton("Brake")) {
            CurrentBrakeTorque = BreakForce;
        } else {
            CurrentBrakeTorque = 0f;
        }

        // JUMP
        if (Input.GetButtonDown("Jump")) {
            RigidBody.AddForce(RigidBody.centerOfMass + new Vector3(0f, 1000, 0f), ForceMode.Impulse);
        }

        // BOOST
        if (Input.GetButton("Boost")) {
            RigidBody.AddForce(transform.forward * 1000, ForceMode.Impulse);
        }
    }

    void FixedUpdate() {
        // INPUT
        float motor = MaxMotorTorque * Input.GetAxis("Accelerator");
        float steering = MaxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (SimpleAxleData axle in Axels) {
            if (axle.Steering) {
                axle.LeftWheel.steerAngle = steering;
                axle.RightWheel.steerAngle = steering;
            }

            if (axle.Motor) {
                axle.LeftWheel.motorTorque = motor;
                axle.RightWheel.motorTorque = motor;
            }

            if (axle.Breaks) {
                axle.LeftWheel.brakeTorque = CurrentBrakeTorque;
                axle.RightWheel.brakeTorque = CurrentBrakeTorque;
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

}