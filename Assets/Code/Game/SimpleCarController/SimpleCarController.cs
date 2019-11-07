using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public enum CarOwner {kPlayer, kAI, kLast };

public class SimpleCarController : MonoBehaviour {

    [SerializeField] private Rigidbody RigidBody = null;
    [SerializeField] private List<SimpleAxleData> Axels = new List<SimpleAxleData>();
    [SerializeField] private float MaxMotorTorque = 400f;
    [SerializeField] private float MaxSteeringAngle = 30f;
    [SerializeField] private float BreakForce = 1;
    [SerializeField] private GameObject WeaponGameObject = null;
    [SerializeField] private WeaponController WeaponController = null;
    [SerializeField] private AudioSource AudioSource = null;
    [SerializeField] private AudioSource SireneSource = null;
    [SerializeField] private Light RedSireneLights = null;
    [SerializeField] private Light BlueSireneLights = null;
    [SerializeField] private float IntesityChange = 0.1f;

    private bool Break = false;
    private CarOwner carOwner = CarOwner.kLast;
    private bool IsSirenePlaying = false;
    private bool IsRedBlinking = false;

    public void Init(CarOwner _carOwner) {
        carOwner = _carOwner;
    }

    void Start() {
        RigidBody.centerOfMass += new Vector3(0, -0.2f, 0);
        RedSireneLights.gameObject.SetActive(false);
        BlueSireneLights.gameObject.SetActive(false);
    }

    public float GetMaxSteeringAngle() {
        return MaxSteeringAngle;
    }

    public void Shoot() {
        if (WeaponController != null) {
            GameObject hommingTarget = null;
            if (WeaponController.GetWeaponType() == WeaponSpreadType.HommingMissle) {
                List<GameObject> npcObjects = LevelManager.Instance.worldLoader.GetAllNPC();
                hommingTarget = FindNearestNPC(npcObjects);
            }

            WeaponController.FireWeapon(transform.forward, hommingTarget);
        }
    }

    public void StopShooting() {
        WeaponController.StopShooting();
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

    public void EquipWeapon(WeaponData weaponData) {
        if (WeaponController != null) {
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
            AudioSource.pitch = RigidBody.velocity.magnitude * 0.5f;
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

            if (Break) {
                axle.LeftWheel.brakeTorque = BreakForce;
                axle.RightWheel.brakeTorque = BreakForce;
                axle.LeftWheel.motorTorque = 0;
                axle.RightWheel.motorTorque = 0;
            } else if (isMovingForward) {
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

    public void ForceBreak() {
        Break = true;
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform wheelTransform) {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    private GameObject FindNearestNPC(List<GameObject> npcObjects) {
        if (npcObjects.Count == 0) {
            return null;
        }

        float minDistance = float.MaxValue;
        GameObject nearest = npcObjects[0];

        foreach (GameObject g in npcObjects) {
            float currDistance = Vector3.Distance(transform.position, g.transform.position);
            if (currDistance < minDistance) {
                minDistance = currDistance;
                nearest = g;
            }
        }

        return nearest;
    }

    void OnCollisionEnter(Collision collision) {
        if (carOwner != CarOwner.kAI) {
            // TODO(Rok Kos): Implement logic for losing life for player
            return;
        }
        if (collision.gameObject.tag == "Projectile") {
            RigidBody.AddForce(Vector3.up * 300, ForceMode.Impulse);
            RigidBody.AddTorque(Vector3.right * 300, ForceMode.Impulse);
            // TODO(Rok Kos): Do something similiar as for human spawners
            //ProjectileContoller projectileContoller = collision.gameObject.GetComponent<ProjectileContoller>();
            //Health -= projectileContoller.GetDamage();
            //if (Health <= 0) {
            //    OnHumanDeath(this);
            //    GameManager.Instance.AddBadAssPoints(BadAssScore);
            //}
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (carOwner != CarOwner.kAI) {
            // TODO(Rok Kos): Implement logic for losing life for player
            return;
        }

        if (other.tag == "Projectile") {
            RigidBody.AddForce(Vector3.up * 300, ForceMode.Impulse);
            RigidBody.AddTorque(Vector3.right * 300, ForceMode.Impulse);

            // TODO(Rok Kos): Do something similiar as for human spawners
            //ProjectileContoller projectileContoller = other.gameObject.GetComponent<ProjectileContoller>();
            //Health -= projectileContoller.GetDamage();
            //if (Health <= 0) {
            //    OnHumanDeath(this);
            //    GameManager.Instance.AddBadAssPoints(BadAssScore);
            //}
        }
    }

    public bool GetSirenePlaying() {
        return IsSirenePlaying;
    }

    public void PlaySirene() {
        SireneSource.Play();
        IsSirenePlaying = true;
        RedSireneLights.gameObject.SetActive(true);
        BlueSireneLights.gameObject.SetActive(true);
        StartCoroutine("BlingSirene");
    }

    public void StopSirene() {
        SireneSource.Stop();
        IsSirenePlaying = false;
        RedSireneLights.gameObject.SetActive(false);
        BlueSireneLights.gameObject.SetActive(false);
        StopCoroutine("BlingSirene");
    }

    private IEnumerator BlingSirene() {
        while (true) {
            if (IsRedBlinking) {
                RedSireneLights.intensity += IntesityChange;
                BlueSireneLights.intensity -= IntesityChange;
                if (RedSireneLights.intensity > 2) {
                    IsRedBlinking = false;
                }
            } else {
                RedSireneLights.intensity -= IntesityChange;
                BlueSireneLights.intensity += IntesityChange;
                if (RedSireneLights.intensity <= 0) {
                    IsRedBlinking = true;
                }
            }
           
            yield return null;
        }
    }
}