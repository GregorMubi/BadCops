using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    CarController carController;

    Vector3 offset;
    void Start()
    {
        offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = carController.transform.localPosition + offset;
    }
}
