﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour {

    [SerializeField] private Rigidbody RigidBody = null;
    [SerializeField] private List<SimpleAxleData> Axels = new List<SimpleAxleData>();
    [SerializeField] private float MaxMotorTorque = 400f;
    [SerializeField] private float MaxSteeringAngle = 30f;
    [SerializeField] private float BreakForce = 1;
    [SerializeField] private GameObject WeaponGameObject = null;
    [SerializeField] private WeaponController WeaponController = null;
    [SerializeField] private AudioSource AudioSource = null;

    void Start() {
        RigidBody.centerOfMass += new Vector3(0, -0.2f, 0);
    }

    public float GetMaxSteeringAngle() {
        return MaxSteeringAngle;
    }

    public void Shoot() {
        if (WeaponController != null) {
            WeaponController.FireWeapon(transform.forward);
        }
    }

    public void EnableEngineSound(bool enable) {
        if (AudioSource != null) {
            if (enable && !AudioSource.isPlaying) {
                AudioSource.Play(0);
            } else if (!enable && AudioSource.isPlaying) {
                AudioSource.Stop();
            }
        }
    }

    public void EquipWeaon(int weaponIndex) {
        if (WeaponController != null) {
            WeaponData weaponData = LevelManager.Instance.GetWeaponDatas().WeaponDatas[weaponIndex];
            WeaponController.Init(weaponData, WeaponGameObject);
        }
    }

    public void SetTransform(Vector3 position, float rotation) {
        RigidBody.transform.position = position;
        RigidBody.transform.eulerAngles = new Vector3(0, rotation, 0);
        RigidBody.velocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
    }

    void Update() {
        if (AudioSource != null) {
            AudioSource.pitch = RigidBody.velocity.magnitude * 0.5f - 0.5f;
        }
    }

    public void UpdateInput(float motor, float steering) {
        motor *= MaxMotorTorque;
        steering *= MaxSteeringAngle;
        double angle = StaticGameHelper.AngleBetweenVectors(RigidBody.velocity, RigidBody.transform.forward);
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

    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform wheelTransform) {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}