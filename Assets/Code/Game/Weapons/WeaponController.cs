using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    private WeaponData WeaponData;
    private float coolDown = 0.0f;
    private uint projectileCount = 0;
    private GameObject WeaponPositionGameObject;
    private ProjectileContoller laserInstance = null;

    public void Init(WeaponData weaponData, GameObject weaponPositionObject) {
        WeaponData = weaponData;
        WeaponPositionGameObject = weaponPositionObject;
        laserInstance = null;
    }

    private const float kGoldenRationInversed = 0.618033f; // this is: 1 / golden ratio 
    public void FireWeapon(Vector3 dir, GameObject hommingTarget = null) {
        if (WeaponData == null) {
            return;
        }

        if (coolDown >= 1.0f / (float) WeaponData.rateOfFire) {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Fire</color>");
            coolDown = 0;

            switch (WeaponData.weaponSpreadType) {
                case WeaponSpreadType.Bullet: {
                        Fire(dir, Vector3.zero);
                        break;
                    }
                case WeaponSpreadType.Hydra: {
                        for (int i = 0; i < WeaponData.numberOfBulletsPerShot; ++i) {
                            Fire(dir + Vector3.up * 0.1f * i, Vector3.zero);
                        }
                        break;
                    }

                case WeaponSpreadType.Shotgun: {

                        int numPoints = WeaponData.numberOfBulletsPerShot;
                        float power = 0.5f;
                        float turnFraction = kGoldenRationInversed;

                        for (int i = 0; i < numPoints; ++i) {
                            float distance = Mathf.Pow(i / (numPoints - 1.0f), power);
                            float angle = 2 * Mathf.PI * turnFraction * i;
                            float xOffset = Mathf.Cos(angle) * distance;
                            float yOffset = Mathf.Sin(angle) * distance;
                            Fire(dir, new Vector3(xOffset, 0, yOffset));
                        }

                        break;
                    }

                case WeaponSpreadType.HommingMissle: {
                        Fire(dir, Vector3.zero, hommingTarget);
                        break;
                    }

                case WeaponSpreadType.kLaser: {
                    if (laserInstance == null) {
                        laserInstance = Instantiate(WeaponData.projectile, WeaponPositionGameObject.transform.position, Quaternion.identity, WeaponPositionGameObject.transform);
                        laserInstance.transform.localScale = new Vector3(1, 1, 84.99f);
                        laserInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        laserInstance.Init(dir, WeaponData.speed, WeaponData.damage, WeaponData.explosion, ProjectileType.kLaser);
                    }
                    break;
                }

                default:
                    Debug.Log("<color=green>WeaponController::</color> <color=red>Not Implemented Spread type</color>");
                    break;
            }

        } else {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Weapon on cooldown</color>");
        }

    }

    public void StopShooting() {
        if (WeaponData.weaponSpreadType == WeaponSpreadType.kLaser && laserInstance != null) {
            // TODO(Rok Kos): Maybe just disable it
            Destroy(laserInstance.gameObject);
            laserInstance = null;
        }
    }

    private void Update() {
        coolDown += Time.deltaTime;
    }

    private void Fire(Vector3 dir, Vector3 positionOffset, GameObject hommingTarget = null) {
        if (WeaponData == null) {
            return;
        }
        //TODO(Rok Kos): Use polling
        ProjectileContoller projectile = Instantiate(WeaponData.projectile, WeaponPositionGameObject.transform.position + positionOffset, Quaternion.identity, null);

        if (hommingTarget == null) {
            projectile.Init(dir, WeaponData.speed, WeaponData.damage, WeaponData.explosion);
        } else {
            projectile.Init(dir, WeaponData.speed, WeaponData.damage, WeaponData.explosion, ProjectileType.kHomingMissle, hommingTarget);
        }


        projectile.name = WeaponData.weaponName + "_projectile_" + projectileCount;
        projectileCount++;
        Destroy(projectile.gameObject, 10);
    }

    public WeaponSpreadType GetWeaponType() {
        return WeaponData.weaponSpreadType;
    }

    private void OnDestroy() {
        if (laserInstance != null) {
            Destroy(laserInstance.gameObject);
        }
    }
}
