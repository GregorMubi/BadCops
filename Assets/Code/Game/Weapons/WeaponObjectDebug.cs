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
            weaponController.FireWeapon(Vector3.up);
        }

        if (Input.GetMouseButtonDown(0)) {
            Vector3 origin = transform.position;
            Vector3 mousePos = Input.mousePosition;
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit);
            }
            Vector3 hitPos = hit.point;
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * 1000, Color.red, 10);


            Vector3 dir = hitPos - origin;
            Debug.LogFormat("<color=green>WeaponObjectDebug::</color> <color=red>Mouse Pos: {0:0.00} {1:0.00} {2:0.00}</color>", mousePos.x, mousePos.y, mousePos.z);
            Debug.LogFormat("<color=green>WeaponObjectDebug::</color> <color=red>World point: {0:0.00} {1:0.00} {2:0.00}</color>", hitPos.x, hitPos.y, hitPos.z);
            Debug.LogFormat("<color=green>WeaponObjectDebug::</color> <color=red>Direction of projectile: {0:0.00} {1:0.00} {2:0.00}</color>", dir.x, dir.y, dir.z);
            weaponController.FireWeapon(dir);
        }
#endif

    }
}
