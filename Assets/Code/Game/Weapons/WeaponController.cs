using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    private float coolDown = 0.0f;

    public void FireWeapon() {
        if (coolDown <= 0.0f) {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Fire</color>");
            coolDown = weaponData.rateOfFire;
            Fire();
        }
        else {
            Debug.Log("<color=green>WeaponController::</color> <color=red>Weapon on cooldown</color>");
        }
        
    }

    private void Update() {
        coolDown -= Time.deltaTime;
    }

    private void Fire() {
    }
}
