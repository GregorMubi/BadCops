using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private GameObject ObjectToFollow = null;

    Vector3 Offset = new Vector3(2.5f, 5, -5);
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        transform.position = ObjectToFollow.transform.position + Offset;
        transform.LookAt(ObjectToFollow.transform);
    }
}
