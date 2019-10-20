using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    private float coolDown = 0.0f;
    private uint projectileCount = 0;


    private const float kGoldenRationInversed = 0.618033f; // this is: 1 / golden ratio 
    public void FireWeapon(Vector3 dir) {
        if (coolDown <= 0.0f) {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Fire</color>");
            coolDown = weaponData.rateOfFire;

            switch (weaponData.weaponSpreadType) {
                case WeaponSpreadType.Bullet: {
                        Fire(dir, Vector3.zero);
                        break;
                }
                case WeaponSpreadType.Hydra:
                {
                    for (int i = 0; i < weaponData.numberOfBulletsPerShot; ++i)
                    {
                        Fire(dir + Vector3.up * 0.1f * i, Vector3.zero);
                    }
                    break;
                }

                case WeaponSpreadType.Shotgun:
                    {

                        int numPoints = weaponData.numberOfBulletsPerShot;
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

                default:
                    Debug.Log("<color=green>WeaponController::</color> <color=red>Not Implemented Spread type</color>");
                    break;
            }
            
        }
        else {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Weapon on cooldown</color>");
        }
        
    }

    private void Update() {
        coolDown -= Time.deltaTime;
    }

    private void Fire(Vector3 dir, Vector3 positionOffset) {
        //TODO(Rok Kos): Use polling
        ProjectileContoller projectile = Instantiate(weaponData.projectile, transform.position + positionOffset, Quaternion.identity, transform);
        projectile.Init(dir, weaponData.speed, weaponData.explosion);
        projectile.name = weaponData.weaponName + "_projectile_" + projectileCount;
        projectileCount++;
        Destroy(projectile.gameObject, 10);
    }
}
