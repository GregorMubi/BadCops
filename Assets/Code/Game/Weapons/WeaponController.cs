using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    private float coolDown = 0.0f;
    private uint projectileCount = 0;

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
                        int i = 1;
                        float radius = 1.5f;
                        int partition = 7;
                        while (i <= weaponData.numberOfBulletsPerShot) {
                            if (i % partition == 0) {
                                radius *= 2;
                            }

                            float angle = 360 / ((i % partition) + 1);
                            angle *= Mathf.Deg2Rad;
                            float xOffset = Mathf.Cos(angle) * radius;
                            float yOffset = Mathf.Sin(angle) * radius;

                            // TODO(Rok Kos): Figure out in which direction is vector turned
                            Fire(dir, new Vector3(xOffset, 0, yOffset));
                            i++;
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
