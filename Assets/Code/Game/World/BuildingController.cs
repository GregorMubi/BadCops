using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private Transform parentSmoke = null;
    [SerializeField] private GameObject vfxSmokePrefab = null;

    private bool isBurning = false;
    private float burningTimer = 0.0f;
    private const float kTimeOfBurn = 10.0f;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Projectile" && !isBurning) {
            GameObject smoke = Instantiate(vfxSmokePrefab, parentSmoke);
            smoke.transform.localPosition = Vector3.zero;
            Destroy(smoke, kTimeOfBurn);
            isBurning = true;
            GameManager.Instance.AddBadAssPoints(3.0f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Projectile" && !isBurning) {
            GameObject smoke = Instantiate(vfxSmokePrefab, parentSmoke);
            smoke.transform.localPosition = Vector3.zero;
            Destroy(smoke, kTimeOfBurn);
            isBurning = true;
            GameManager.Instance.AddBadAssPoints(3.0f);
        }
    }

    private void Update() {
        if (isBurning) {
            burningTimer += Time.deltaTime;
            if (burningTimer > kTimeOfBurn) {
                isBurning = false;
            }
        }
    }

}
