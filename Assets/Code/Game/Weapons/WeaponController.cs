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
            Fire(dir);
        }
        else {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Weapon on cooldown</color>");
        }
        
    }

    private void Update() {
        coolDown -= Time.deltaTime;
    }

    private void Fire(Vector3 dir) {
        //TODO(Rok Kos): Use polling
        ProjectileContoller projectile = Instantiate(weaponData.projectile, transform.position, Quaternion.identity, transform);
        projectile.Init(dir, weaponData.speed, weaponData.explosion);
        projectile.name = weaponData.weaponName + "_projectile_" + projectileCount;
        projectileCount++;
    }
}
