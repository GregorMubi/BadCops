using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { kNormal, kHomingMissle, kLast};

public class ProjectileContoller : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    
    private Vector3 movementDir = Vector3.zero;
    private float speed = 0.0f; 
    private int damage = 0;

    ExplosionController explosionControllerPrefab = null;

    private ProjectileType type = ProjectileType.kNormal;
    private Transform hommingTarget = null;
    public void Init(Vector3 _movementDir, float _speed, int _damage, ExplosionController _explosionControllerPrefab)
    {
        movementDir = _movementDir;
        speed = _speed;
        rigidbody.velocity = _movementDir * _speed;
        explosionControllerPrefab = _explosionControllerPrefab;
        damage = _damage;
    }
    public void Init(Vector3 _movementDir, float _speed, int _damage, ExplosionController _explosionControllerPrefab, ProjectileType _type, Transform _hommingTarget)
    {
        Init(_movementDir, _speed, _damage, _explosionControllerPrefab);
        type = _type;
        hommingTarget = _hommingTarget;
        rigidbody.isKinematic = true;
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

        if (collision.collider.tag == "NPC") {
            Debug.Log("<color=green>ProjectileContoller::</color> <color=red>Hit NPC</color>");
            //TODO(Rok Kos): Use polling
            ExplosionController explosionController = Instantiate(explosionControllerPrefab, transform.position, Quaternion.identity, null);
            explosionController.PlayExplosion();
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (type == ProjectileType.kHomingMissle) {
            transform.position = Vector3.Lerp(transform.position, hommingTarget.position, Time.deltaTime);
        }
    }

    public int GetDamage() {
        return damage;
    }
}
