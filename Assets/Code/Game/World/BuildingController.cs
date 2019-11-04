using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private Transform parentSmoke = null;
    [SerializeField] private GameObject vfxSmokePrefab = null;

    private bool isBurning = false;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Projectile" && !isBurning) {
            GameObject smoke = Instantiate(vfxSmokePrefab, parentSmoke);
            smoke.transform.localPosition = Vector3.zero;
            Destroy(smoke, 10f);
            isBurning = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Projectile" && !isBurning) {
            GameObject smoke = Instantiate(vfxSmokePrefab, parentSmoke);
            smoke.transform.localPosition = Vector3.zero;
            Destroy(smoke, 10f);
            isBurning = true;
        }
    }

}
