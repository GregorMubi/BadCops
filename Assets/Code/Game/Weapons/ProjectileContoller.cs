using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContoller : MonoBehaviour
{
    private Vector3 movementDir = Vector3.zero;
    private float speed = 0.0f;

    public void Init(Vector3 _movementDir, float _speed)
    {
        movementDir = _movementDir;
        speed = _speed;
    }

    private void Update()
    {
        transform.position += movementDir * Time.deltaTime * speed;
    }
}
