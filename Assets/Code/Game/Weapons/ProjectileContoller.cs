using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContoller : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] ParticleSystem ps_boom;
    private Vector3 movementDir = Vector3.zero;
    private float speed = 0.0f;

    public void Init(Vector3 _movementDir, float _speed)
    {
        rigidbody.velocity = movementDir * speed;
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.layer == 8) {
            Debug.Log("<color=green>ProjectileContoller::</color> <color=red>Hit Enviroment</color>");
            // TODO(Rok Kos): Play some particles
            Destroy(this.gameObject);
        }
    }
}
