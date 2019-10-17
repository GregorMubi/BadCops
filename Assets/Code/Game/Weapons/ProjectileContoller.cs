using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContoller : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    
    private Vector3 movementDir = Vector3.zero;
    private float speed = 0.0f;

    ExplosionController explosionControllerPrefab = null;

    public void Init(Vector3 _movementDir, float _speed, ExplosionController _explosionControllerPrefab)
    {
        movementDir = _movementDir;
        speed = _speed;
        rigidbody.velocity = _movementDir * _speed;
        explosionControllerPrefab = _explosionControllerPrefab;


    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.layer == 8) {
            Debug.LogFormat("<color=green>ProjectileContoller::</color> <color=red>End Pos:{0:0.00} {1:0.00} {2:0.00}</color>", transform.position.x, transform.position.y, transform.position.z);
            Debug.Log("<color=green>ProjectileContoller::</color> <color=red>Hit Enviroment</color>");
            //TODO(Rok Kos): Use polling
            ExplosionController explosionController = Instantiate(explosionControllerPrefab, transform.position, Quaternion.identity, null);
            explosionController.PlayExplosion();
            Destroy(this.gameObject);


        }
    }
}
