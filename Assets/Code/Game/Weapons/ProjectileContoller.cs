using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { kNormal, kHomingMissle, kLaser, kLast};

public class ProjectileContoller : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] BoxCollider boxCollider;

    private Vector3 movementDir = Vector3.zero;
    private float speed = 0.0f; 
    private int damage = 0;

    ExplosionController explosionControllerPrefab = null;

    private ProjectileType type = ProjectileType.kNormal;
    private GameObject hommingTarget = null;
    public void Init(Vector3 _movementDir, float _speed, int _damage, ExplosionController _explosionControllerPrefab, ProjectileType _type = ProjectileType.kNormal)
    {
        movementDir = _movementDir;
        speed = _speed;
        rigidbody.velocity = _movementDir * _speed;
        explosionControllerPrefab = _explosionControllerPrefab;
        damage = _damage;
        type = _type;
    }
    public void Init(Vector3 _movementDir, float _speed, int _damage, ExplosionController _explosionControllerPrefab, ProjectileType _type, GameObject _hommingTarget)
    {
        Init(_movementDir, _speed, _damage, _explosionControllerPrefab, _type);
        hommingTarget = _hommingTarget;
        rigidbody.isKinematic = true;
        boxCollider.isTrigger = true;
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
            ExplosionController explosionController = Instantiate(explosionControllerPrefab, collision.collider.transform.position, Quaternion.identity, null);
            explosionController.PlayExplosion();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "NPC") {
            Debug.Log("<color=green>ProjectileContoller::</color> <color=red>Hit NPC</color>");
            //TODO(Rok Kos): Use polling
            ExplosionController explosionController = Instantiate(explosionControllerPrefab, other.transform.position, Quaternion.identity, null);
            explosionController.PlayExplosion();
            if (type != ProjectileType.kLaser) {
                Destroy(this.gameObject);
            }
        }
    }

    private void Update()
    {
        if (type == ProjectileType.kHomingMissle) {
            transform.position += (hommingTarget.transform.position - transform.position).normalized * speed * Time.deltaTime;
            //transform.position = Vector3.Lerp(transform.position, hommingTarget.transform.position, Time.deltaTime * 2);
            Debug.DrawLine(transform.position, hommingTarget.transform.position, Color.red, 1);
        }
    }

    public int GetDamage() {
        return damage;
    }
}
