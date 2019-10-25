using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour {

    [SerializeField] private Rigidbody Rigidbody = null;
    [SerializeField] private float BadAssScore = 1.0f;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            if (Rigidbody.isKinematic) {
                Rigidbody.isKinematic = false;
                GameManager.Instance.AddBadAssPoints(BadAssScore);
            }
        }
    }
}
