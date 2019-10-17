using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private GameObject ObjectToFollow = null;

    Vector3 Offset = new Vector3(2.5f, 5, -5);

    public void Init(GameObject objectToFollow) {
        ObjectToFollow = objectToFollow;
    }

    // Update is called once per frame
    void Update() {
        if (ObjectToFollow != null) {
            transform.position = ObjectToFollow.transform.position + Offset;
            transform.LookAt(ObjectToFollow.transform);
        }
    }
}
