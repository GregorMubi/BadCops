using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectDebug : MonoBehaviour
{
    [SerializeField] WeaponController weaponController;
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F)) {
            weaponController.FireWeapon();
        }

#endif

    }
}
